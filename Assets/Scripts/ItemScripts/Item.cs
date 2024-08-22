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
using System.Collections;
using UnityEngine.SceneManagement;


public abstract class Item : MonoBehaviour, IBeginDragHandler  , IDragHandler  , IEndDragHandler , IEventSystemHandler     , IPointerEnterHandler , IPointerExitHandler    
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


    public Canvas Description;
    private Canvas CanvasDescription;
    private bool showCanvasBefore = false;
    protected bool canShowDescription = true;
    private bool Exit = false;

    //лучи
    public List<BoxCollider2D> itemColliders = new List<BoxCollider2D>();
    public List<RaycastHit2D> hits = new List<RaycastHit2D>();
    public List<RaycastHit2D> hitsForBackpack= new List<RaycastHit2D>();
    public List<RaycastStructure> careHits = new List<RaycastStructure>();
    public List<RaycastStructure> careHitsForBackpack = new List<RaycastStructure>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //не лучи
    public Transform bagTransform;
    public BoxCollider2D[] collidersArray;

    public Rigidbody2D rb;


    public RectTransform rectTransform;

    public bool firstTap = true; //костыль, но сори
    public bool needToRotate;
    public bool needToDynamic = false;
    public bool needToRotateToStartRotation = false;

    protected PlayerBackpackBattle Player;
    protected PlayerBackpackBattle Enemy;
    public Animator animator;


    public bool Impulse = false;



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
        if (GetComponent<Animator>() != null)
        {
            animator = GetComponent<Animator>();
            animator.enabled = false;
        }
        initializationItemColliders();

        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            if(gameObject.transform.parent.name == GameObject.Find("backpack").transform.name)
            {
                Player = GameObject.Find("Character").GetComponent<PlayerBackpackBattle>();
                Enemy = GameObject.Find("CharacterEnemy").GetComponent<PlayerBackpackBattle>();
            }

            if (gameObject.transform.parent.name == GameObject.Find("backpackEnemy").transform.name)
            {
                Player = GameObject.Find("CharacterEnemy").GetComponent<PlayerBackpackBattle>();
                Enemy = GameObject.Find("Character").GetComponent<PlayerBackpackBattle>();
            }
        }         
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
    public void OnImpulse()
    {
        if (Impulse)
        {
            Impulse = false;
            rb.mass = 0.1f;
            rb.AddForce(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")), ForceMode2D.Impulse);
            rb.AddTorque(15);
           // rb.AddRelativeForceX(10, ForceMode2D.Impulse);
            Debug.Log(Input.GetAxis("Mouse X"));
            Debug.Log(Input.GetAxis("Mouse Y"));
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
        if (SceneManager.GetActiveScene().name == "BackPackShop")
        {
            TapFirst();
            TapRotate();
            DeleteNestedObject();
            gameObject.transform.SetParent(GameObject.Find("backpack").transform);
            OnPointerExit(eventData);
            canShowDescription = false;
        }
    }
    void Update()
    {
        Rotate();
        SwitchDynamicStatic();
        OnImpulse();
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
    public virtual void CreateCareRayсast()
    {
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                if (careHits.Where(e => e.raycastHit.collider != null && e.raycastHit.collider.name == hit.collider.name).Count() == 0)
                {
                    hit.collider.GetComponent<SpriteRenderer>().color = Color.red;
                    careHits.Add(new RaycastStructure(hit));//объекты
                }
            }
        }
        foreach (var hit in hitsForBackpack)
        {
            if (hit.collider != null)
            {
                if (careHitsForBackpack.Where(e => e.raycastHit.collider != null && e.raycastHit.collider.name == hit.collider.name).Count() == 0)
                {
                    careHitsForBackpack.Add(new RaycastStructure(hit));//объекты
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
        CreateCareRayсast();
    }
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (SceneManager.GetActiveScene().name == "BackPackShop")
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            RaycastEvent();
        }
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
        if (SceneManager.GetActiveScene().name == "BackPackShop")
        {
            needToRotate = false;
            image.color = imageColor;

            if (CorrectEndPoint())
            {
                CorrectPosition();
                SetNestedObject();
            }
            else
            {
                needToDynamic = true;
                Impulse = true;
                //gameObject.transform.SetParent(backpack.transform);

            }
            ChangeColorToDefault();


            careHits.Clear();
            canShowDescription = true;
            OnPointerEnter(eventData);
        }
    }
    public virtual void ShowDiscriptionActivation()
    {
        Debug.Log("Описание: описание!");
    }
    public virtual void Activation()
    {
        Debug.Log("Активация " + this.name);
    }
    IEnumerator ShowDescription()
    {
        yield return new WaitForSeconds(.25f);
        if (!Exit)
        {
            if (canShowDescription)
            {
                if (!showCanvasBefore)
                {
                    showCanvasBefore = true;
                    CanvasDescription = Instantiate(Description, GameObject.Find("Canvas").GetComponent<RectTransform>().transform);
                    //showCanvas.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
                }
                else
                {
                    CanvasDescription.enabled = true;
                }
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
        {
            Exit = false;
            Debug.Log(Description.gameObject.name + "вошёл");
            StartCoroutine(ShowDescription());
        }
           
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log(Description.gameObject.name + "вышел");
        Exit = true;
         Debug.Log("убрали курсор");
        if (canShowDescription && CanvasDescription != null)
        {
            CanvasDescription.enabled = false;
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        //if (col.gameObject.tag == "InvisibleWalls")
        //    rb.AddForce(transform.right * 1, ForceMode2D.Impulse);
        //todo
    }


}
