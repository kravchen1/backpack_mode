using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemStar : MonoBehaviour
{
    [SerializeField] private LayerMask _raycastMask = 1 << 8;
    [SerializeField] private List<ItemType> _allowedItemTypes = new List<ItemType>();
    [SerializeField] private List<ItemRarity> _allowedItemRarities = new List<ItemRarity>();

    [Header("References")]
    [SerializeField] private GameObject _starEmpty;
    [SerializeField] private GameObject _starFill;

    private BoxCollider2D _boxCollider;
    private Transform _playerInventory;
    [SerializeField] private GameObject _currentItem;
    private bool _isStarEnabled = false;

    public GameObject CurrentItem => _currentItem;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _boxCollider = GetComponent<BoxCollider2D>();

        if (_starEmpty == null || _starFill == null)
        {
            CacheStarReferences();
        }

        FindPlayerInventory();
        SetVisualsState(false);
    }

    private void CacheStarReferences()
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

    private void FindPlayerInventory()
    {
        _playerInventory = GameObject.Find("InventoryData")?.transform;
        if (_playerInventory == null)
        {
            Debug.LogWarning("InventoryData not found in scene", this);
        }
    }

    private void FixedUpdate()
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
            var cell = hit.collider.GetComponent<Cell>();
            if (cell != null && IsValidItem(cell.NestedObject))
            {
                _currentItem = cell.NestedObject;
                return;
            }
        }
        _currentItem = null;
    }

    private bool IsValidItem(GameObject itemObject)
    {
        if (itemObject == null) return false;

        var itemStructure = itemObject.GetComponent<ItemStats>();
        if (itemStructure == null) return false;

        bool typeValid = _allowedItemTypes.Count == 0 || HasMatchingItemType(itemStructure.itemTypes);
        bool rarityValid = _allowedItemRarities.Count == 0 || _allowedItemRarities.Contains(itemStructure.itemRarity);

        return typeValid && rarityValid;
    }

    private bool HasMatchingItemType(List<ItemType> itemTypesToCheck)
    {
        if (_allowedItemTypes.Count == 0) return true;

        foreach (var itemType in itemTypesToCheck)
        {
            if (_allowedItemTypes.Contains(itemType))
                return true;
        }

        return false;
    }

    private void UpdateStarVisuals()
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
            //_currentItem = null;
            SetVisualsState(false);
        }
        else
        {
            UpdateStarVisuals();
        }
    }

    // Editor validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_starEmpty == null || _starFill == null)
        {
            CacheStarReferences();
        }
    }
#endif
}