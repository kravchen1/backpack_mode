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
    [HideInInspector] public CharacterStats characterStats;

    [HideInInspector] public bool lastParentWasStorage;
    public GameObject Description;
    //public GameObject DescriptionLogCharacter;
    //public GameObject DescriptionLogEnemy;

    private Sprite originalSprite;
    private GameObject prefabAnimationAttack;

    [HideInInspector] public GameObject CanvasDescription;


    [HideInInspector] private bool showCanvasBefore = false;
    [HideInInspector] protected bool canShowDescription = true;
    [HideInInspector] public bool Exit = false;
    //public float rbMass = 0.1f;

    //public int originalLayer = 0;

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
    [HideInInspector] public Collider2D collider;

    [HideInInspector] public Vector3 lastItemPosition;

    [HideInInspector] public RectTransform rectTransform;

    [HideInInspector] public bool firstTap = true; //�������, �� ����
    [HideInInspector] public bool needToRotate;
    [HideInInspector] public bool needToDynamic = false;
    [HideInInspector] public bool needToRotateToStartRotation = false;

    [HideInInspector] protected PlayerBackpackBattle Player;
    [HideInInspector] protected PlayerBackpackBattle Enemy;
    [HideInInspector] public GameObject placeForDescription;


    [HideInInspector] public Animator animator;
    [HideInInspector] public Animator sellChestAnimator;
    [HideInInspector] public AudioSource sellChestSound;
    [HideInInspector] public bool Impulse = false;

    [HideInInspector] public List<GameObject> stars;
    private Sprite emptyStar;
    private Sprite fillStar;

    [HideInInspector] public ShopItem shopItem;

    [HideInInspector] public bool isSellChest = false;

    [HideInInspector] private Camera mainCamera;
    [HideInInspector] public bool isDragging = false;
    [HideInInspector] public Vector3 offset;
    [HideInInspector] private int countClickRotate = 0, maxCountClickRotate = 100;
    [HideInInspector] public float timer_cooldownStatic = 12.5f;
    [HideInInspector] public float timerStatic = 12.5f;
    [HideInInspector] public bool timerStatic_locked_out = true;

    [HideInInspector] public OtherItemMusicEffects itemMusicEffects;
    public String originalName;


    protected float timer_cooldownStart = 1f;
    protected bool timer_locked_outStart = true;
    [HideInInspector] protected bool isEat = false;


    private Animator playerAnimator;
    private Animator enemyAnimator;

    protected LogManager logManager;

    private float minDelay = 0.1f;
    private float maxDelay = 1.0f;
    private float moveDistance = 0.5f;


    public enum ItemRarity
    {
        Common,      // Обычный
        Rare,        // Редкий
        Epic,        // Эпический
        Legendary  // Легендарный
    }
    public ItemRarity rarity;


    public enum ItemType
    {
        Weapon,
        Bag,
        Pet,
        Stone,
        Food
    }
    public ItemType itemType;

    public float weight;


    void Awake()
    {
        FindPlayerAndEnemyForBattle();
        itemMusicEffects = GetComponent<OtherItemMusicEffects>();
        Initialization();
    }

    private void Start()
    {
        //OnImpulse();

    }


    private void FindPlayerAndEnemyForBattle()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();
            enemyAnimator = GameObject.FindGameObjectWithTag("Enemy").GetComponentInChildren<Animator>();
            goAnimationsAttack = GameObject.FindGameObjectWithTag("BattleAnimations");
        }
    }

    public void Initialization()
    {
        if (!PlayerPrefs.HasKey("Found" + originalName))
        {
            PlayerPrefs.SetInt("Found" + originalName, 1);
        }

        shopItem = GetComponent<ShopItem>();
        rb = GetComponent<Rigidbody2D>();
        rectTransform = GetComponent<RectTransform>();
        startRectTransformZ = rectTransform.eulerAngles.z;
        image = GetComponent<SpriteRenderer>();
        canvas = GetComponentInParent<Canvas>();
        imageColor = image.color;
        originalSprite = image.sprite;

        prefabAnimationAttack = Resources.Load<GameObject>("AttackAnimation");
        emptyStar = Resources.Load<Sprite>("Items/stars/EmptyStar");
        fillStar = Resources.Load<Sprite>("Items/stars/FillStar");

        stars = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.tag == "StarActivation")
            {
                stars.Add(child);
            }
        }

        needToRotate = false;
        collider = GetComponent<PolygonCollider2D>();
        //originalLayer = gameObject.layer;
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
            logManager = GameObject.FindGameObjectWithTag("BattleLog").GetComponent<LogManager>();
            if (gameObject.transform.parent.name == GameObject.FindGameObjectWithTag("backpack").transform.name)
            {
                placeForDescription = GameObject.FindWithTag("DescriptionPlace");
                Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBackpackBattle>();
                Enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<PlayerBackpackBattle>();
            }

            if (gameObject.transform.parent.name == GameObject.FindGameObjectWithTag("backpackEnemy").transform.name)
            {
                placeForDescription = GameObject.FindWithTag("DescriptionPlaceEnemy");
                Player = GameObject.FindGameObjectWithTag("Enemy").GetComponent<PlayerBackpackBattle>();
                Enemy = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBackpackBattle>();
            }
        }
        else
        {
            FindPlaceForDescription();
        }
    }

    void FindPlaceForDescription()
    {
        if (gameObject.transform.parent.name == GameObject.FindGameObjectWithTag("backpack").transform.name || gameObject.transform.parent.name == GameObject.FindGameObjectWithTag("Storage").transform.name)
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

    protected Vector3 shopItemStartPosition;
    public virtual void OnMouseDown()
    {
        if (returnToOriginalPosition != null)
        {
            StopCoroutine(returnToOriginalPosition);
            DragManager.isReturnToOrgignalPos = false;
        }
        lastParentWasStorage = transform.parent.CompareTag("Storage");
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

                    //gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("backpack").transform);
                    ChangeShowStars(true);
                    //gameObject.layer = LayerMask.NameToLayer("DraggingObject");
                    // Начинаем перетаскивание
                    isDragging = true;
                    // Вычисляем смещение между курсором и объектом

                    offset = transform.position - GetMouseWorldPosition();
                    //Debug.Log(offset);
                }
                else
                {

                }
            }
            else
            {
                if (gameObject.transform.parent.CompareTag("Storage"))
                {
                    ObjectsDynamic("Storage");
                }

                DragManager.isDragging = true;
                itemMusicEffects.OnItemUp();
                if (animator != null && !isEat) animator.Play("ItemClick");
                DeletenestedObjectStars();
                IgnoreCollisionObject(true);
                image.sortingOrder = 4;
                if (!DragManager.isReturnToOrgignalPos)
                    lastItemPosition = gameObject.transform.position;
                needToDynamic = true;
                //List<GameObject> list = new List<GameObject>();
                //if (ItemInGameObject("Shop", list) && shopData.CanBuy(gameObject.GetComponent<Item>()))
                //TapFirst();
                TapRotate();

                //gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("backpack").transform);
                ChangeShowStars(true);


                // Начинаем перетаскивание
                isDragging = true;
                //gameObject.layer = LayerMask.NameToLayer("DraggingObject");
                //Меняем слой на DraggingObject
                // Вычисляем смещение между курсором и объектом
                offset = transform.position - GetMouseWorldPosition();
            }
        }
    }
    protected Coroutine returnToOriginalPosition;

    public virtual void OnMouseUp()
    {
        MouseUp();
    }

    private void MouseUp()
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

        bool sellItem = false;
        //if (SceneManager.GetActiveScene().name == "BackPackShop" || SceneManager.GetActiveScene().name == "BackpackView")
        if (SceneManager.GetActiveScene().name != "BackPackBattle" && SceneManager.GetActiveScene().name != "GenerateMap" && SceneManager.GetActiveScene().name != "Cave" && SceneManager.GetActiveScene().name != "SceneShowItems")
        {
            needToRotate = false;
            image.color = imageColor;
            if (shopItem != null)
            {
                if (hitsForNotShopZone.Any(e => e.collider != null))
                {
                    DeleteNestedObject(gameObject.transform.parent.tag);
                    shopItem.BuyItem(gameObject.GetComponent<Item>());

                    ExtendedCorrectPosition();
                    ChangeColorToDefault();

                    placeForDescription = GameObject.FindWithTag("DescriptionPlace");

                    needToRotateToStartRotation = false;
                }
                else
                {
                    if (shopItem.defaultPosition != transform.position)
                    {
                        returnToOriginalPosition = StartCoroutine(ReturnToOriginalPosition(shopItem.defaultPosition));
                    }
                }
            }
            else
            {
                if (hitsForNotShopZone.Any(e => e.collider != null))
                {
                    DeleteNestedObject(gameObject.transform.parent.tag);
                    ExtendedCorrectPosition();
                    ChangeColorToDefault();


                    needToRotateToStartRotation = false;
                }
                else
                {
                    if (lastItemPosition != transform.position && !isSellChest)
                    {
                        returnToOriginalPosition = StartCoroutine(ReturnToOriginalPosition(lastItemPosition));
                    }
                }
            }
            if (isSellChest)
            {
                sellItem = true;
                SellItem();
            }

            // Заканчиваем перетаскивание
            isDragging = false;
            //gameObject.layer = originalLayer;
            ClearCareRaycast(false);
            image.sortingOrder = 1;
            //ChangeShowStars(true);

            Vector2 mousePos = Input.mousePosition;

            if (!(mousePos.x < 0 ||
                    mousePos.x > Screen.width ||
                    mousePos.y < 0 ||
                    mousePos.y > Screen.height)
            )
            {
                if (!sellItem)
                {
                    Exit = false;
                    canShowDescription = true;
                    ShowDescription();
                }
            }
            else
            {
                //Debug.Log("Курсор за пределами игрового экрана!");
                ChangeShowStars(false);
            }


            FindPlaceForDescription();


        }

        
    }

    public void CouratineMove(Vector3 destination)
    {
        StartCoroutine(ReturnToOriginalPosition(destination));
    }

    public virtual System.Collections.IEnumerator ReturnToOriginalPosition(Vector3 originalPosition)
    {
        //Debug.Log(DragManager.isReturnToOrgignalPos);
        DragManager.isReturnToOrgignalPos = true;
        //Debug.Log(DragManager.isReturnToOrgignalPos);
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
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -2);
        if (transform.parent.tag == "Storage")
        {
            needToDynamic = true;
        }
        DragManager.isReturnToOrgignalPos = false;
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
                FillStars();
            }
            canShowDescription = false;
        }
        Rotate();
        SwitchDynamicStatic();
        //OnImpulse();
        RotationToStartRotation();
        CoolDownStatic();

    }


    public virtual void Update()
    {
        try
        {
            defaultItemUpdate();
        }
        catch
        {
            MouseUp();
        }
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
    public virtual int ExtendedCorrectEndPoint()
    {
        if (careHits != null)
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
        else
        {
            return 0;
        }
    }
    public virtual void ExtendedCorrectPosition()
    {
        switch (ExtendedCorrectEndPoint())
        {
            case 1:
                //Debug.Log("case1");
                gameObject.transform.SetParent(careHits[0].raycastHit.transform.parent.transform);
                if (characterStats == null)
                {
                    characterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None)[0];
                }
                //if (lastParentWasStorage)
                //{
                //    decimal preciseWeight = (decimal)characterStats.storageWeight - (decimal)weight;
                //    characterStats.storageWeight = (float)Math.Round(preciseWeight, 2);
                //    lastParentWasStorage = false;
                //}
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
                    nestedObjectItem.DeleteNestedObject(nestedObjectItem.transform.parent.tag);
                    nestedObjectItem.needToDynamic = true;
                    timerStatic_locked_out = true;
                    timerStatic = timer_cooldownStatic;
                    //nestedObjectItem.Impulse = true;
                    //nestedObjectItem.rb.AddForce(new Vector2(0, -1f), ForceMode2D.Impulse);
                    nestedObjectItem.rb.excludeLayers = 0;
                    nestedObjectItem.gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("Storage").transform);
                    if (characterStats == null)
                    {
                        characterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None)[0];
                    }
                    //nestedObjectItem.lastParentWasStorage = true;
                    //decimal preciseWeight = (decimal)characterStats.storageWeight + (decimal)nestedObjectItem.weight;
                    //characterStats.storageWeight = (float)Math.Round(preciseWeight, 2);
                    //Debug.Log("case2");
                }
                //gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("backpack").transform);
                gameObject.transform.SetParent(careHits[0].raycastHit.transform.parent.transform);
                CorrectPosition();
                SetNestedObject();
                rb.excludeLayers = (1 << 9) | (1 << 10);
                EffectPlaceCorrect();
                //if (lastParentWasStorage)
                //{
                //    decimal preciseWeight = (decimal)characterStats.storageWeight - (decimal)weight;
                //    characterStats.storageWeight = (float)Math.Round(preciseWeight, 2);
                //    lastParentWasStorage = false;
                //}
                break;
            case 3:
                //Debug.Log("case3");
                gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("Storage").transform);
                if (characterStats == null)
                {
                    characterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None)[0];
                }
                //if (!lastParentWasStorage)
                //{
                //    decimal preciseWeight = (decimal)characterStats.storageWeight + (decimal)weight;
                //    characterStats.storageWeight = (float)Math.Round(preciseWeight, 2);
                //    lastParentWasStorage = true;
                //}
                needToDynamic = true;
                timerStatic_locked_out = true;
                timerStatic = timer_cooldownStatic;

                //Impulse = true;
                //MoveObjectOnEndDrag();
                IgnoreCollisionObject(false);
                rb.excludeLayers = (1 << 10) | (1 << 11);
                EffectPlaceNoCorrect();
                break;
        }
    }

    public virtual void EffectPlaceCorrect()
    {
        //lastItemPosition = transform.position;
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
        //Debug.Log(angle);
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
    public virtual void CorrectPosition()
    {
        if (hits.Where(e => e.hits[0].collider == null).Count() == 0)
        {
            try
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
                //Debug.Log(offset);
                needToDynamic = false;
                foreach (var careHit in careHitsForBackpack)
                {
                    careHit.raycastHit.collider.GetComponent<SpriteRenderer>().color = Color.black;
                }
            }
            catch (Exception ex)
            {

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
        var storageRect = GameObject.FindGameObjectWithTag("Storage").GetComponent<RectTransform>().rect;
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
            StartCoroutine(moveObject(new Vector3(GameObject.FindGameObjectWithTag("Storage").transform.position.x - 1, transform.position.y + 3, 0f)));
        }
    }
    public IEnumerator moveObject(Vector3 destination)
    {
        //IgnoreCollisionObject(true);//включаем игнорирование
        var origin = transform.position;

        //var destination = GameObject.FindGameObjectWithTag("Storage").transform.position;
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

    protected GameObject sellPrice;
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
                    if (SceneManager.GetActiveScene().name != "BackPack")
                    {
                        sellPrice = hit.collider.gameObject.transform.GetChild(0).gameObject;
                        sellPrice.SetActive(true);
                        sellPrice.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>().text = ((int)(itemCost / 2)).ToString();
                    }
                }
            }
        }
        else if (isSellChest)
        {
            sellChestAnimator.Play("SellChestClose");
            isSellChest = false;
            if (SceneManager.GetActiveScene().name != "BackPack")
            {
                sellPrice.SetActive(false);
            }
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


    IEnumerator MoveWithRandomDelay()
    {
        // Рандомная задержка для распределения нагрузки
        float delay = UnityEngine.Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);

        // Смещаем объект в случайном направлении (без физики)
        Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
        transform.position += (Vector3)(randomDir * moveDistance);
    }
    public void OnImpulse()
    {
        if (Impulse)
        {
            //Time.fixedDeltaTime = 0.06f;
            Impulse = false;
            // float screenWidth = Camera.main.orthographicSize * 2 * Screen.width / Screen.height;
            // float screenHeight = Camera.main.orthographicSize * 2;

            //var storageRect = GameObject.FindGameObjectWithTag("Storage").GetComponent<RectTransform>().rect;
            MoveWithRandomDelay();
            //rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
            //rb.useAutoMass = true; //= baseMass + (storageRect.xMin + storageRect.yMin) * massMultiplier;
            //rb.mass = 0.2f;
            //rb.AddForce(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")), ForceMode2D.Impulse);
            //rb.AddForce(new Vector2(randomX, randomY), ForceMode2D.Impulse);
            //rb.AddTorque(15);
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
    private List<Collider2D> _ignoredColliders = new List<Collider2D>();

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
        characterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None)[0];
        //if (lastParentWasStorage)
        //{
        //    decimal preciseWeight = (decimal)characterStats.storageWeight - (decimal)weight;
        //    characterStats.storageWeight = (float)Math.Round(preciseWeight, 2);
        //}
        if (SceneManager.GetActiveScene().name != "BackPack")
        {
            characterStats.playerCoins = characterStats.playerCoins + (int)(itemCost / 2);
            characterStats.coinsText.text = characterStats.playerCoins.ToString();
        }
        sellChestSound.Play();
        sellChestAnimator.Play("SellChestClose");
        if (sellPrice != null)
        {
            sellPrice.SetActive(false);
        }
        Destroy(gameObject);
        Destroy(CanvasDescription.gameObject);
    }


    public List<HitsStructure> CreateRaycast(System.Int32 mask)
    {
        List<HitsStructure> rayCasts = new List<HitsStructure>();
        foreach (var collider in itemColliders)
        {
            try
            {
                List<RaycastHit2D> hits = new List<RaycastHit2D>();
                Vector2[] corners = new Vector2[4];
                corners[0] = collider.bounds.min; // ������ ����� ����
                corners[1] = new Vector2(collider.bounds.min.x, collider.bounds.max.y); // ������� ����� ����
                corners[2] = collider.bounds.max; // ������� ������ ����
                corners[3] = new Vector2(collider.bounds.max.x, collider.bounds.min.y); // ������ ������ ����

                float t = 1f / 5f;
                // 
                Vector2 center = collider.bounds.center;

                // 
                for (int i = 0; i < corners.Length; i++)
                {
                    Vector2 midPoint = center + t * (corners[i] - center); // 
                    hits.Add(Physics2D.Raycast(midPoint, Vector2.zero, 0, mask)); //
                }

                rayCasts.Add(new HitsStructure(hits));
            }
            catch
            {
                return null;
            }
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
                        if (hit.hits[0].collider.gameObject.transform.parent.parent.tag != "Storage")
                        {
                            if (careHits.Where(e => e.raycastHit.collider != null && e.raycastHit.collider.name == hit.hits[0].collider.name).Count() == 0)
                            {
                                if (hit.hits[0].collider.GetComponent<Cell>().nestedObject != null && hit.hits[0].collider.GetComponent<Cell>().nestedObject != this.gameObject)
                                    hit.hits[0].collider.GetComponent<SpriteRenderer>().color = Color.yellow;
                                else
                                    hit.hits[0].collider.GetComponent<SpriteRenderer>().color = Color.green;
                                careHits.Add(new RaycastStructure(hit.hits[0]));//�������
                            }
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
        if (careHits != null && hits != null)
        {
            //Debug.Log(gameObject.name + "1");
            foreach (var Carehit in careHits)
            {
                foreach (var hit in hits)
                {

                    // 1 hit
                    // 4 hits
                    if ((hit.hits.Where(e => e.collider != null && e.collider.name == Carehit.raycastHit.collider.name).Count() == 0) || hit.hits.Where(e => e.collider == null).Count() == colliderCount * 4)//ToDo
                    {
                        //Debug.Log(gameObject.name + "2");
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
                    //Debug.Log(gameObject.name + " 2.5 " + hit.hits.Where(e => e.collider == null).Count() + " / " + colliderCount + " : " + hit.hits.Where(e => e.collider != null && e.collider.name == Carehit.raycastHit.collider.name).Count());
                    if ((hit.hits.Where(e => e.collider != null && e.collider.name == Carehit.raycastHit.collider.name).Count() == 0) || hit.hits.Where(e => e.collider == null).Count() == colliderCount)
                    {
                        //Debug.Log(gameObject.name + "3");
                        Carehit.isDeleted = true;
                        Carehit.raycastHit.collider.GetComponent<SpriteRenderer>().color = Color.black;
                        if (!nested)
                            Carehit.raycastHit.collider.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
            }

            careHitsForBackpack.RemoveAll(e => e.isDeleted == true);
        }
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
        if (dp != null)
            for (int i = 0; i < dp.transform.childCount; i++)
                Destroy(dp.transform.GetChild(i).gameObject);
    }
    public virtual void ShowDescription()
    {
        if (!Exit)
        {
            ChangeShowStars(true);
            if (canShowDescription && !DragManager.isDragging)
            {
                if (!showCanvasBefore)
                {
                    DeleteAllDescriptions();

                    CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
                }
            }
        }
        else
        {
            DeleteAllDescriptions();
        }
    }

    private void OnMouseEnter()
    {
        if (!DragManager.isDragging)
        {
            // Код, который выполнится при наведении курсора на коллайдер
            if (SceneManager.GetActiveScene().name != "BackPackBattle") if (animator != null && !isEat) animator.Play("ItemAiming");
            Exit = false;
            //Debug.Log(Time.time + (!Exit).ToString());
            //Debug.Log(DragManager.isDragging);
            ShowDescription();
            //ShowDescription2();
        }
    }

    private void OnMouseExit()
    {
        if (!Exit)
        {
            Exit = true;
            //Debug.Log(Time.time + (!Exit).ToString());
        }
        
        if (!MouseExit())
        {
            Invoke("MouseExit", 0.2f);
            //StartCoroutine(ShowDescription());
        }
    }

    bool MouseExit()
    {
        if (!DragManager.isDragging)
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
            ChangeShowStars(false);
            //Debug.Log(canShowDescription + "/" + CanvasDescription.name);
            if (canShowDescription && CanvasDescription != null)
            {
                Destroy(CanvasDescription.gameObject);
                return true;
            }
            return false;
        }
        return false;
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
        DeletenestedObjectStars();
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
        DeletenestedObjectStars();
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
        DeletenestedObjectStars();
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
                //Time.fixedDeltaTime = 0.02f;
                // a delayed action could be called from here
                // once the lock-out period expires
            }
        }
    }


    protected float timerStart = 0.5f;
    [HideInInspector] public float timer = 0f;
    [HideInInspector] public float timer_cooldown = 0f;
    public float baseTimerCooldown = 0f;

    public void ObjectsDynamic(string tag)
    {
        GameObject parentObject = GameObject.FindWithTag(tag);

        for(int i = 0; i< parentObject.transform.childCount; i++)
        {
           
            if(parentObject.transform.GetChild(i).GetComponent<Item>() != null)
            {
                var item = parentObject.transform.GetChild(i).GetComponent<Item>();
                item.needToDynamic = true;
                item.timerStatic_locked_out = true;
                timerStatic = timer_cooldownStatic;
            }
        }
    }


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

    


    private GameObject goAnimationAttack;
    private GameObject goAnimationsAttack;
    public void StopAttackAnimation()
    {
        Destroy(goAnimationAttack);
    }
    public void AttackAnimation(int damage)
    {
        if (originalSprite != null)
        {
            
            if (goAnimationAttack != null)
            {
                Destroy(goAnimationAttack);
            }
            //Debug.Log(prefabAnimationAttack);
            goAnimationAttack = Instantiate(prefabAnimationAttack, goAnimationsAttack.GetComponent<RectTransform>().transform);
            if (prefabAnimationAttack == null)
            {
                Debug.Log(originalName + " не заполнена анимации атаки, а используется");
            }
            goAnimationAttack.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = originalSprite;
            goAnimationAttack.transform.GetChild(1).GetComponent<TextMeshPro>().text = "-" + damage.ToString();
            goAnimationAttack.transform.GetChild(1).GetComponent<TextMeshPro>().fontSize = 750 + damage;


            int r = UnityEngine.Random.Range(1, 6);
            if (gameObject.transform.parent.name == GameObject.FindGameObjectWithTag("backpack").transform.name)//значит атакует врага
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                goAnimationAttack.GetComponent<Animator>().Play("itemAttackEnemy" + r.ToString());
                if (playerAnimator == null)
                {
                    FindPlayerAndEnemyForBattle();
                }
                playerAnimator.Play("Attack1", -1, 0f);
            }
            else//атакуют персонажа
            {
                goAnimationAttack.GetComponent<Animator>().Play("itemAttackPlayer" + r.ToString());
                if (enemyAnimator == null)
                {
                    FindPlayerAndEnemyForBattle();
                }
                if (enemyAnimator != null) enemyAnimator.Play("Attack1", -1, 0f);
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



        int armorBefore = Enemy.armor;
        if (damage < armorBefore)
        {
            Enemy.armor -= damage;
            logManager.CreateLogMessageAttackOnArmor(originalName, damage, Player.isPlayer);
            //CreateLogMessage(gameObject.name + " destroy " + damage.ToString() + " armor", Player.isPlayer);
        }
        else
        {
            int dmgArmor = (int)armorBefore;
            Enemy.hp -= (damage - dmgArmor);
            if (armorBefore == 0)
            {
                //CreateLogMessage(gameObject.name + " deal " + Math.Abs((Enemy.armor - damage)).ToString() + " damage", Player.isPlayer);
                logManager.CreateLogMessageAttackWithoutArmor(originalName, damage, Player.isPlayer);
            }
            else
            {
                //CreateLogMessage(gameObject.name + " destroy " + armorBefore.ToString() + " armor and deal " + Math.Abs((Enemy.armor - damage)).ToString() + " damage", Player.isPlayer);
                logManager.CreateLogMessageAttackOnHalfArmor(originalName, Math.Abs((Enemy.armor - damage)), armorBefore, Player.isPlayer);
            }
            Enemy.armor = 0;
        }
    }

    protected void AttackSelf(int damage)
    {
        int armorBefore = Player.armor;
        if (damage < Player.armor)
        {
            Player.armor -= damage;
            logManager.CreateLogMessageAttackOnArmor(originalName, damage, !Player.isPlayer);
        }
        else
        {
            int dmgArmor = (int)Player.armor;
            Player.hp -= (damage - dmgArmor);
            if (armorBefore == 0)
            {
                logManager.CreateLogMessageAttackWithoutArmor(originalName, damage, !Player.isPlayer);
            }
            else
            {
                logManager.CreateLogMessageAttackOnHalfArmor(originalName, Math.Abs((Enemy.armor - damage)), armorBefore, !Player.isPlayer);
            }
            Player.armor = 0;
        }
    }








    
    public void CreateLogMessage(string message, bool Player)
    {

        
        if (Player)
        {
            //obj = Instantiate(DescriptionLogCharacter, placeForLogDescription.GetComponent<RectTransform>().transform);
        }
        else
        {
            //obj = Instantiate(DescriptionLogEnemy, placeForLogDescription.GetComponent<RectTransform>().transform);
        }
        //obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ts.nowTime.ToString() + " - " + message;
        //obj.GetComponent<LogMessage>().nestedObject = gameObject;
    }

    public void CreateLogMessage(GameObject log, string message)
    {
        TimeSpeed ts;
        ts = GameObject.FindGameObjectWithTag("SliderTime").GetComponent<TimeSpeed>();
        //var obj = Instantiate(log, placeForLogDescription.GetComponent<RectTransform>().transform);
        //obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ts.nowTime.ToString() + " - " + message;
        //obj.GetComponent<LogMessage>().nestedObject = gameObject;
    }


    protected virtual void FillStars()
    {
        //FillnestedObjectStarsStars(256);
    }
}
