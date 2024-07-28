using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;
using UnityEditor.Rendering;
using UnityEngine.UIElements;

public abstract class Item : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private string Name;
    private int colliderCount;
    private  RectTransform rectTransform;

    private UnityEngine.UI.Image image;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Color imageColor;

    //лучи
    private List<Collider2D> itemColliders = new List<Collider2D>();
    private List<RaycastHit2D> hits = new List<RaycastHit2D>();
    private List<RaycastStructure> careHits = new List<RaycastStructure>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //не лучи
    private Transform bagTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<UnityEngine.UI.Image>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        imageColor = image.color;

        var collidersArray = rectTransform.GetComponents<Collider2D>();
        for (int i = 0; i < collidersArray.Count(); i++)
        {
            itemColliders.Add(collidersArray[i]);
        }
        colliderCount = collidersArray.Count();
    }

    

    void Update()
    {
        var КЕ = 1;
        if (Input.GetKey(KeyCode.R))
        {
            rectTransform.Rotate(Vector2.left);
            Debug.Log("123");
        }
    }

    void CreateRaycast(List<Collider2D> itemColliders)
    {
        foreach (var collider in itemColliders)
        {
            hits.Add(Physics2D.Raycast(collider.bounds.center, -Vector2.up));
        }
    }

    public void RaycastEvent()
    {
        hits.Clear();
        foreach (var collider in itemColliders)
        {
            hits.Add(Physics2D.Raycast(collider.bounds.center, -Vector2.up));
        }
        
        foreach (var Carehit in careHits)
        {
            //foreach (var h in hits.Where(e => e.collider.name == Carehit.raycastHit.collider.name))
               // Debug.Log("h " + h.collider.name);
            //Debug.Log("res " + hits.Where(e => e.collider.name == Carehit.raycastHit.collider.name).Count().ToString());

            if ((hits.Where(e => e.collider != null && e.collider.name == Carehit.raycastHit.collider.name).Count() == 0 /*&& careHits.Count != colliderCount*/) || hits.Where(e => e.collider == null).Count() == colliderCount)
            {
                Carehit.raycastHit.collider.GetComponent<UnityEngine.UI.Image>().color = imageColor;
                Carehit.isDeleted = true;
            }



            /*
            hits: 1,2,3,4 
            careHits: 1,2,3,4


            hits:1,2,null,null
            careHits: 1,2,3,4

            */
        }

        /*
        foreach(var careHit in careHits.Where(e => e.isDeleted == true))
        {
            careHit.raycastHit.collider.GetComponent<Image>().color = imageColor;
        }*/
        careHits.RemoveAll(e => e.isDeleted == true);


        foreach (var hit in hits)
        {
           
            if (hit.collider != null)
            {
                //Debug.Log(hit.collider.name);
                
                if (careHits.Where(e => e.raycastHit.collider != null && e.raycastHit.collider.name == hit.collider.name).Count() == 0)
                {
                    hit.collider.GetComponent<UnityEngine.UI.Image>().color = Color.red;
                    careHits.Add(new RaycastStructure(hit));//объекты
                    bagTransform = hit.transform.parent.transform;
                }
            }


        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        //image.color. = 0.5f;
        //image.raycastTarget = false;
        //canvasGroup.blocksRaycasts = false;
        //var test = eventData.pointerDrag.transform.localPosition - eventData.pointerDrag.transform.GetChild(0).localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        float mw = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKeyDown(KeyCode.R))
        {
            rectTransform.Rotate(Vector2.left);
        }
        if (mw > 0.1)
        {
            rectTransform.Rotate(Vector2.left);
        }
        if (mw < -0.1)
        {
            rectTransform.Rotate(Vector2.right);
        }

        RaycastEvent();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        
        image.color = imageColor;
        image.raycastTarget = true;
        canvasGroup.blocksRaycasts = true;
        //var tre = careHits.AsQueryable().Distinct().Count();
       // var tre2 = careHits.AsQueryable().Distinct();
        
        if (careHits.Count() == colliderCount)
        {
            if (hits.Where(e => e.collider == null).Count() == 0)
            {
                var minX = careHits[0].raycastHit.collider.transform.localPosition.x;
                var maxY = careHits[0].raycastHit.collider.transform.localPosition.y;
                Vector2 colliderPos = careHits[0].raycastHit.collider.transform.localPosition;
            
                for (int i = 1; i < careHits.Count; i++)
                {
                    if (careHits[i].raycastHit.collider.transform.localPosition.x <= minX && careHits[i].raycastHit.collider.transform.localPosition.y >= maxY)
                    {
                        minX = careHits[i].raycastHit.collider.transform.localPosition.x;
                        maxY = careHits[i].raycastHit.collider.transform.localPosition.y;
                        colliderPos = careHits[i].raycastHit.collider.transform.localPosition;
                    }
                }
                rectTransform.SetParent(bagTransform);
                rectTransform.localPosition = colliderPos - itemColliders[0].offset;
                foreach(var careHit in careHits)
                {
                    careHit.raycastHit.collider.GetComponent<UnityEngine.UI.Image>().color = imageColor;
                }
            }
            else
            {

            }
        }
        careHits.Clear();
        
    }

}
