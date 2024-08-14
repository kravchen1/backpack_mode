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

public abstract class Item : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private string Name;
    private int colliderCount;
    private RectTransform rectTransform;
    private float startRectTransformZ;

    private UnityEngine.UI.Image image;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Color imageColor;
    private bool needToRotate;
    private bool needToDynamic = false;
    private bool needToRotateToStartRotation = false;

    //лучи
    private List<Collider2D> itemColliders = new List<Collider2D>();
    private List<RaycastHit2D> hits = new List<RaycastHit2D>();
    private List<RaycastStructure> careHits = new List<RaycastStructure>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //не лучи
    private Transform bagTransform;
    private Collider2D[] collidersArray;

    private Rigidbody2D rb;


    private bool firstTap = true; //костыль, но сори


    void initializationItemColliders()
    {
        collidersArray = rectTransform.GetComponents<Collider2D>();
        itemColliders.Clear();
        for (int i = 0; i < collidersArray.Count(); i++)
        {
            //Debug.Log(collidersArray[i].bounds.center.ToString());
            itemColliders.Add(collidersArray[i]);
        }
        colliderCount = collidersArray.Count();
    }
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rectTransform = GetComponent<RectTransform>();
        startRectTransformZ = rectTransform.eulerAngles.z;// rectTransform.rotation.z;
        image = GetComponent<UnityEngine.UI.Image>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        imageColor = image.color;
        needToRotate = false;

        initializationItemColliders();

    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && needToRotate)
        {
            Vector3 newRotation = new Vector3(0, 0, 90);
            rectTransform.Rotate(newRotation);
            //rectTransform.SyncTransforms()
            Physics2D.SyncTransforms();
            RaycastEvent();
        }


        if (needToDynamic)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            rb.bodyType = RigidbodyType2D.Static;
        }

        if(needToRotateToStartRotation)
        {
            if(rectTransform.eulerAngles.z >= - 5  && rectTransform.eulerAngles.z <= 5)
            //if(rectTransform.eulerAngles.z == startRectTransformZ)
            {
                //rectTransform.Rotate(0, 0, 1);
                //Debug.Log(rectTransform.rotation.z);
                // Debug.Log(startRectTransformZ);
                
                needToRotateToStartRotation = false;
                rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

            }
            else
            {
                rectTransform.Rotate(0, 0, 500 * Time.deltaTime);
                Debug.Log(rectTransform.rotation.z);
                Debug.Log(startRectTransformZ);
            }

        }


    }

    void createRaycast()
    {
        foreach (var collider in itemColliders)
        {
            hits.Add(Physics2D.Raycast(collider.bounds.center, -Vector2.up, 0, 128));
            //Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask)
        }
    }

    void buildCareRayCast()
    {
        foreach (var hit in hits)
        {

            if (hit.collider != null)
            {
                if (careHits.Where(e => e.raycastHit.collider != null && e.raycastHit.collider.name == hit.collider.name).Count() == 0)
                {
                    hit.collider.GetComponent<UnityEngine.UI.Image>().color = Color.red;
                    careHits.Add(new RaycastStructure(hit));//объекты
                    bagTransform = hit.transform.parent.transform;
                }
            }
        }
    }

    public void RaycastEvent()
    {
        hits.Clear();
        createRaycast();
        foreach (var Carehit in careHits)
        {
            if ((hits.Where(e => e.collider != null && e.collider.name == Carehit.raycastHit.collider.name).Count() == 0) || hits.Where(e => e.collider == null).Count() == colliderCount)
            {
                Carehit.raycastHit.collider.GetComponent<UnityEngine.UI.Image>().color = imageColor;
                Carehit.isDeleted = true;
            }
        }

        careHits.RemoveAll(e => e.isDeleted == true);

        buildCareRayCast();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if(firstTap)
        {
            firstTap = false;
            rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }

        needToRotate = true;
        if (needToDynamic)
        {
            needToRotateToStartRotation = true;
        }
        else
        {
           
        }
        needToDynamic = false;
        
        //image.color. = 0.5f;
        //image.raycastTarget = false;
        //canvasGroup.blocksRaycasts = false;
        //var test = eventData.pointerDrag.transform.localPosition - eventData.pointerDrag.transform.GetChild(0).localPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        /*
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
        */
        RaycastEvent();
    }

    public Vector2 calculateOffset(List<Collider2D> itemColliders)
    {
        //var minX = itemColliders[0].bounds.center.x;
        //var maxY = itemColliders[0].bounds.center.y;
       //Vector2 offset = itemColliders[0].offset;

        //for (int i = 1; i < itemColliders.Count; i++)
        //{
        //    if (itemColliders[i].bounds.center.x <= minX && itemColliders[i].bounds.center.y >= maxY)
        //    {
        //        minX = itemColliders[i].bounds.center.x;
        //        maxY = itemColliders[i].bounds.center.y;
        //        offset = itemColliders[i].offset;
        //    }
        //}

        var maxY = itemColliders[0].bounds.center.y;
        Vector2 offset = itemColliders[0].offset;

        for (int i = 1; i < itemColliders.Count; i++)
        {
            Debug.Log("0) " + itemColliders[i].bounds.center.y);
            Debug.Log("0)Round " + Mathf.Round(itemColliders[i].bounds.center.y * 10.0f) * 0.1f );
            if (itemColliders[i].bounds.center.y >= maxY)
            {
                maxY = itemColliders[i].bounds.center.y;
            }
        }
        var newListItemColiders = itemColliders.Where(e => Mathf.Round(e.bounds.center.y * 10.0f) * 0.1f == Mathf.Round(maxY * 10.0f) * 0.1f).ToList();
        var minX = newListItemColiders[0].bounds.center.x;
        //foreach (var careHit in newListCareHits)//.Where(e => e.raycastHit.collider.transform.localPosition.y == maxY))
        // {
        //Debug.Log(careHit.raycastHit.collider.transform.localPosition);
        //}
        //Debug.Log(minX);
        //Debug.Log(maxY);
        foreach (var itemColider in newListItemColiders)//.Where(e => e.raycastHit.collider.transform.localPosition.y == maxY))
        {
            Debug.Log("1) " + itemColider.bounds.center.x);
            if (Mathf.Round(itemColider.bounds.center.y * 10.0f) * 0.1f == Mathf.Round(maxY * 10.0f) * 0.1f)// && careHit.raycastHit.collider.transform.localPosition.x <= minX
            {
                if (itemColider.bounds.center.x <= minX)// && careHit.raycastHit.collider.transform.localPosition.x <= minX
                {
                    minX = itemColider.bounds.center.x;
                    offset = itemColider.offset;
                    //Debug.Log("-------");
                }
            }
        }
        //Physics2D.SyncTransforms();
        //Debug.Log(minX.ToString() + ';' + maxY.ToString())




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
        Debug.Log(rectTransform.eulerAngles.z);
        return offset;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        needToRotate = false;
        image.color = imageColor;
        image.raycastTarget = true;
        canvasGroup.blocksRaycasts = true;
        //var tre = careHits.AsQueryable().Distinct().Count();
        // var tre2 = careHits.AsQueryable().Distinct();

        if (careHits.Count() == colliderCount)
        {
            if (hits.Where(e => e.collider == null).Count() == 0)
            {
                var maxY = careHits[0].raycastHit.collider.transform.localPosition.y;
                Vector2 colliderPos = careHits[0].raycastHit.collider.transform.localPosition;

                for (int i = 1; i < careHits.Count; i++)
                {
                    if (careHits[i].raycastHit.collider.transform.localPosition.y >= maxY)
                    {
                        maxY = careHits[i].raycastHit.collider.transform.localPosition.y;
                    }
                }
                var newListCareHits = careHits.Where(e => e.raycastHit.collider.transform.localPosition.y == maxY).ToList();
                var minX = newListCareHits[0].raycastHit.collider.transform.localPosition.x;
                //foreach (var careHit in newListCareHits)//.Where(e => e.raycastHit.collider.transform.localPosition.y == maxY))
               // {
                    //Debug.Log(careHit.raycastHit.collider.transform.localPosition);
                //}
                //Debug.Log(minX);
                //Debug.Log(maxY);
                foreach (var careHit in newListCareHits)//.Where(e => e.raycastHit.collider.transform.localPosition.y == maxY))
                {
                    Debug.Log(careHit.raycastHit.collider.transform.localPosition.x);
                    if (careHit.raycastHit.collider.transform.localPosition.y == maxY)// && careHit.raycastHit.collider.transform.localPosition.x <= minX
                    {
                        if (careHit.raycastHit.collider.transform.localPosition.x <= minX)// && careHit.raycastHit.collider.transform.localPosition.x <= minX
                        {
                            minX = careHit.raycastHit.collider.transform.localPosition.x;
                            colliderPos = careHit.raycastHit.collider.transform.localPosition;
                            //Debug.Log("-------");
                        }
                    }
                }
                //Debug.Log(colliderPos);
                //for (int i = 1; i < careHits.Count; i++)
                //{
                //    if (careHits[i].raycastHit.collider.transform.localPosition.x <= minX && careHits[i].raycastHit.collider.transform.localPosition.y >= maxY)
                //    {
                //        minX = careHits[i].raycastHit.collider.transform.localPosition.x;
                //        maxY = careHits[i].raycastHit.collider.transform.localPosition.y;
                //        colliderPos = careHits[i].raycastHit.collider.transform.localPosition;
                //    }
                //}

                //for (int i = 1; i < careHits.Count; i++)
                //{
                //    if (careHits[i].raycastHit.collider.transform.localPosition.x <= minX && careHits[i].raycastHit.collider.transform.localPosition.y >= maxY)
                //    {
                //        minX = careHits[i].raycastHit.collider.transform.localPosition.x;
                //        maxY = careHits[i].raycastHit.collider.transform.localPosition.y;
                //        colliderPos = careHits[i].raycastHit.collider.transform.localPosition;
                //    }
                //}
                rectTransform.SetParent(bagTransform);
                var offset = calculateOffset(itemColliders);
                Debug.Log(rectTransform.localPosition);
                Debug.Log(colliderPos);
                Debug.Log(offset);
                rectTransform.localPosition = offset + colliderPos;
                needToDynamic = false;
                Debug.Log(rectTransform.localPosition);

                //Debug.Log(calculateOffset(itemColliders));
                foreach (var careHit in careHits)
                {
                    careHit.raycastHit.collider.GetComponent<UnityEngine.UI.Image>().color = imageColor;
                }
            }
            
        }
        else
        {
            needToDynamic = true;
        }
        careHits.Clear();

    }

}
