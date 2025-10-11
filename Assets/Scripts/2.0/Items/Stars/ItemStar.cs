using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemStar : MonoBehaviour
{
    [SerializeField] protected LayerMask _raycastMask = 1 << 9;
    [SerializeField] protected List<ItemType> _allowedItemTypes = new List<ItemType>();
    [SerializeField] protected List<ItemRarity> _allowedItemRarities = new List<ItemRarity>();

    [Header("References")]
    [SerializeField] protected GameObject _starEmpty;
    [SerializeField] protected GameObject _starFill;

    protected BoxCollider2D _boxCollider;
    //protected Transform _playerInventory;
    [SerializeField] protected GameObject _currentItem;
    protected bool _isStarEnabled = false;

    // Флаг для отслеживания текущего состояния применения модификаций
    protected bool _isModificationApplied = false;

    public GameObject CurrentItem => _currentItem;
    public List<ItemType> AllowedItemTypes => _allowedItemTypes;

    protected void Awake()
    {
        Initialize();
    }

    protected void Initialize()
    {
        _boxCollider = GetComponent<BoxCollider2D>();

        if (_starEmpty == null || _starFill == null)
        {
            CacheStarReferences();
        }

        //FindPlayerInventory();
        SetVisualsState(false);
    }

    protected void CacheStarReferences()
    {
        if (transform.childCount >= 2)
        {
            _starEmpty = transform.GetChild(0).gameObject;
            _starFill = transform.GetChild(1).gameObject;
        }
        else
        {
            Debug.LogError($"ItemStar on {gameObject.name} requires at least 2 child objects", this);
        }
    }

    //protected void FindPlayerInventory()
    //{
    //    _playerInventory = GameObject.Find("InventoryData")?.transform;
    //    if (_playerInventory == null)
    //    {
    //        Debug.LogWarning("InventoryData not found in scene", this);
    //    }
    //}

    protected void FixedUpdate()
    {
        if (_isStarEnabled)
        {
            PerformRaycastCheck();
            UpdateStarVisuals();
        }
    }

    public void PerformRaycastCheck()
    {
        var hit = Physics2D.Raycast(_boxCollider.bounds.center, Vector2.zero, 0f, _raycastMask);

        if (hit.collider != null)
        {
            var itemStat = hit.collider.gameObject;
            if (itemStat != null && IsValidItem(itemStat))
            {
                GameObject newItem = itemStat.transform.parent.gameObject;

                // Если предмет изменился или модификация еще не применялась
                if (_currentItem != newItem || !_isModificationApplied)
                {
                    // Если был предыдущий предмет - отменяем его модификации
                    if (_currentItem != null && _currentItem != newItem)
                    {
                        StarActionDisable();
                        
                    }

                    // Применяем модификации к новому предмету
                    _currentItem = newItem;
                    if (_currentItem.GetComponent<ItemActionModifyController>() != null)
                    {
                        StarActionEnable();
                        _isModificationApplied = true;
                    }
                }
                return;
            }
        }

        // Если предмета нет, но модификация была применена - отменяем
        if (_isModificationApplied)
        {
            StarActionDisable();
            _isModificationApplied = false;
        }
        _currentItem = null;
    }

    protected virtual void StarActionEnable()
    {

    }

    protected virtual void StarActionDisable()
    {

    }

    protected bool IsValidItem(GameObject itemObject)
    {
        if (itemObject == null) return false;

        var itemStructure = itemObject.GetComponentInParent<ItemStats>();
        if (itemStructure == null) return false;

        bool typeValid = _allowedItemTypes.Count == 0 || HasMatchingItemType(itemStructure.itemTypes);
        bool rarityValid = _allowedItemRarities.Count == 0 || _allowedItemRarities.Contains(itemStructure.itemRarity);

        return typeValid && rarityValid;
    }

    protected bool HasMatchingItemType(List<ItemType> itemTypesToCheck)
    {
        if (_allowedItemTypes.Count == 0) return true;

        foreach (var itemType in itemTypesToCheck)
        {
            if (_allowedItemTypes.Contains(itemType))
                return true;
        }

        return false;
    }

    protected void UpdateStarVisuals()
    {
        bool hasValidItem = _currentItem != null;

        if (_starFill != null) _starFill.SetActive(hasValidItem);
        if (_starEmpty != null) _starEmpty.SetActive(!hasValidItem);
    }

    private void SetVisualsState(bool enabled)
    {
        if (_starFill != null) _starFill.SetActive(enabled);
        if (_starEmpty != null) _starEmpty.SetActive(enabled);
    }

    public void SetStarEnabled(bool enabled)
    {
        _isStarEnabled = enabled;

        if (!_isStarEnabled)
        {
            //// Отменяем модификации при выключении звезды
            //if (_currentItem != null && _isModificationApplied)
            //{
            //    _currentItem.GetComponent<ItemActionModifyController>().ModifyDisableItem(gameObject.transform.parent.parent.gameObject);
            //    _isModificationApplied = false;
            //}
            SetVisualsState(false);
        }
        else
        {
            UpdateStarVisuals();
        }
    }

    // Editor validation
#if UNITY_EDITOR
    protected void OnValidate()
    {
        if (_starEmpty == null || _starFill == null)
        {
            CacheStarReferences();
        }
    }
#endif
}