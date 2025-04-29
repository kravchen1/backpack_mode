using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using Assets.Scripts.ItemScripts;
using static Item_v2;


//public class HitsStructure_v2
//{
//    public List<RaycastHit2D> hits = new List<RaycastHit2D>();
//    public HitsStructure_v2(List<RaycastHit2D> hits)
//    {
//        this.hits = hits;
//    }
//}
public abstract class Item_v2 : MonoBehaviour
{
    [HideInInspector] private Animator animator;
    //dragging--
    [HideInInspector] private Camera mainCamera;
    [HideInInspector] private Vector3 offset;
    [HideInInspector] private bool isDragging = false;
    //--dragging

    //raycasts--
    [HideInInspector] private List<BoxCollider2D> itemColliders = new List<BoxCollider2D>();

    [HideInInspector] private List<HitsStructure> hits = new List<HitsStructure>();
    [HideInInspector] private List<RaycastStructure> careHits = new List<RaycastStructure>();
    //--raycasts


    //descriptions--
    [HideInInspector] private GameObject placeForDescription;
    public GameObject Description;
    [HideInInspector] private GameObject CanvasDescription;
    //--descriptions


    public enum ItemPlace
    {
        Backpack,
        Storage,
        CaveStone,
        Warehouse,
        Shop
    }
    public ItemPlace itemPlace;

    private void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        itemPlace = FindItemPlace();
        FindPlaceForDescription();


        initializationItemColliders();
    }

    ItemPlace FindItemPlace()
    {
        //todo
        return ItemPlace.Shop;
    }


    public virtual void OnMouseDown()
    {
        InitializedMouseDown();
    }

    public virtual void OnMouseUp()
    {
        InitializedMouseUp();
    }

    public virtual void Update()
    {
        Dragging();
    }

    private void OnMouseEnter()
    {
        if (!DragManager.isDragging)
        {
            if (animator != null) animator.Play("ItemAiming");
            ShowDescription();
        }
    }

    private void OnMouseExit()
    {
        if (!DragManager.isDragging)
        {
            if (animator != null) animator.Play("ItemAimingOff");
            EndShowDescription();
        }
    }


    //dragging--
    Vector3 GetMouseWorldPosition()
    {
        // Получаем позицию курсора в мировых координатах
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = mainCamera.nearClipPlane; // Устанавливаем Z, чтобы получить координаты в 3D пространстве
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }

    void InitializedMouseDown()
    {
        offset = transform.position - GetMouseWorldPosition();
        isDragging = true;
        DragManager.isDragging = true;
        if (animator != null) animator.Play("ItemClick");
    }

    void InitializedMouseUp()
    {
        isDragging = false;
        DragManager.isDragging = false;
        if (animator != null) animator.Play("ItemClickOff");
    }

    void Dragging()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPosition() + offset;
            RaycastEvent();
        }
    }
    //--dragging


    //raycasts--
    void initializationItemColliders()
    {
        BoxCollider2D[] collidersArray;
        collidersArray = gameObject.GetComponentsInChildren<BoxCollider2D>();

        for (int i = 0; i < collidersArray.Count(); i++)
        {
            if (!collidersArray[i].name.Contains("Star"))
                itemColliders.Add(collidersArray[i]);
        }
    }
    public virtual void RaycastEvent()
    {
        hits.Clear();
        hits = CreateRaycast(128);
        ClearCareRaycast();
        CreateCareRaycast();
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
                
                Vector2 center = collider.bounds.center;

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
                            if (hit.hits[0].collider.GetComponent<Cell>().nestedObject != null && hit.hits[0].collider.GetComponent<Cell>().nestedObject != this.gameObject)
                                hit.hits[0].collider.GetComponent<SpriteRenderer>().color = Color.yellow;
                            else
                                hit.hits[0].collider.GetComponent<SpriteRenderer>().color = Color.green;
                            careHits.Add(new RaycastStructure(hit.hits[0]));
                        }
                    }
                }
            }
        }

    }
    public virtual void ClearCareRaycast()
    {
        if (careHits != null && hits != null)
        {
            foreach (var Carehit in careHits)
            {
                foreach (var hit in hits)
                {
                    if ((hit.hits.Where(e => e.collider != null && e.collider.name == Carehit.raycastHit.collider.name).Count() == 0) || hit.hits.Where(e => e.collider == null).Count() == itemColliders.Count() * 4)
                    {
                        Carehit.raycastHit.collider.GetComponent<SpriteRenderer>().color = Color.black;
                        Carehit.isDeleted = true;
                    }
                }
            }

            careHits.RemoveAll(e => e.isDeleted == true);
        }
    }
    //--raycasts


    //descriptions--
    void DeleteAllDescriptions()
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
        //ChangeShowStars(true);
        DeleteAllDescriptions();
        CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
    }

    public virtual void EndShowDescription()
    {
        //ChangeShowStars(true);
        Destroy(CanvasDescription.gameObject);
        DeleteAllDescriptions();
    }

    void FindPlaceForDescription()
    {
        switch(itemPlace)
        {
            case ItemPlace.Backpack:
                placeForDescription = GameObject.FindWithTag("DescriptionPlace");
                break;
            case ItemPlace.Storage:
                placeForDescription = GameObject.FindWithTag("DescriptionPlace");
                break;
            case ItemPlace.Shop:
                placeForDescription = GameObject.FindWithTag("DescriptionPlaceEnemy");
                break;
            case ItemPlace.CaveStone:
                placeForDescription = GameObject.FindWithTag("DescriptionPlaceEnemy");
                break;
            case ItemPlace.Warehouse:
                placeForDescription = GameObject.FindWithTag("DescriptionPlaceEnemy");
                break;
        }           
    }
    //--descriptions



}
