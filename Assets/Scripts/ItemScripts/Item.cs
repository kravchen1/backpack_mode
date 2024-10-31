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
using static UnityEditor.Progress;
using static UnityEngine.UI.Image;
using UnityEditor.SceneManagement;


public abstract class Item : MonoBehaviour, IBeginDragHandler  , IDragHandler  , IEndDragHandler , IEventSystemHandler, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler , IPointerExitHandler    
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


    public float itemCost;

    //лучи
    public List<BoxCollider2D> itemColliders = new List<BoxCollider2D>();
    public List<RaycastHit2D> hits = new List<RaycastHit2D>();
    public List<RaycastHit2D> hitsForBackpack= new List<RaycastHit2D>();
    public List<RaycastStructure> careHits = new List<RaycastStructure>();
    public List<RaycastStructure> careHitsForBackpack = new List<RaycastStructure>();
    public List<RaycastHit2D> hitSellChest = new List<RaycastHit2D>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //не лучи
    public Transform bagTransform;
    public BoxCollider2D[] collidersArray;

    public Rigidbody2D rb;

    public Vector3 lastItemPosition;

    public RectTransform rectTransform;

    public bool firstTap = true; //костыль, но сори
    public bool needToRotate;
    public bool needToDynamic = false;
    public bool needToRotateToStartRotation = false;

    protected PlayerBackpackBattle Player;
    protected PlayerBackpackBattle Enemy;
    public Animator animator;
    Animator sellChestAnimator;
    public bool Impulse = false;

    public List<GameObject> stars;
    public Sprite emptyStar;
    public Sprite fillStar;

    public ShopItem shopItem;

    public bool isSellChest = false;



    

    //void SetItemCost()
    //{
    //    if (gameObject.name.ToUpper().Contains("BAG"))
    //        itemCost = 4;
    //    if (gameObject.name.ToUpper().Contains("SWORD") && gameObject.name.ToUpper().Contains("CURSE"))
    //    {
    //        itemCost = 7;
    //    }
    //    if (gameObject.name.ToUpper().Contains("SWORD"))
    //    {
    //        itemCost = 3;
    //    }
    //    if (gameObject.name.ToUpper().Contains("SWORD"))
    //    {
    //        itemCost = 3;
    //    }
    //}

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
            //animator.enabled = false;
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

    // Метод, который будет вызываться при нажатии на объект
    public void OnPointerDown(PointerEventData eventData)
    {
        // Проверяем, был ли нажат левый кнопка мыши
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Объект был нажат!");
            animator.Play("ItemClick");
            // Здесь можно добавить код, который будет выполняться при нажатии
        }
    }

    // Метод, который будет вызываться при отпускания объекта без перетягивания
    public void OnPointerClick(PointerEventData eventData)
    {
        // Проверяем, был ли клик левой кнопкой мыши
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //RaycastEvent();
            Debug.Log("Объект был кликнут!");
            animator.Play("ItemClickOff");
            // Здесь можно добавить код, который будет выполняться при клике
        }
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
           // Debug.Log(Input.GetAxis("Mouse X"));
            //Debug.Log(Input.GetAxis("Mouse Y"));
        }
    }
    public void RotationToStartRotation()
    {
       // Debug.Log(needToRotateToStartRotation);
       // Debug.Log(rectTransform.eulerAngles.z);
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
        if (SceneManager.GetActiveScene().name == "BackPackShop" || SceneManager.GetActiveScene().name == "BackpackView")
        {
            lastItemPosition = gameObject.transform.position;
            if (GetComponent<ShopItem>() != null)
            {
                shopItem = GetComponent<ShopItem>();
                if (shopItem.CanBuy(GetComponent<Item>()))
                {
                    TapFirst();
                    TapRotate();
                    DeleteNestedObject();
                    //gameObject.transform.SetParent(GameObject.Find("backpack").transform);
                    OnPointerExit(eventData);
                    ChangeShowStars(true);
                    canShowDescription = false;
                }
                else
                {
                    eventData.pointerDrag = null;
                }
            }
            else
            {
                //List<GameObject> list = new List<GameObject>();
                //if (ItemInGameObject("Shop", list) && shopData.CanBuy(gameObject.GetComponent<Item>()))
                TapFirst();
                TapRotate();
                DeleteNestedObject();
                //gameObject.transform.SetParent(GameObject.Find("backpack").transform);
                OnPointerExit(eventData);
                ChangeShowStars(true);
                canShowDescription = false;
            }
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

    public void FillnestedObjectStarsStars(System.Int32 mask, String tag)
    {
        RaycastHit2D raycast;
        foreach (var star in stars)
        {
            //Debug.Log(gameObject.name + star.GetComponent<RectTransform>().GetComponent<BoxCollider2D>().bounds.center);
            raycast = Physics2D.Raycast(star.GetComponent<RectTransform>().GetComponent<BoxCollider2D>().bounds.center, new Vector2(0.0f, 0.0f), 0, mask);
            if(raycast.collider != null && raycast.collider.gameObject.tag == tag/*"gloves"*/)//6787
            {
                star.GetComponent<Cell>().nestedObject = raycast.collider.gameObject;
                star.GetComponent<SpriteRenderer>().sprite = fillStar;
            }
            else
            {
                star.GetComponent<Cell>().nestedObject = null;
                star.GetComponent<SpriteRenderer>().sprite = emptyStar;
            }
        }
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
                Carehit.raycastHit.collider.GetComponent<SpriteRenderer>().color = Color.black;
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
        hitSellChest.Clear();
        hitSellChest = CreateRaycast(32768);
        ClearCareRaycast();
        CreateCareRayсast();
    }

    public virtual void SellChest()
    {
        if (hitSellChest.Any(e => e.collider != null && e.collider.name == "SellChest") && gameObject.GetComponent<ShopItem>() == null)
        {
            foreach (var hit in hitSellChest.Where(e => e.collider != null && e.collider.name == "SellChest"))
            {
                if (!isSellChest)
                {
                    sellChestAnimator = hit.collider.gameObject.GetComponent<Animator>();
                    sellChestAnimator.Play("Metal Chest Opening",0, 2);
                    isSellChest = true;
                }
            }
        }
        else if (isSellChest)
        {
            sellChestAnimator.Play("Metal Chest Closed");
            isSellChest = false;
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (SceneManager.GetActiveScene().name == "BackPackShop" || SceneManager.GetActiveScene().name == "BackpackView")
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            RaycastEvent();
            SellChest();
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

    private int ExtendedCorrectEndPoint()
    {
        if (careHits.Count() == colliderCount && careHits.Where(e => e.raycastHit.collider.GetComponent<Cell>().nestedObject != null).Count() == 0)
        {
            return 1; //точки совпадают и нет предметов
        } 
        else if (careHits.Count() == colliderCount && careHits.Where(e => e.raycastHit.collider.GetComponent<Cell>().nestedObject != null).Count() != 0)
        {
            return 2; //точки совпадают, но есть предметы
        }
        else
        {
            return 3; //точки не совпадают
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
                careHit.raycastHit.collider.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
    }
    public void ChangeColorToDefault()
    {
        foreach (var Carehit in careHits)
        {
            Carehit.raycastHit.collider.GetComponent<SpriteRenderer>().color = Color.black;
        }
    }
    public void SetNestedObject()
    {
        foreach (var Carehit in careHits)
        {
            Carehit.raycastHit.collider.GetComponent<Cell>().nestedObject = gameObject;
        }
    }

    public virtual void SellItem()
    {
        var listCharacterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None);
        var characterStats = listCharacterStats[0];
        characterStats.playerCoins = characterStats.playerCoins + (float)Math.Ceiling(itemCost/2);
        characterStats.coinsText.text = characterStats.playerCoins.ToString();
        sellChestAnimator.Play("Metal Chest Closed");
        Destroy(gameObject);
    }
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (SceneManager.GetActiveScene().name == "BackPackShop" || SceneManager.GetActiveScene().name == "BackpackView")
        {
            needToRotate = false;
            image.color = imageColor;
            if (shopItem != null)
            {
                shopItem.BuyItem(gameObject.GetComponent<Item>());
            }

            if(isSellChest)
            {
                SellItem();
            }

            //if (CorrectEndPoint())
            //{
            //    gameObject.transform.SetParent(GameObject.Find("backpack").transform);
            //    CorrectPosition();
            //    SetNestedObject();
            //}
            //else
            //{
                
            //    gameObject.transform.SetParent(GameObject.Find("Storage").transform);
            //    needToDynamic = true;
            //    Impulse = true;
            //    MoveObjectOnEndDrag();
            //    //gameObject.transform.SetParent(backpack.transform);
            //}


            switch(ExtendedCorrectEndPoint())
            {
                case 1:
                    gameObject.transform.SetParent(GameObject.Find("backpack").transform);
                    CorrectPosition();
                    SetNestedObject();
                    break;
                case 2:
                    foreach(var Carehit in careHits.Where(e => e.raycastHit.collider.GetComponent<Cell>().nestedObject != null))
                    {
                        var nestedObjectItem = Carehit.raycastHit.collider.GetComponent<Cell>().nestedObject.GetComponent<Item>();
                        nestedObjectItem.MoveObjectOnEndDrag();
                        nestedObjectItem.DeleteNestedObject();
                        nestedObjectItem.needToDynamic = true;
                        nestedObjectItem.Impulse = true;
                    }
                    gameObject.transform.SetParent(GameObject.Find("backpack").transform);
                    CorrectPosition();
                    SetNestedObject();

                    break;
                case 3:
                    gameObject.transform.SetParent(GameObject.Find("Storage").transform);
                    needToDynamic = true;
                    Impulse = true;
                    MoveObjectOnEndDrag();
                    break;
            }

            ChangeColorToDefault();


            careHits.Clear();
            canShowDescription = true;
            OnPointerEnter(eventData);
        }
    }

    public void MoveObjectOnEndDrag()
    {
        //List<GameObject> list = new List<GameObject>();
        //if (ItemInGameObject("Storage", list))
        StartCoroutine(moveObject(GameObject.Find("Storage").transform.position));
        //else
        //    StartCoroutine(moveObject(lastItemPosition));
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
                    var starsDesctiprion = CanvasDescription.GetComponentInChildren<SpriteRenderer>();
                    if (starsDesctiprion != null)
                    {
                        starsDesctiprion.enabled = false;
                    }
                }
            }
        }
    }

    public void ChangeShowStars(bool show)
    {
        foreach (GameObject star in stars)
        {
            star.GetComponent<SpriteRenderer>().enabled = show;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeShowStars(true);
        if (eventData.pointerDrag == null)
        {
            Exit = false;
            //Debug.Log(Description.gameObject.name + "вошёл");
            StartCoroutine(ShowDescription());
        }   
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log(Description.gameObject.name + "вышел");
        Exit = true;
        ChangeShowStars(false);
       // Debug.Log("убрали курсор");
        if (canShowDescription && CanvasDescription != null)
        {
            CanvasDescription.enabled = false;
            var starsDesctiprion = CanvasDescription.GetComponentInChildren<SpriteRenderer>();
            if (starsDesctiprion != null)
            {
                starsDesctiprion.enabled = false;
            }
        }
    }
    public bool ObjectInBag()
    {
        List<GameObject> backpack = new List<GameObject>();
        GameObject.FindGameObjectsWithTag("backpack", backpack);

        var rectTransform = backpack[0].GetComponent<RectTransform>();

        if (gameObject.transform.localPosition.x > rectTransform.rect.max.x || gameObject.transform.localPosition.y > rectTransform.rect.max.y || gameObject.transform.localPosition.x < rectTransform.rect.min.x || gameObject.transform.localPosition.y < rectTransform.rect.min.y)
        {
            return false;
        }
        else
            return true;
    }
    public bool ItemInGameObject(string gameObjectName, List<GameObject> gameObjectList)
    {
        GameObject.FindGameObjectsWithTag(gameObjectName, gameObjectList);

        var rectTransform = gameObjectList[0].GetComponent<RectTransform>();

        if (gameObject.transform.localPosition.x > rectTransform.rect.max.x || gameObject.transform.localPosition.y > rectTransform.rect.max.y || gameObject.transform.localPosition.x < rectTransform.rect.min.x || gameObject.transform.localPosition.y < rectTransform.rect.min.y)
        {
            return false;
        }
        else
            return true;
    }

    public IEnumerator moveObject(Vector3 destination)
    {
        var origin = transform.position;
        //var destination = GameObject.Find("Storage").transform.position;
        //var destination = new Vector3(0,0,0);
        float totalMovementTime = 0.5f; //the amount of time you want the movement to take
        float currentMovementTime = 0f;//The amount of time that has passed
        while (Vector3.Distance(transform.position, destination) > 1)
        {
            currentMovementTime += Time.deltaTime;
            transform.position = Vector3.Lerp(origin, destination, currentMovementTime / totalMovementTime);
            yield return null;
        }
    }
    void OnCollisionEnter2D(Collision2D col)
    {
        //if (col.gameObject.tag == "InvisibleWalls")
        //    rb.AddForce(transform.right * 1, ForceMode2D.Impulse);
        //todo
    }


}
