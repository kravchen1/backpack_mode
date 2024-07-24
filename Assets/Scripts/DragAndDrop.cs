using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public RectTransform rectTransform;
    private Image image;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Color imageColor;
    //private List<Transform> cellList = new List<Transform>();
    private List<Vector2> vectorList = new List<Vector2>();
    private List<Collider2D> colliders = new List<Collider2D>();
    private Vector3 offset;
    private Transform bagTransform;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        imageColor = image.color;
    }
    private void Start()
    {
        var collidersArray = rectTransform.GetComponents<Collider2D>();
        for (int i = 0; i < collidersArray.Count(); i++)
        {
            colliders.Add(collidersArray[i]);
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        image.color = new Color(0f, 255f, 200f, 0.7f);
        image.raycastTarget = false;
        canvasGroup.blocksRaycasts = false;
        //var test = eventData.pointerDrag.transform.localPosition - eventData.pointerDrag.transform.GetChild(0).localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        image.color = imageColor;
        image.raycastTarget = true;
        canvasGroup.blocksRaycasts = true;
        Debug.Log("OnEndDragVectorListCount:" + vectorList.Count.ToString());
        if (vectorList.Count > 0)
        {
            var minX = vectorList[0].x;
            var maxY = vectorList[0].y;
            Vector2 colliderPos = vectorList[0];
            if (vectorList.Count == 4)
            {
                for (int i = 1; i < vectorList.Count; i++)
                {
                    if (vectorList[i].x <= minX && vectorList[i].y >= maxY)
                    {
                        minX = vectorList[i].x;
                        maxY = vectorList[i].y;
                        colliderPos = vectorList[i];
                    }
                }
                rectTransform.SetParent(bagTransform);
                rectTransform.localPosition = colliderPos - colliders[0].offset;
            }
        }
        vectorList.Clear();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var cellTransform = collision.GetComponent<RectTransform>();
        bagTransform = cellTransform.parent.transform;
        if (vectorList.Where(e => e == (Vector2)cellTransform.localPosition).Count() == 0 && vectorList.Count()<4)
            vectorList.Add((Vector2)cellTransform.localPosition);
        Debug.Log("GlobalPosition:" + (Vector2)cellTransform.position);
        Debug.Log(collision.name + " Trigger " + this.name);
        Debug.Log("OnTriggerEnterVectorListCount:" + vectorList.Count.ToString());
        //this.GetComponent<Image>().color = new Color32(255, 255, 225, 100);
        //PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        //// globaltest.text = PlayerPrefs.GetInt("level").ToString();
        //PlayerPrefs.Save();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var cellTransform = collision.GetComponent<RectTransform>();
        //Debug.Log("OnTriggerExitVectorListCount:" + vectorList.Count.ToString());
        if(vectorList.Count() == 4)
            vectorList.Remove(cellTransform.localPosition);
        Debug.Log(collision.name + " AnTrigger " + this.name);
        Debug.Log("OnTriggerExitVectorListCount:" + vectorList.Count.ToString());
        //this.GetComponent<Image>().color = color;
    }
}