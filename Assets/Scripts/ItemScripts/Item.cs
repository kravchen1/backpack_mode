using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using Assets.Scripts.ItemScripts;


public class HitsStructure
{
    public List<RaycastHit2D> hits = new List<RaycastHit2D>();
    public HitsStructure(List<RaycastHit2D> hits)
    {
        this.hits = hits;
    }
}
public abstract class Item : MonoBehaviour
{
    [HideInInspector] public int speedRotation = 500;
    [HideInInspector] public string Name;
    [HideInInspector] public int colliderCount;

    [HideInInspector] public float startRectTransformZ;

    [HideInInspector] public SpriteRenderer image;
    [HideInInspector] public Canvas canvas;

    //protected CanvasGroup canvasGroup;
    [HideInInspector] public Color imageColor;
    [HideInInspector] public string prefabOriginalName;


    public GameObject Description;
    public GameObject DescriptionLogCharacter;
    public GameObject DescriptionLogEnemy;

    public Sprite originalSprite;
    public GameObject prefabAnimationAttack;

    [HideInInspector] public GameObject CanvasDescription;


    [HideInInspector] private bool showCanvasBefore = false;
    [HideInInspector] protected bool canShowDescription = true;
    [HideInInspector] public bool Exit = false;
    //public float rbMass = 0.1f;


    public int itemCost;

    //����
    [HideInInspector] public List<BoxCollider2D> itemColliders = new List<BoxCollider2D>();
    [HideInInspector] public List<HitsStructure> hits = new List<HitsStructure>();


    [HideInInspector] public List<RaycastHit2D> hitsForBackpack = new List<RaycastHit2D>();
    [HideInInspector] public List<RaycastHit2D> hitsForNotShopZone = new List<RaycastHit2D>();
    [HideInInspector] public List<RaycastStructure> careHits = new List<RaycastStructure>();
    [HideInInspector] public List<RaycastStructure> careHitsForBackpack = new List<RaycastStructure>();
    [HideInInspector] public List<RaycastHit2D> hitSellChest = new List<RaycastHit2D>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //�� ����
    [HideInInspector] public Transform bagTransform;
    [HideInInspector] public BoxCollider2D[] collidersArray;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public PolygonCollider2D collider;

    [HideInInspector] public Vector3 lastItemPosition;

    [HideInInspector] public RectTransform rectTransform;

    [HideInInspector] public bool firstTap = true; //�������, �� ����
    [HideInInspector] public bool needToRotate;
    [HideInInspector] public bool needToDynamic = false;
    [HideInInspector] public bool needToRotateToStartRotation = false;

    [HideInInspector] protected PlayerBackpackBattle Player;
    [HideInInspector] protected PlayerBackpackBattle Enemy;
    [HideInInspector] protected GameObject placeForDescription;
    [HideInInspector] protected GameObject placeForLogDescription;


    [HideInInspector] public Animator animator;
    [HideInInspector] Animator sellChestAnimator;
    public AudioSource sellChestSound;
    [HideInInspector] public bool Impulse = false;

    public List<GameObject> stars;
    public Sprite emptyStar;
    public Sprite fillStar;

    [HideInInspector] public ShopItem shopItem;

    [HideInInspector] public bool isSellChest = false;

    [HideInInspector] private Camera mainCamera;
    [HideInInspector] public bool isDragging = false;
    [HideInInspector] public Vector3 offset;
    [HideInInspector] private int countClickRotate = 0, maxCountClickRotate = 100;
    private float timer_cooldownStatic = 12.5f;
    protected float timerStatic = 12.5f;
    protected bool timerStatic_locked_out = true;

    [HideInInspector] public OtherItemMusicEffects itemMusicEffects;
    public String originalName;


    protected float timer_cooldownStart = 1f;
    protected bool timer_locked_outStart = true;
    [HideInInspector] protected bool isEat = false;

    void Awake()
    {
        itemMusicEffects = GetComponent<OtherItemMusicEffects>();
        Initialization();
    }

    private void Start()
    {
        //FillnestedObjectStarsStars(256);
    }

    public void Initialization()
    {
        shopItem = GetComponent<ShopItem>();
        rb = GetComponent<Rigidbody2D>();
        rectTransform = GetComponent<RectTransform>();
        startRectTransformZ = rectTransform.eulerAngles.z;
        image = GetComponent<SpriteRenderer>();
        canvas = GetComponentInParent<Canvas>();
        imageColor = GetComponent<SpriteRenderer>().color;
        needToRotate = false;
        collider = GetComponent<PolygonCollider2D>();
        if (GetComponent<Animator>() != null)
        {
            animator = GetComponent<Animator>();
            //animator.enabled = false;
        }
        mainCamera = Camera.main;
        initializationItemColliders();

        if (SceneManager.GetActiveScene().name == "BackPackShop")
        {
            if (gameObject.transform.parent.name != GameObject.FindGameObjectWithTag("Shop").transform.name)
                placeForDescription = GameObject.FindWithTag("DescriptionPlace");
            else
                placeForDescription = GameObject.FindWithTag("DescriptionPlaceEnemy");
        }
        else if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            placeForLogDescription = GameObject.FindGameObjectWithTag("BattleLogContent");
            if (gameObject.transform.parent.name == GameObject.Find("backpack").transform.name)
            {
                placeForDescription = GameObject.FindWithTag("DescriptionPlace");
                Player = GameObject.Find("Character").GetComponent<PlayerBackpackBattle>();
                Enemy = GameObject.Find("CharacterEnemy").GetComponent<PlayerBackpackBattle>();
            }

            if (gameObject.transform.parent.name == GameObject.Find("backpackEnemy").transform.name)
            {
                placeForDescription = GameObject.FindWithTag("DescriptionPlaceEnemy");
                Player = GameObject.Find("CharacterEnemy").GetComponent<PlayerBackpackBattle>();
                Enemy = GameObject.Find("Character").GetComponent<PlayerBackpackBattle>();
            }
        }
        else
        {
            FindPlaceForDescription();
        }
    }

    void FindPlaceForDescription()
    {
        if (gameObject.transform.parent.name == GameObject.FindGameObjectWithTag("backpack").transform.name)
            placeForDescription = GameObject.FindWithTag("DescriptionPlace");
        else
            placeForDescription = GameObject.FindWithTag("DescriptionPlaceEnemy");
    }

    void initializationItemColliders()
    {
        collidersArray = gameObject.GetComponentsInChildren<BoxCollider2D>();
        itemColliders.Clear();
        for (int i = 0; i < collidersArray.Count(); i++)
        {
            if (!collidersArray[i].name.Contains("Star"))
                itemColliders.Add(collidersArray[i]);
        }
        colliderCount = itemColliders.Count();
    }

    private Vector3 shopItemStartPosition;
    public virtual void OnMouseDown()
    {
        shopItem = GetComponent<ShopItem>();
        //if (SceneManager.GetActiveScene().name == "BackPackShop" || SceneManager.GetActiveScene().name == "BackpackView")
        if (SceneManager.GetActiveScene().name != "BackPackBattle")
        {
            if (shopItem != null)//покупка
            {
                if (shopItem.CanBuy(GetComponent<Item>()))
                {
                    DragManager.isDragging = true;
                    itemMusicEffects.OnItemUp();
                    if (animator != null && !isEat) animator.Play("ItemClick");
                    DeletenestedObjectStars();
                    IgnoreCollisionObject(true);
                    image.sortingOrder = 4;
                    lastItemPosition = gameObject.transform.position;
                    needToDynamic = true;
                    //TapFirst();
                    TapRotate();
                    DeleteNestedObject(gameObject.transform.parent.tag);
                    //gameObject.transform.SetParent(GameObject.Find("backpack").transform);
                    ChangeShowStars(true);
                    canShowDescription = false;

                    // Начинаем перетаскивание
                    isDragging = true;
                    // Вычисляем смещение между курсором и объектом

                    offset = transform.position - GetMouseWorldPosition();
                    Debug.Log(offset);
                }
                else
                {

                }
            }
            else
            {
                DragManager.isDragging = true;
                itemMusicEffects.OnItemUp();
                if (animator != null && !isEat) animator.Play("ItemClick");
                DeletenestedObjectStars();
                IgnoreCollisionObject(true);
                image.sortingOrder = 4;
                lastItemPosition = gameObject.transform.position;
                needToDynamic = true;
                //List<GameObject> list = new List<GameObject>();
                //if (ItemInGameObject("Shop", list) && shopData.CanBuy(gameObject.GetComponent<Item>()))
                //TapFirst();
                TapRotate();
                DeleteNestedObject(gameObject.transform.parent.tag);
                //gameObject.transform.SetParent(GameObject.Find("backpack").transform);
                ChangeShowStars(true);
                canShowDescription = false;


                // Начинаем перетаскивание
                isDragging = true;
                // Вычисляем смещение между курсором и объектом
                offset = transform.position - GetMouseWorldPosition();
            }
        }
    }

    public virtual void OnMouseUp()
    {
        if (isDragging)
        {
            DragManager.isDragging = false;
            //if (SceneManager.GetActiveScene().name == "BackPackShop") if (animator != null) animator.Play("ItemAiming");
            if (GetComponent<AnimationStart>() != null)
            {
                GetComponent<AnimationStart>().Play();
            }
            IgnoreCollisionObject(false);
            if (SceneManager.GetActiveScene().name != "BackPackBattle") if (animator != null && !isEat) animator.Play("ItemClickOff");
        }


        //if (SceneManager.GetActiveScene().name == "BackPackShop" || SceneManager.GetActiveScene().name == "BackpackView")
        if (SceneManager.GetActiveScene().name != "BackPackBattle")
        {
            needToRotate = false;
            image.color = imageColor;
            if (shopItem != null)
            {
                if (hitsForNotShopZone.Any(e => e.collider != null))
                {

                    shopItem.BuyItem(gameObject.GetComponent<Item>());

                    ExtendedCorrectPosition();
                    ChangeColorToDefault();
                    careHits.Clear();
                    placeForDescription = GameObject.FindWithTag("DescriptionPlace");

                    needToRotateToStartRotation = false;
                    canShowDescription = true;
                }
                else
                {
                    if (shopItem.defaultPosition != transform.position)
                    {
                        StartCoroutine(ReturnToOriginalPosition(shopItem.defaultPosition));
                    }
                }
            }
            else
            {
                if (hitsForNotShopZone.Any(e => e.collider != null))
                {
                    ExtendedCorrectPosition();
                    ChangeColorToDefault();
                    careHits.Clear();
                    canShowDescription = true;

                    needToRotateToStartRotation = false;
                }
                else
                {
                    if (lastItemPosition != transform.position)
                    {
                        StartCoroutine(ReturnToOriginalPosition(lastItemPosition));
                    }
                }
            }
            if (isSellChest)
            {
                SellItem();
            }
        }

        // Заканчиваем перетаскивание
        isDragging = false;
        ClearCareRaycast(false);
        image.sortingOrder = 1;
        StartCoroutine(ShowDescription());
        //FillnestedObjectStarsStars(256);
        FindPlaceForDescription();
    }

    public System.Collections.IEnumerator ReturnToOriginalPosition(Vector3 originalPosition)
    {

        float time = 1f; // Время возвращения
        float elapsedTime = 0f;
        Vector3 startingPos = transform.position;
        Quaternion startingRot = transform.rotation;
        while (elapsedTime < time)
        {
            //Debug.Log(transform.position);
            transform.position = Vector3.Lerp(startingPos, originalPosition, (elapsedTime / time));
            //transform.rotation = Quaternion.Lerp(startingRot, Quaternion.identity, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition; // Убедитесь, что позиция точно установлена
    }
    public void defaultItemUpdate()
    {
        if (isDragging)
        {
            //Debug.Log(transform.localPosition);
            // Перемещаем объект в позицию курсора с учетом смещения   
            //if (SceneManager.GetActiveScene().name == "BackPackShop" || SceneManager.GetActiveScene().name == "BackpackView")
            if (SceneManager.GetActiveScene().name != "BackPackBattle")
            {
                transform.position = GetMouseWorldPosition() + offset;
                RaycastEvent();
                DeleteAllDescriptions();
                SellChest();
            }
        }
        else
        {
            //if (SceneManager.GetActiveScene().name == "BackPackShop")

        }
        Rotate();
        SwitchDynamicStatic();
        OnImpulse();
        RotationToStartRotation();
        CoolDownStatic();
    }
    public virtual void Update()
    {
        defaultItemUpdate();
    }


    public void ChangeShowStars(bool enabled)
    {
        foreach (var star in stars)
        {
            star.gameObject.GetComponent<SpriteRenderer>().enabled = enabled;//SetActive(enabled);
        }
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
    public int ExtendedCorrectEndPoint()
    {
        if (careHits.Count() == colliderCount && careHits.Where(e => e.raycastHit.collider.GetComponent<Cell>().nestedObject != null).Count() == 0)
        {
            return 1; //no swap item
        }
        else if (careHits.Count() == colliderCount && careHits.Where(e => e.raycastHit.collider.GetComponent<Cell>().nestedObject != null).Count() != 0)
        {
            return 2; //swap item
        }
        else
        {
            return 3; //don`t correct
        }
    }
    public void ExtendedCorrectPosition()
    {
        switch (ExtendedCorrectEndPoint())
        {
            case 1:
                gameObject.transform.SetParent(careHits[0].raycastHit.transform.parent.transform);

                CorrectPosition();
                SetNestedObject();
                rb.excludeLayers = (1 << 9) | (1 << 10);
                EffectPlaceCorrect();
                break;
            case 2:
                foreach (var Carehit in careHits.Where(e => e.raycastHit.collider.GetComponent<Cell>().nestedObject != null))
                {
                    var nestedObjectItem = Carehit.raycastHit.collider.GetComponent<Cell>().nestedObject.GetComponent<Item>();
                    //nestedObjectItem.MoveObjectOnEndDrag();
                    nestedObjectItem.EffectPlaceNoCorrect();
                    nestedObjectItem.DeleteNestedObject(gameObject.transform.parent.tag);
                    nestedObjectItem.needToDynamic = true;
                    timerStatic_locked_out = true;
                    timerStatic = timer_cooldownStatic;
                    //nestedObjectItem.Impulse = true;
                    //nestedObjectItem.rb.AddForce(new Vector2(0, -1f), ForceMode2D.Impulse);
                    nestedObjectItem.rb.excludeLayers = 0;
                    nestedObjectItem.gameObject.transform.SetParent(GameObject.Find("Storage").transform);
                }
                //gameObject.transform.SetParent(GameObject.Find("backpack").transform);
                gameObject.transform.SetParent(careHits[0].raycastHit.transform.parent.transform);
                CorrectPosition();
                SetNestedObject();
                rb.excludeLayers = (1 << 9) | (1 << 10);
                EffectPlaceCorrect();
                break;
            case 3:
                gameObject.transform.SetParent(GameObject.Find("Storage").transform);
                needToDynamic = true;
                timerStatic_locked_out = true;
                timerStatic = timer_cooldownStatic;

                //Impulse = true;
                //MoveObjectOnEndDrag();
                IgnoreCollisionObject(false);
                rb.excludeLayers = 0;// (1 << 9);
                EffectPlaceNoCorrect();
                break;
        }
    }

    public virtual void EffectPlaceCorrect()
    {
    }
    public virtual void EffectPlaceNoCorrect()
    {
    }

    public virtual Vector3 calculateOffset(List<BoxCollider2D> itemColliders)
    {
        var maxY = itemColliders[0].bounds.center.y;
        Vector3 offset = itemColliders[0].offset;

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


        float angle = rectTransform.eulerAngles.z;
        Debug.Log(angle);
        if (angle < 45 || angle > 315)
        {
            offset = -offset;
        }
        else if (angle >= 45 && angle < 135)
        {
            var i = offset.x;
            offset.x = offset.y;
            offset.y = i;

            offset.y = -offset.y;
        }
        else if (angle >= 135 && angle < 225)
        {
        }
        else // (angle >= 225 && angle < 315)
        {
            var i = offset.x;
            offset.x = offset.y;
            offset.y = i;

            offset.x = -offset.x;
        }


        //if (rectTransform.eulerAngles.z == 90f)
        //{

            

        //}
        //if (rectTransform.eulerAngles.z == 270f)
        //{

            

        //}
        //if (rectTransform.eulerAngles.z == 0)
        //{
            
        //}

        //Debug.Log(rectTransform.eulerAngles.z);

        return offset;
    }
    public void CorrectPosition()
    {
        if (hits.Where(e => e.hits[0].collider == null).Count() == 0)
        {
            var maxY = careHitsForBackpack[0].raycastHit.collider.transform.localPosition.y;
            Vector3 colliderPos = careHitsForBackpack[0].raycastHit.collider.transform.localPosition;

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
            rectTransform.localPosition = offset + colliderPos + new Vector3(0f, 0f, -2f);
            Debug.Log(offset);
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
            //Carehit.raycastHit.collider.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    public void SetNestedObject()
    {
        foreach (var Carehit in careHits)
        {
            Carehit.raycastHit.collider.GetComponent<Cell>().nestedObject = gameObject;
        }
    }

    public void MoveObjectOnEndDrag()
    {
        //List<GameObject> list = new List<GameObject>();
        //if (ItemInGameObject("Storage", list))
        var storageRect = GameObject.Find("Storage").GetComponent<RectTransform>().rect;
        int storageWidthDelenie = 3;
        if (gameObject.transform.localPosition.x > storageRect.min.x + storageRect.width / storageWidthDelenie
            &&
            gameObject.transform.localPosition.x < storageRect.max.x - storageRect.width / storageWidthDelenie)
        {
            //StartCoroutine(moveObject(storage.transform.position));
            //storageRect.position
        }
        else
        {
            //StartCoroutine(moveObject(lastItemPosition));
            StartCoroutine(moveObject(new Vector3(GameObject.Find("Storage").transform.position.x - 1, transform.position.y + 3, 0f)));
        }
    }
    public IEnumerator moveObject(Vector3 destination)
    {
        //IgnoreCollisionObject(true);//включаем игнорирование
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

    public virtual void SellChest()
    {
        if (hitSellChest.Any(e => e.collider != null && e.collider.name == "SellChest") && gameObject.GetComponent<ShopItem>() == null)
        {
            foreach (var hit in hitSellChest.Where(e => e.collider != null && e.collider.name == "SellChest"))
            {
                if (!isSellChest)
                {
                    sellChestSound = hit.collider.gameObject.GetComponent<AudioSource>();
                    sellChestAnimator = hit.collider.gameObject.GetComponent<Animator>();
                    sellChestAnimator.Play("SellChestOpen");
                    isSellChest = true;
                }
            }
        }
        else if (isSellChest)
        {
            sellChestAnimator.Play("SellChestClose");
            isSellChest = false;
        }
    }


    public void TapFirst()
    {
        if (firstTap)
        {
            firstTap = false;
            shopItem = GetComponent<ShopItem>();
            if (shopItem != null)
                shopItemStartPosition = shopItem.transform.position;
            //rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
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
    public void DeleteNestedObject(string tag)
    {
        Cell[] cellList = GameObject.FindWithTag(tag).GetComponentsInChildren<Cell>();

        foreach (var cell in cellList)
        {
            if (cell.nestedObject != null && cell.nestedObject.name == gameObject.name)
            {
                cell.nestedObject = null;
            }
        }
    }
    public void RotationToStartRotation()
    {
        if (needToRotateToStartRotation)
        {
            float angle = rectTransform.eulerAngles.z;
            if (angle < 45 || angle > 315)
            {
                needToRotateToStartRotation = false;
                rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            else if (angle >= 45 && angle < 135)
            {
                needToRotateToStartRotation = false;
                rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            }
            else if (angle >= 135 && angle < 225)
            {
                needToRotateToStartRotation = false;
                rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
            }
            else // (angle >= 225 && angle < 315)
            {
                needToRotateToStartRotation = false;
                rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
            }


            //if (rectTransform.eulerAngles.z >= -5 && rectTransform.eulerAngles.z <= 5)
            //{
                
            //}
            //else
            //{
            //    if (countClickRotate < maxCountClickRotate)
            //    {
            //        rectTransform.Rotate(0, 0, speedRotation * Time.deltaTime);
            //        countClickRotate++;
            //    }
            //    else
            //    {
            //        countClickRotate = 0;
            //        needToRotateToStartRotation = false;
            //        rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            //    }
            //}
        }
    }
    public void OnImpulse()
    {
        if (Impulse)
        {
            Impulse = false;
            // float screenWidth = Camera.main.orthographicSize * 2 * Screen.width / Screen.height;
            // float screenHeight = Camera.main.orthographicSize * 2;

            var storageRect = GameObject.Find("Storage").GetComponent<RectTransform>().rect;


            //rb.useAutoMass = true; //= baseMass + (storageRect.xMin + storageRect.yMin) * massMultiplier;
            rb.mass = 0.2f;
            rb.AddForce(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")), ForceMode2D.Impulse);
            rb.AddTorque(15);
            // rb.AddRelativeForceX(10, ForceMode2D.Impulse);
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
    public void Rotate()
    {
        // Получаем значение прокрутки колёсика мыши
        float scrollData = Input.GetAxis("Mouse ScrollWheel");

        if ((Input.GetKeyDown(KeyCode.R) || scrollData != 0) && needToRotate)
        {
            Vector3 newRotation = new Vector3(0, 0, 90);
            rectTransform.Rotate(newRotation);
            Physics2D.SyncTransforms();
            RaycastEvent();
        }
    }
    public Vector3 GetMouseWorldPosition()
    {
        // Получаем позицию курсора в мировых координатах
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = mainCamera.nearClipPlane; // Устанавливаем Z, чтобы получить координаты в 3D пространстве
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }
    public void IgnoreCollisionObject(bool ignoreCollisionObject)//true - ignore //false - not ignore
    {
        Collider2D[] colliders = FindObjectsByType<Collider2D>(FindObjectsSortMode.None);
        foreach (var otherCollider in colliders)
        {
            // ���������, ��� ��� �� ��� ���������
            if (otherCollider != collider && otherCollider.attachedRigidbody != null)
            {
                Physics2D.IgnoreCollision(collider, otherCollider, ignoreCollisionObject);
            }
        }
    }
    public virtual void SellItem()
    {
        var listCharacterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None);
        var characterStats = listCharacterStats[0];
        characterStats.playerCoins = characterStats.playerCoins + (int)(itemCost / 2);
        characterStats.coinsText.text = characterStats.playerCoins.ToString();
        sellChestSound.Play();
        sellChestAnimator.Play("SellChestClose");
        Destroy(gameObject);
        Destroy(CanvasDescription.gameObject);
    }


    public List<HitsStructure> CreateRaycast(System.Int32 mask)
    {
        List<HitsStructure> rayCasts = new List<HitsStructure>();
        foreach (var collider in itemColliders)
        {
            //��������� ����� ������? ToDo
            List<RaycastHit2D> hits = new List<RaycastHit2D>();

            //hits.Add(Physics2D.Raycast(collider.bounds.center, new Vector2(0.0f, 0.0f), 0, mask));
            //hits.Add(Physics2D.Raycast(collider.bounds.center, new Vector2(0.0f, 0.0f), 0, mask));
            //hits.Add(Physics2D.Raycast(collider.bounds.center, new Vector2(0.0f, 0.0f), 0, mask));
            //hits.Add(Physics2D.Raycast(collider.bounds.center, new Vector2(0.0f, 0.0f), 0, mask));
            //Physics2D.Raycast hit1 = Physics2D.Raycast(collider.bounds.center, new Vector2(0.0f, 0.0f), 0, mask);


            // �������� ���������� ����� ����������
            Vector2[] corners = new Vector2[4];
            corners[0] = collider.bounds.min; // ������ ����� ����
            corners[1] = new Vector2(collider.bounds.min.x, collider.bounds.max.y); // ������� ����� ����
            corners[2] = collider.bounds.max; // ������� ������ ����
            corners[3] = new Vector2(collider.bounds.max.x, collider.bounds.min.y); // ������ ������ ����

            // ����������� ��� 1/3 ����
            float t = 1f / 5f;
            // �������� ����� ����������
            Vector2 center = collider.bounds.center;

            // ��������� ���� �� �������� ������� ����� ������� � ������ �����
            for (int i = 0; i < corners.Length; i++)
            {
                Vector2 midPoint = center + t * (corners[i] - center); // ������� �������� ����� ������� � �����
                hits.Add(Physics2D.Raycast(midPoint, Vector2.zero, 0, mask)); // ��������� ���
                //hits.Add(Physics2D.Raycast(center, Vector2.zero, 0, mask)); // ��������� ��� �� ������
            }

            rayCasts.Add(new HitsStructure(hits));
        }
        return rayCasts;
    }
    public List<RaycastHit2D> CreateRaycastForSellChest(System.Int32 mask)
    {
        List<RaycastHit2D> rayCasts = new List<RaycastHit2D>();
        foreach (var collider in itemColliders)
        {
            List<RaycastHit2D> hits = new List<RaycastHit2D>();

            hits.Add(Physics2D.Raycast(collider.bounds.center, new Vector2(0.0f, 0.0f), 0, mask));

            //Physics2D.Raycast hit1 = Physics2D.Raycast(collider.bounds.center, new Vector2(0.0f, 0.0f), 0, mask);


            rayCasts.Add(Physics2D.Raycast(collider.bounds.center, new Vector2(0.0f, 0.0f), 0, mask));
        }
        return rayCasts;
    }
    public virtual void CreateCareRaycast()
    {
        bool createCareHit = true;
        foreach (var hit in hits)
        {
            if (hit.hits[0].collider != null && hit.hits[0].collider.gameObject.GetComponentInParent<ShopItem>() == null)
            {
                if (hit.hits.Where(e => e.collider != null).Count() == 4)
                {
                    foreach (var hitSmall in hit.hits)
                    {
                        if (hitSmall.collider.name != hit.hits[0].collider.name)
                        {
                            createCareHit = false;
                        }
                    }
                    if (createCareHit)
                    {
                        if (careHits.Where(e => e.raycastHit.collider != null && e.raycastHit.collider.name == hit.hits[0].collider.name).Count() == 0)
                        {
                            hit.hits[0].collider.GetComponent<SpriteRenderer>().color = Color.green;
                            careHits.Add(new RaycastStructure(hit.hits[0]));//�������
                        }
                    }
                }
            }
        }
        foreach (var hit in hitsForBackpack)
        {
            if (hit.collider != null)
            {
                if (careHitsForBackpack.Where(e => e.raycastHit.collider != null && e.raycastHit.collider.name == hit.collider.name).Count() == 0)
                {
                        careHitsForBackpack.Add(new RaycastStructure(hit));//�������
                        hit.collider.GetComponent<SpriteRenderer>().color = Color.yellow;
                        hit.collider.GetComponent<SpriteRenderer>().enabled = true;
                    
                }
            }
        }
    }

    public void updateColorCells()
    {
        foreach (var hit in hitsForBackpack)
        {
            if (hit.collider != null)
            {
                    hit.collider.GetComponent<SpriteRenderer>().color = Color.black;
                    hit.collider.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    public virtual void ClearCareRaycast(bool nested) //true - если внутри сумки, false - если без сумки
    {
        foreach (var Carehit in careHits)
        {
            foreach (var hit in hits)
            {

                // 1 hit
                // 4 hits
                if ((hit.hits.Where(e => e.collider != null && e.collider.name == Carehit.raycastHit.collider.name).Count() == 0) || hit.hits.Where(e => e.collider == null).Count() == colliderCount * 4)//ToDo
                {
                    Carehit.raycastHit.collider.GetComponent<SpriteRenderer>().color = Color.black;
                    Carehit.isDeleted = true;
                }
            }
        }

        careHits.RemoveAll(e => e.isDeleted == true);

        foreach (var Carehit in careHitsForBackpack)
        {
            foreach (var hit in hits)
            {
                if ((hit.hits.Where(e => e.collider != null && e.collider.name == Carehit.raycastHit.collider.name).Count() == 0) || hit.hits.Where(e => e.collider == null).Count() == colliderCount)
                {
                    Carehit.isDeleted = true;
                    Carehit.raycastHit.collider.GetComponent<SpriteRenderer>().color = Color.black;
                    if(!nested)
                        Carehit.raycastHit.collider.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }

        careHitsForBackpack.RemoveAll(e => e.isDeleted == true);
    }
    public virtual void RaycastEvent()
    {
        hits.Clear();
        hitsForBackpack.Clear();
        hitsForBackpack = CreateRaycastForSellChest(128);//ToDo

        hitsForNotShopZone.Clear();
        hitsForNotShopZone = CreateRaycastForSellChest(LayerMask.GetMask("NotShopZoneCollider"));
        hits = CreateRaycast(256);
        hitSellChest.Clear();
        hitSellChest = CreateRaycastForSellChest(32768);
        ClearCareRaycast(false);
        CreateCareRaycast();
        //FillnestedObjectStarsStars(256);
    }


    public void DeleteAllDescriptions()
    {
        var dp = GameObject.FindWithTag("DescriptionPlace");
        if (dp != null)
            for (int i = 0; i < dp.transform.childCount; i++)
                Destroy(dp.transform.GetChild(i).gameObject);
        dp = GameObject.FindWithTag("DescriptionPlaceEnemy");
        if(dp != null)
            for (int i = 0; i < dp.transform.childCount; i++)
                Destroy(dp.transform.GetChild(i).gameObject);
    }
    public virtual IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            ChangeShowStars(true);
            if (canShowDescription)
            {
                if (!showCanvasBefore)
                {
                    DeleteAllDescriptions();


                    CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
                    //showCanvas.transform.SetParent(GameObject.Find("Canvas").GetComponent<RectTransform>());
                }
                else
                {
                    //CanvasDescription.SetActive(true);
                    //var starsDesctiprion = CanvasDescription.GetComponentInChildren<SpriteRenderer>();
                    //if (starsDesctiprion != null)
                    //{
                    //    starsDesctiprion.enabled = true;
                    //}
                }
            }
        }
    }
    private void OnMouseEnter()
    {
        if (!isDragging)
        {
            // Код, который выполнится при наведении курсора на коллайдер
            if (SceneManager.GetActiveScene().name != "BackPackBattle") if (animator != null && !isEat) animator.Play("ItemAiming");
            Exit = false;
            StartCoroutine(ShowDescription());
        }
    }

    private void OnMouseExit()
    {
        if (!isDragging)
        {
            //Debug.Log(Description.gameObject.name + "�����");
            if (SceneManager.GetActiveScene().name != "BackPackBattle")
                if (animator != null && !isEat)
                {
                    animator.Play("ItemAimingOff");
                    if (GetComponent<AnimationStart>() != null)
                    {
                        GetComponent<AnimationStart>().Play();
                    }
                }
            //Debug.Log(Description.gameObject.name + " ItemAiming");
            Exit = true;
            ChangeShowStars(false);
            // Debug.Log("������ ������");
            if (canShowDescription && CanvasDescription != null)
            {
                //CanvasDescription.SetActive(false);
                Destroy(CanvasDescription.gameObject);
                //var starsDesctiprion = CanvasDescription.GetComponentInChildren<SpriteRenderer>();
                //if (starsDesctiprion != null)
                //{
                //    starsDesctiprion.enabled = false;
                //}
            }
        }
    }



    //public virtual void StarActivation()
    //{
    //    //Debug.Log("��������� " + this.name);
    //}
    public virtual void StarActivation(Item item)
    {
        //Debug.Log("��������� " + this.name);
    }

    public virtual void Activation()
    {
        //Debug.Log("��������� " + this.name);
    }
    public virtual int BlockActivation()
    {
        //Debug.Log("��������� " + this.name);
        return 0;
    }
    public virtual void StartActivation()
    {
        //Debug.Log("��������� " + this.name);
    }

    //public List<GameObject> nestedStarObjects = new List<GameObject>();
    public void FillnestedObjectStarsStars(System.Int32 mask)
    {
        RaycastHit2D raycast;
        foreach (var star in stars)
        {
            raycast = Physics2D.Raycast(star.GetComponent<RectTransform>().GetComponent<BoxCollider2D>().bounds.center, new Vector2(0.0f, 0.0f), 0, mask);
            if (raycast.collider != null && raycast.collider.gameObject.GetComponent<Cell>().nestedObject != gameObject)//
            {
                if (stars.Where(e => e.GetComponent<Cell>().nestedObject == raycast.collider.gameObject.GetComponent<Cell>().nestedObject).Count() == 0)
                {
                    star.GetComponent<Cell>().nestedObject = raycast.collider.gameObject.GetComponent<Cell>().nestedObject;
                    star.GetComponent<SpriteRenderer>().sprite = fillStar;
                }
            }
            else
            {
                star.GetComponent<Cell>().nestedObject = null;
                star.GetComponent<SpriteRenderer>().sprite = emptyStar;
            }
        }
    }

    public void FillnestedObjectStarsStars(System.Int32 mask, String tag)
    {
        RaycastHit2D raycast;
        foreach (var star in stars)
        {
            raycast = Physics2D.Raycast(star.GetComponent<RectTransform>().GetComponent<BoxCollider2D>().bounds.center, new Vector2(0.0f, 0.0f), 0, mask);
            if (raycast.collider != null && raycast.collider.gameObject.GetComponent<Cell>().nestedObject != gameObject)//
            {
                if (raycast.collider.gameObject.GetComponent<Cell>().nestedObject != null && raycast.collider.gameObject.GetComponent<Cell>().nestedObject.tag.Contains(tag))
                {
                    if (stars.Where(e => e.GetComponent<Cell>().nestedObject == raycast.collider.gameObject.GetComponent<Cell>().nestedObject).Count() == 0)
                    {
                        star.GetComponent<Cell>().nestedObject = raycast.collider.gameObject.GetComponent<Cell>().nestedObject;
                        star.GetComponent<SpriteRenderer>().sprite = fillStar;
                    }
                }
            }
            else
            {
                star.GetComponent<Cell>().nestedObject = null;
                star.GetComponent<SpriteRenderer>().sprite = emptyStar;
            }
        }
    }

    public void FillnestedObjectStarsStars(System.Int32 mask, String tag1, String tag2)
    {
        RaycastHit2D raycast;
        foreach (var star in stars)
        {
            raycast = Physics2D.Raycast(star.GetComponent<RectTransform>().GetComponent<BoxCollider2D>().bounds.center, new Vector2(0.0f, 0.0f), 0, mask);
            if (raycast.collider != null && raycast.collider.gameObject.GetComponent<Cell>().nestedObject != gameObject)//
            {
                if (raycast.collider.gameObject.GetComponent<Cell>().nestedObject != null && (raycast.collider.gameObject.GetComponent<Cell>().nestedObject.tag.Contains(tag1) || raycast.collider.gameObject.GetComponent<Cell>().nestedObject.tag.Contains(tag2)))
                {
                    if (stars.Where(e => e.GetComponent<Cell>().nestedObject == raycast.collider.gameObject.GetComponent<Cell>().nestedObject).Count() == 0)
                    {
                        star.GetComponent<Cell>().nestedObject = raycast.collider.gameObject.GetComponent<Cell>().nestedObject;
                        star.GetComponent<SpriteRenderer>().sprite = fillStar;
                    }
                }
            }
            else
            {
                star.GetComponent<Cell>().nestedObject = null;
                star.GetComponent<SpriteRenderer>().sprite = emptyStar;
            }
        }
    }
    public void DeletenestedObjectStars()
    {
        foreach (var star in stars)
        {
            star.GetComponent<Cell>().nestedObject = null;
            star.GetComponent<SpriteRenderer>().sprite = emptyStar;
        }
    }

    //public virtual void FillStarEffect(Item item)
    //{
    //    Debug.Log(gameObject.name + "   FillStarEffectItem");
    //}

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

    
    public void CoolDownStatic()
    {
        if (timerStatic_locked_out == true)
        {
            timerStatic -= Time.deltaTime;
            //Debug.Log(gameObject.name + " " + timerStatic.ToString());
            if (timerStatic <= 0)
            {
                timerStatic = timer_cooldownStatic;
                timerStatic_locked_out = false;
                this.needToDynamic = false;

                // a delayed action could be called from here
                // once the lock-out period expires
            }
        }
    }


    protected float timerStart = 0.5f;
    [HideInInspector] public float timer = 0f;
    [HideInInspector] public float timer_cooldown = 0f;
    public float baseTimerCooldown = 0f;




    public void CheckNestedObjectActivation(string objectActivation)
    {
        var bags = GameObject.FindGameObjectsWithTag(objectActivation);
        //var bagCells = GameObject.FindGameObjectsWithTag("BagCell");
        List<Bag> bagsWithFireBody = new List<Bag>();

        foreach (var bag in bags)
        {
            var bagCells = bag.GetComponentsInChildren<Cell>();
            bool find = false;
            foreach (var cell in bagCells)
            {
                if (!find)
                {
                    if (cell.nestedObject == gameObject)
                    {
                        bagsWithFireBody.Add(bag.GetComponent<Bag>());
                        find = true;
                        continue;
                    }
                }
            }
            if (find)
            {
                find = false;
                continue;
            }

        }

        foreach (var bag in bagsWithFireBody)
        {
            bag.StarActivation(null);
        }
    }

    public void CheckNestedObjectStarActivation(Item item)
    {
        var stars = GameObject.FindGameObjectsWithTag("StarActivation").Where(e => e.GetComponent<Cell>().nestedObject == gameObject);
        foreach (var star in stars)
        {
            star.GetComponentInParent<Item>().StarActivation(item);
        }
    }

    public void CreateLogMessage(string message, bool Player)
    {
        GameObject obj;
        if (Player)
        {
            obj = Instantiate(DescriptionLogCharacter, placeForLogDescription.GetComponent<RectTransform>().transform);
        }
        else
        {
            obj = Instantiate(DescriptionLogEnemy, placeForLogDescription.GetComponent<RectTransform>().transform);
        }
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        //obj.GetComponent<LogMessage>().nestedObject = gameObject;
    }

    public void CreateLogMessage(GameObject log, string message)
    {
        var obj = Instantiate(log, placeForLogDescription.GetComponent<RectTransform>().transform);
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        //obj.GetComponent<LogMessage>().nestedObject = gameObject;
    }


    private GameObject goAnimationAttack;
    public void StopAttackAnimation()
    {
        Destroy(goAnimationAttack);
    }
    public void AttackAnimation(int damage)
    {
        if (originalSprite != null)
        {
            GameObject goAnimationsAttack = GameObject.FindGameObjectWithTag("BattleAnimations");
            goAnimationAttack = Instantiate(prefabAnimationAttack, goAnimationsAttack.GetComponent<RectTransform>().transform);
            goAnimationAttack.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = originalSprite;
            goAnimationAttack.transform.GetChild(1).GetComponent<TextMeshPro>().text = "-" + damage.ToString();
            goAnimationAttack.transform.GetChild(1).GetComponent<TextMeshPro>().fontSize = 750 + damage;


            int r = UnityEngine.Random.Range(1, 6);
            if (gameObject.transform.parent.name == GameObject.Find("backpack").transform.name)//значит атакует врага
            {
                goAnimationAttack.GetComponent<Animator>().Play("itemAttackEnemy" + r.ToString());
            }
            else//атакуют персонажа
            {
                goAnimationAttack.GetComponent<Animator>().Play("itemAttackPlayer" + r.ToString());
            }
            Invoke("StopAttackAnimation", 0.4f);
        }

    }

    protected void Attack(int damage, bool anim)
    {
        if (anim)
        {
            AttackAnimation(damage);
        }
        float armorBefore = Enemy.armor;
        if (damage < armorBefore)
        {
            Enemy.armor -= damage;
            CreateLogMessage(gameObject.name + " destroy " + damage.ToString() + " armor", Player.isPlayer);
        }
        else
        {
            int dmgArmor = (int)armorBefore;
            Enemy.hp -= (damage - dmgArmor);
            if (armorBefore == 0)
            {
                CreateLogMessage(gameObject.name + " deal " + Math.Abs((Enemy.armor - damage)).ToString() + " damage", Player.isPlayer);
            }
            else
            {
                CreateLogMessage(gameObject.name + " destroy " + armorBefore.ToString() + " armor and deal " + Math.Abs((Enemy.armor - damage)).ToString() + " damage", Player.isPlayer);
            }
            Enemy.armor = 0;
        }
    }

    protected void AttackSelf(int damage)
    {
        float armorBefore = Player.armor;
        if (damage < Player.armor)
        {
            Player.armor -= damage;
            CreateLogMessage(gameObject.name + " destroy " + damage.ToString() + " armor", !Player.isPlayer);
        }
        else
        {
            int dmgArmor = (int)Player.armor;
            Player.hp -= (damage - dmgArmor);
            if (armorBefore == 0)
            {
                CreateLogMessage(gameObject.name + " deal " + Math.Abs((Player.armor - damage)).ToString() + " damage", !Player.isPlayer);
            }
            else
            {
                CreateLogMessage(gameObject.name + " destroy " + armorBefore.ToString() + " armor and deal " + Math.Abs((Player.armor - damage)).ToString() + " damage", !Player.isPlayer);
            }
            Player.armor = 0;
        }
    }

}
