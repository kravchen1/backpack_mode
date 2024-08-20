using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;
using UnityEditor.Rendering;
using UnityEngine.UIElements;
using static UnityEngine.RectTransform;
using System.Threading;
using Unity.VisualScripting;
using System;

public abstract class Item : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public int speedRotation = 500;
    public string Name;
    public int colliderCount;

    public float startRectTransformZ;

    public SpriteRenderer image;
    public Canvas canvas;
    //protected CanvasGroup canvasGroup;
    public Color imageColor;
    public string prefabOriginalName;


    //ëó÷è
    public List<BoxCollider2D> itemColliders = new List<BoxCollider2D>();
    public List<RaycastHit2D> hits = new List<RaycastHit2D>();
    public List<RaycastHit2D> hitsForBackpack= new List<RaycastHit2D>();
    public List<RaycastStructure> careHits = new List<RaycastStructure>();
    public List<RaycastStructure> careHitsForBackpack = new List<RaycastStructure>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //íå ëó÷è
    public Transform bagTransform;
    public BoxCollider2D[] collidersArray;

    public Rigidbody2D rb;


    public RectTransform rectTransform;

    public bool firstTap = true; //êîñòûëü, íî ñîðè
    public bool needToRotate;
    public bool needToDynamic = false;
    public bool needToRotateToStartRotation = false;




    void initializationItemColliders()
    {
        if (gameObject.name.Contains("bag"))
        {
            collidersArray = new BoxCollider2D[rectTransform.childCount];
            for (int i = 0; i < rectTransform.childCount; i++)
                collidersArray[i] = rectTransform.GetChild(i).GetComponent<BoxCollider2D>();
        }
        else
            collidersArray = rectTransform.GetComponents<BoxCollider2D>();
        itemColliders.Clear();
        for (int i = 0; i < collidersArray.Count(); i++)
        {
            itemColliders.Add(collidersArray[i]);
        }
        colliderCount = collidersArray.Count();
    }
    public void Initialization()
    {
        rb = GetComponent<Rigidbody2D>();
        rectTransform = GetComponent<RectTransform>();
        startRectTransformZ = rectTransform.eulerAngles.z;
        image = GetComponent<SpriteRenderer>();
        canvas = GetComponentInParent<Canvas>();
        imageColor = GetComponent<SpriteRenderer>().color;
        needToRotate = false;

        initializationItemColliders();
    }
    void Awake()
    {
        Initialization();
    }
    public void Rotate()
    {
        if (Input.GetKeyDown(KeyCode.R) && needToRotate)
        {
            Vector3 newRotation = new Vector3(0, 0, 90);
            rectTransform.Rotate(newRotation);
            Physics2D.SyncTransforms();
            RaycastEvent();
        }
    }
    public void SwitchDynamicStatic()
    {
        if (needToDynamic)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
    }
    public void RotationToStartRotation()
    {
        if (needToRotateToStartRotation)
        {
            if (rectTransform.eulerAngles.z >= -5 && rectTransform.eulerAngles.z <= 5)
            {
                needToRotateToStartRotation = false;
                rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else
            {
                rectTransform.Rotate(0, 0, speedRotation * Time.deltaTime);
            }
        }
    }
    public void TapFirst()
    {
        if (firstTap)
        {
            firstTap = false;
            rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }
    public void TapRotate()
    {
        needToRotate = true;
        if (needToDynamic)
        {
            needToRotateToStartRotation = true;
        }
        else
        {

        }
        needToDynamic = false;
    }
    public void DeleteNestedObject()
    {
        var cellList = GameObject.Find("backpack").GetComponentsInChildren<Cell>();
        foreach (var cell in cellList)
        {
            if (cell.nestedObject != null && cell.nestedObject.name == gameObject.name)
            {
                cell.nestedObject = null;
            }
        }
    }
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        TapFirst();
        TapRotate();
        DeleteNestedObject();
        gameObject.transform.SetParent(GameObject.Find("backpack").transform);
    }
    void Update()
    {
        Rotate();
        SwitchDynamicStatic();
        RotationToStartRotation();
    }
    public List<RaycastHit2D> CreateRaycast(System.Int32 mask)
    {
        List<RaycastHit2D> rayCasts = new List<RaycastHit2D>();
        foreach (var collider in itemColliders)
        {
            rayCasts.Add(Physics2D.Raycast(collider.bounds.center, new Vector2(0.0f, 0.0f), 0, mask));
        }
        return rayCasts;
     }
    public virtual void CreateCareRayñast()
    {
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                if (careHits.Where(e => e.raycastHit.collider != null && e.raycastHit.collider.name == hit.collider.name).Count() == 0)
                {
                    hit.collider.GetComponent<SpriteRenderer>().color = Color.red;
                    careHits.Add(new RaycastStructure(hit));//îáúåêòû
                }
            }
        }
        foreach (var hit in hitsForBackpack)
        {
            if (hit.collider != null)
            {
                if (careHitsForBackpack.Where(e => e.raycastHit.collider != null && e.raycastHit.collider.name == hit.collider.name).Count() == 0)
                {
                    careHitsForBackpack.Add(new RaycastStructure(hit));//îáúåêòû
                }
            }
        }
    }
    public virtual void ClearCareRaycast()
    {
        foreach (var Carehit in careHits)
        {
            if ((hits.Where(e => e.collider != null && e.collider.name == Carehit.raycastHit.collider.name).Count() == 0) || hits.Where(e => e.collider == null).Count() == colliderCount)
            {
                Carehit.raycastHit.collider.GetComponent<SpriteRenderer>().color = imageColor;
                Carehit.isDeleted = true;
            }
        }

        careHits.RemoveAll(e => e.isDeleted == true);

        foreach (var Carehit in careHitsForBackpack)
        {
            if ((hits.Where(e => e.collider != null && e.collider.name == Carehit.raycastHit.collider.name).Count() == 0) || hits.Where(e => e.collider == null).Count() == colliderCount)
            {
                Carehit.isDeleted = true;
            }
        }

        careHitsForBackpack.RemoveAll(e => e.isDeleted == true);
    }
    public virtual void RaycastEvent()
    {
        hits.Clear();
        hitsForBackpack.Clear();
        hitsForBackpack = CreateRaycast(128);
        hits = CreateRaycast(256);

        ClearCareRaycast();
        CreateCareRayñast();
    }
    public virtual void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        RaycastEvent();
    }
    public virtual Vector2 calculateOffset(List<BoxCollider2D> itemColliders)
    {
        var maxY = itemColliders[0].bounds.center.y;
        Vector2 offset = itemColliders[0].offset;

        for (int i = 1; i < itemColliders.Count; i++)
        {
            if (itemColliders[i].bounds.center.y >= maxY)
            {
                maxY = itemColliders[i].bounds.center.y;
            }
        }

        var newListItemColiders = itemColliders.Where(e => Mathf.Round(e.bounds.center.y * 10.0f) * 0.1f == Mathf.Round(maxY * 10.0f) * 0.1f).ToList();
        var minX = newListItemColiders[0].bounds.center.x;
        foreach (var itemColider in newListItemColiders)
        {
            if (Mathf.Round(itemColider.bounds.center.y * 10.0f) * 0.1f == Mathf.Round(maxY * 10.0f) * 0.1f)
            {
                if (itemColider.bounds.center.x <= minX)
                {
                    minX = itemColider.bounds.center.x;
                    offset = itemColider.offset; 
                }
            }
        }

        if (rectTransform.eulerAngles.z == 90f)
        {

            var i = offset.x;
            offset.x = offset.y;
            offset.y = i;

            offset.y = -offset.y;

        }
        if (rectTransform.eulerAngles.z == 270f)
        {

            var i = offset.x;
            offset.x = offset.y;
            offset.y = i;

            offset.x = -offset.x;

        }
        if (rectTransform.eulerAngles.z == 0)
        {
            offset = -offset;
        }

        return offset;
    }
    public virtual bool CorrectEndPoint()
    {

        if (careHits.Count() == colliderCount && careHits.Where(e => e.raycastHit.collider.GetComponent<Cell>().nestedObject != null).Count() == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void CorrectPosition()
    {
        if (hits.Where(e => e.collider == null).Count() == 0)
        {
            var maxY = careHitsForBackpack[0].raycastHit.collider.transform.localPosition.y;
            Vector2 colliderPos = careHitsForBackpack[0].raycastHit.collider.transform.localPosition;

            for (int i = 1; i < careHitsForBackpack.Count; i++)
            {
                if (careHitsForBackpack[i].raycastHit.collider.transform.localPosition.y >= maxY)
                {
                    maxY = careHitsForBackpack[i].raycastHit.collider.transform.localPosition.y;
                }
            }
            var newListCareHits = careHitsForBackpack.Where(e => e.raycastHit.collider.transform.localPosition.y == maxY).ToList();
            var minX = newListCareHits[0].raycastHit.collider.transform.localPosition.x;
            foreach (var careHit in newListCareHits)//.Where(e => e.raycastHit.collider.transform.localPosition.y == maxY))
            {
                if (careHit.raycastHit.collider.transform.localPosition.y == maxY)// && careHit.raycastHit.collider.transform.localPosition.x <= minX
                {
                    if (careHit.raycastHit.collider.transform.localPosition.x <= minX)// && careHit.raycastHit.collider.transform.localPosition.x <= minX
                    {
                        minX = careHit.raycastHit.collider.transform.localPosition.x;
                        colliderPos = careHit.raycastHit.collider.transform.localPosition;

                    }
                }
            }
            var offset = calculateOffset(itemColliders);
            rectTransform.localPosition = offset + colliderPos;
            needToDynamic = false;
            foreach (var careHit in careHitsForBackpack)
            {
                careHit.raycastHit.collider.GetComponent<SpriteRenderer>().color = imageColor;
            }
        }
    }
    public void ChangeColorToDefault()
    {
        foreach (var Carehit in careHits)
        {
            Carehit.raycastHit.collider.GetComponent<SpriteRenderer>().color = imageColor;
        }
    }
    public void SetNestedObject()
    {
        foreach (var Carehit in careHits)
        {
            Carehit.raycastHit.collider.GetComponent<Cell>().nestedObject = gameObject;
        }
    }
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        needToRotate = false;
        image.color = imageColor;

        if(CorrectEndPoint())
        {
            CorrectPosition();
            SetNestedObject();
        }
        else
        {
            needToDynamic = true;
            //gameObject.transform.SetParent(backpack.transform);
        }
        ChangeColorToDefault();


        careHits.Clear();

    }



    public virtual void ShowDiscriptionActivation()
    {
        Debug.Log("Îïèñàíèå: èäè íà õóé!");
    }
    public virtual void Activation()
    {
        Debug.Log("èäè íà õóé!");
    }

}
