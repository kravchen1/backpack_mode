using Steamworks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ItemMove : MonoBehaviour
{
    #region Serialized Fields
    [Header("Stacking Settings")]
    [SerializeField] private bool _isStackable = false;
    [SerializeField] private int _stackCount = 1;
    [SerializeField] private int _maxStackSize = 64;

    [Header("References")]
    [SerializeField] private TextMeshPro _textMeshProCountStack;

    public List<BoxCollider2D> itemColliders = new List<BoxCollider2D>();
    public List<ItemStar> itemStars = new List<ItemStar>();
    #endregion

    #region Private Fields
    // Cache variables
    private Camera _mainCamera;
    private Transform _originalParent;
    private GameObject _playerInventory;
    private GameObject _shopInventory;
    private GameObject _backpackInventory;
    private GameObject _backpackShop;
    private Vector3 _originalPosition;
    private bool _isDragging = false;
    private Vector3 _offset;
    private int _colliderCount;
    private int _previousCountStack = 0;
    private SpriteRenderer _SpriteRenderer;

    // Split item management
    private bool _isSplitItem = false;
    private ItemMove _originalItem = null;
    private ItemMove _splitItem = null; // Для отслеживания разделенного предмета

    // Placement validation
    private bool _canBePlaced = true;
    private readonly List<Cell> _currentGreenCells = new List<Cell>();
    private readonly List<Cell> _currentRedCells = new List<Cell>();
    private readonly List<Cell> _originallyOccupiedCells = new List<Cell>();

    // Color management
    private readonly Dictionary<Collider2D, Color> _originalColors = new Dictionary<Collider2D, Color>();
    private readonly List<Collider2D> _previousHitColliders = new List<Collider2D>();

    // Raycast cache
    private RaycastHit2D[] _raycastHitsCache;

    //Item cache
    private ItemStats _itemStats;

    // Other items cache
    private List<ItemStar> otherItemStarsInInventory;
    #endregion

    #region Constants
    private const int CELL_LAYER = 8;
    private const float RAYCAST_DISTANCE = 0.1f;
    private const float ROTATION_ANGLE = 90f;
    private const int draggingSortingOrder = 5;
    private const int defaultSortingOrder = 2;
    #endregion

    #region Properties
    public bool IsStackable => _isStackable;
    public int StackCount
    {
        get => _stackCount;
        set
        {
            _stackCount = value;
            if (_itemStats != null)
            {
                _itemStats.price = _itemStats.basePrice * _stackCount;
            }
            if (GetComponent<ItemTrade>() != null)
            {
                GetComponent<ItemTrade>().RefreshPrice();
            }
        }
    }
    public int MaxStackSize => _maxStackSize;
    public string ItemName => gameObject.name.Replace("(Clone)", "").Trim();
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        Initialize();
    }

    public virtual void Update()
    {
        UpdateStackVisualization();

        if (_isDragging)
        {
            UpdateDragPosition();
            PerformRaycast();
            HandleRotation();
        }
    }

    void OnDestroy()
    {
        // Если это разделенный предмет, очищаем ссылку у оригинала
        if (_isSplitItem && _originalItem != null)
        {
            _originalItem._splitItem = null;
        }

        // Если это оригинальный предмет, уничтожаем разделенный
        if (_splitItem != null)
        {
            Destroy(_splitItem.gameObject);
        }

        ClearAllCellReferences();
        ResetAllColorsToDefault();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        StackCount = Mathf.Clamp(StackCount, 1, _maxStackSize);
        if (!_isStackable)
        {
            StackCount = 1;
        }
    }
#endif
    #endregion

    #region Input Handlers
    public virtual void OnMouseDown()
    {
        if (DragManager.Instance != null && DragManager.Instance.isDragActive)
        {
            // Обрабатываем разделение стака с учетом разных комбинаций клавиш
            if ((Input.GetKey(KeyCode.LeftShift) || (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl)))
                && _isStackable && StackCount > 1)
            {
                SplitStack();
                return;
            }

            StartDragging();
        }
    }

    public virtual void OnMouseUp()
    {
        if (!_isDragging) return;
        _SpriteRenderer.sortingOrder = defaultSortingOrder;
        if (_textMeshProCountStack != null)
        {
            _textMeshProCountStack.sortingOrder = defaultSortingOrder + 1;
        }
        _isDragging = false;
        if (!IsCursorOverItem())
        {
            OnMouseExit();
        }
        ResetAllColorsToDefault();
        Physics2D.SyncTransforms();

        // Сначала пробуем объединить с другими стаками - это имеет приоритет над размещением
        if (TryMergeWithStackedItem())
        {
            Destroy(gameObject);
            return;
        }

        if (CanBePlaced() && TradeItem())
        {
            CommitPlacement();

            if (_isSplitItem && _originalItem != null)
            {
                _isSplitItem = false;
                _originalItem._splitItem = null;
                _originalItem = null;
            }
        }
        else
        {
            if (_isSplitItem && _originalItem != null)
            {
                ReturnStackToOriginalItem();
                Destroy(gameObject);
            }
            else
            {
                RevertPlacement();
            }
        }

        _previousHitColliders.Clear();
        ClearCurrentCells();
    }

    private void OnMouseEnter()
    {
        if (otherItemStarsInInventory == null)
        {
            FindOtherItemStarsInInventory();
        }

        SetStarsVisibility(true);
    }

    private void OnMouseExit()
    {
        if (!_isDragging)
        {
            SetStarsVisibility(false);
        }
    }
    #endregion

    #region Initialization
    private void Initialize()
    {
        _mainCamera = Camera.main;
        if (GameObject.Find("InventoryData"))
        {
            _playerInventory = GameObject.Find("InventoryData");
        }
        else
        {
            _playerInventory = GameObject.Find("InventoryTradeData");
        }
        _shopInventory = GameObject.Find("ShopData");
        _backpackInventory = GameObject.Find("BackpackInventroy");
        _backpackShop = GameObject.Find("BackpackShop");
        _itemStats = GetComponent<ItemStats>();
        _SpriteRenderer = transform.Find("MainSprite")?.GetComponent<SpriteRenderer>();
        InitializeColliders();
        SaveOriginalState();
    }

    private void InitializeColliders()
    {
        _colliderCount = itemColliders.Count;
        _raycastHitsCache = new RaycastHit2D[_colliderCount];
    }

    private void SaveOriginalState()
    {
        _originalPosition = transform.position;
        _originalParent = transform.parent;
        CacheOriginallyOccupiedCells();
    }

    private void CacheOriginallyOccupiedCells()
    {
        _originallyOccupiedCells.Clear();

        // Оптимизация: кэшируем результат FindObjectsOfType
        var allCells = FindObjectsOfType<Cell>();
        foreach (var cell in allCells)
        {
            if (cell.IsOccupiedBy(gameObject))
            {
                _originallyOccupiedCells.Add(cell);
            }
        }
    }
    #endregion

    #region Dragging Logic
    private void StartDragging()
    {
        _offset = transform.position - GetMouseWorldPosition();
        _isDragging = true;
        _SpriteRenderer.sortingOrder = draggingSortingOrder;
        if (_textMeshProCountStack != null)
        {
            _textMeshProCountStack.sortingOrder = draggingSortingOrder + 1;
        }
        CacheOriginalColors();
        ClearCurrentCells();
        _canBePlaced = true;
        SaveOriginalState();
        ClearAllCellReferences();
        Physics2D.SyncTransforms();
    }

    private void UpdateDragPosition()
    {
        transform.position = GetMouseWorldPosition() + _offset;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -_mainCamera.transform.position.z;
        return _mainCamera.ScreenToWorldPoint(mousePosition);
    }
    #endregion

    #region Stack Management
    private void SplitStack()
    {
        int splitCount = CalculateSplitCount();
        if (splitCount <= 0) return;

        // Если уже есть разделенный предмет, отменяем создание нового
        if (_splitItem != null && _splitItem._isSplitItem)
        {
            // Возвращаем стаки существующему разделенному предмету
            _splitItem.ReturnToOriginalAndDestroy();
            _splitItem = null;
        }

        CacheOriginallyOccupiedCells();
        StackCount -= splitCount;
        UpdateStackVisual();

        GameObject newStack = Instantiate(gameObject, transform.position, Quaternion.identity, transform.parent);
        ItemMove newItemMove = newStack.GetComponent<ItemMove>();

        if (newItemMove != null)
        {
            newItemMove.SaveOriginalState();
            newItemMove.StackCount = splitCount;
            newItemMove._isDragging = true;
            newItemMove._offset = transform.position - GetMouseWorldPosition();
            newItemMove.CacheOriginalColors();
            newItemMove.ClearCurrentCells();
            newItemMove._canBePlaced = true;
            newItemMove._originallyOccupiedCells.Clear();
            newItemMove._isSplitItem = true;
            newItemMove._originalItem = this;

            if (GameObject.Find("InventoryData"))
            {
                newItemMove._playerInventory = GameObject.Find("InventoryData");
            }
            else
            {
                newItemMove._playerInventory = GameObject.Find("InventoryTradeData");
            }
            newItemMove._shopInventory = GameObject.Find("ShopData");
            newItemMove._backpackInventory = GameObject.Find("BackpackInventroy");
            newItemMove._backpackShop = GameObject.Find("BackpackShop");

            // Сохраняем ссылку на разделенный предмет
            _splitItem = newItemMove;

            newItemMove.UpdateStackVisual();

            // Запускаем корутину для обработки перетаскивания
            StartCoroutine(HandleSplitDrag(newItemMove));
        }

        _isDragging = false;
        RestoreOriginallyOccupiedCells();
    }

    /// <summary>
    /// Рассчитывает количество предметов для разделения в зависимости от комбинации клавиш
    /// </summary>
    private int CalculateSplitCount()
    {
        // LShift + LCtrl = 1 предмет
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl))
        {
            return 1;
        }
        // Только LShift = половина стака
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            return Mathf.CeilToInt(StackCount / 2f);
        }

        return 0;
    }

    // Новый метод для возврата и уничтожения разделенного предмета
    private void ReturnToOriginalAndDestroy()
    {
        if (_originalItem != null)
        {
            _originalItem.StackCount += StackCount;
            _originalItem.UpdateStackVisual();
            _originalItem._splitItem = null;
        }
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator HandleSplitDrag(ItemMove draggedItem)
    {
        // Ждем до конца кадра, чтобы все инициализировалось
        yield return null;

        // Теперь обрабатываем перетаскивание в Update
        while (draggedItem._isDragging)
        {
            // Если кнопка мыши отпущена, вызываем OnMouseUp
            if (Input.GetMouseButtonUp(0))
            {
                draggedItem.OnMouseUp();
                yield break;
            }

            // Обновляем позицию предмета
            draggedItem.transform.position = draggedItem.GetMouseWorldPosition() + draggedItem._offset;
            draggedItem.PerformRaycast();

            yield return null;
        }
    }

    private bool TryMergeWithStackedItem()
    {
        if (!_isStackable) return false;

        ItemMove targetItem = FindStackableItemUnderMouse();
        if (targetItem == null || targetItem == this || targetItem.ItemName != ItemName) return false;

        int availableSpace = targetItem._maxStackSize - targetItem.StackCount;
        if (availableSpace <= 0) return false;

        int amountToTransfer = Mathf.Min(StackCount, availableSpace);

        // СОХРАНЯЕМ ИНФОРМАЦИЮ О ПЕРЕНОСЕ ДЛЯ ТОРГОВЛИ И ВЕСА
        bool wasInPlayerInventory = IsInPlayerInventory();
        bool targetInPlayerInventory = targetItem.IsInPlayerInventory();

        targetItem.StackCount += amountToTransfer;
        targetItem.UpdateStackVisual();

        // ПРАВИЛЬНОЕ ОБНОВЛЕНИЕ ВЕСА И ДЕНЕГ
        HandleStackMergeTradeAndWeight(amountToTransfer, wasInPlayerInventory, targetInPlayerInventory, targetItem);

        StackCount -= amountToTransfer;

        if (StackCount <= 0)
        {
            if (_isSplitItem && _originalItem != null)
            {
                _originalItem._splitItem = null;
            }

            // ЕСЛИ ПРЕДМЕТ ПОЛНОСТЬЮ ПЕРЕМЕЩЕН, ВЫЗЫВАЕМ ДОПОЛНИТЕЛЬНУЮ ЛОГИКУ
            if (wasInPlayerInventory != targetInPlayerInventory)
            {
                HandleCompleteStackTransfer(wasInPlayerInventory, targetInPlayerInventory);
            }

            return true;
        }

        UpdateStackVisual();
        return false;
    }

    /// <summary>
    /// Обрабатывает логику торговли и веса при объединении стаков
    /// </summary>
    private void HandleStackMergeTradeAndWeight(int transferredAmount, bool wasInPlayerInventory,
        bool targetInPlayerInventory, ItemMove targetItem)
    {
        // Если предмет перемещается ИЗ инвентаря игрока В магазин
        if (wasInPlayerInventory && !targetInPlayerInventory)
        {
            // Продажа части стака
            float salePrice = CalculatePartialPrice(transferredAmount);
            PlayerDataManager.Instance.Stats.Money += salePrice;
            Debug.Log($"Sold {transferredAmount} items for {salePrice}");
        }
        // Если предмет перемещается ИЗ магазина В инвентарь игрока
        else if (!wasInPlayerInventory && targetInPlayerInventory)
        {
            // Покупка части стака
            float purchasePrice = CalculatePartialPrice(transferredAmount);
            if (PlayerDataManager.Instance.Stats.Money >= purchasePrice)
            {
                PlayerDataManager.Instance.Stats.Money -= purchasePrice;

                Debug.Log($"Purchased {transferredAmount} items for {purchasePrice}");
            }
            else
            {
                // Откатываем транзакцию если недостаточно денег
                targetItem.StackCount -= transferredAmount;
                StackCount += transferredAmount;
                targetItem.UpdateStackVisual();
                UpdateStackVisual();
            }
        }
        // Если оба предмета в одной зоне (только обновляем вес если нужно)
        else if (wasInPlayerInventory && targetInPlayerInventory)
        {
            // Вес автоматически корректируется через AddWeightToPlayer/RemoveWeightFromPlayer
            // так как targetItem уже в инвентаре игрока
        }
    }

    private ItemMove FindStackableItemUnderMouse()
    {
        Vector2 mousePos = GetMouseWorldPosition();

        // Используем более точный поиск всех предметов под курсором
        var hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                ItemMove item = hit.collider.GetComponentInParent<ItemMove>();

                // Игнорируем этот предмет и проверяем, что нашли другой стакуемый предмет
                if (item != null && item != this && item.ItemName == ItemName && item.IsStackable)
                {
                    return item;
                }
            }
        }

        return null;
    }

    private void ReturnStackToOriginalItem()
    {
        if (_originalItem != null)
        {
            // Возвращаем стаки оригинальному предмету
            _originalItem.StackCount += StackCount;
            _originalItem.UpdateStackVisual();
            _originalItem._splitItem = null; // Очищаем ссылку
        }
    }

    private void UpdateStackVisualization()
    {
        if (!_isStackable || _previousCountStack == StackCount) return;

        _previousCountStack = StackCount;

        if (_textMeshProCountStack != null)
        {
            _textMeshProCountStack.text = StackCount > 1 ? StackCount.ToString() : string.Empty;
        }

        // Оптимизация: обновляем прозрачность только если изменилось состояние стека
        var renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color color = renderer.color;
            color.a = StackCount > 1 ? 0.9f : 1.0f;
            renderer.color = color;
        }
    }

    public void UpdateStackVisual()
    {
        UpdateStackVisualization();
    }

    public bool CanAddToStack(int amount)
    {
        return _isStackable && (StackCount + amount) <= _maxStackSize;
    }

    public void AddToStack(int amount)
    {
        if (_isStackable)
        {
            StackCount = Mathf.Min(StackCount + amount, _maxStackSize);
            UpdateStackVisual();
        }
    }
    #endregion

    #region Placement & Collision Detection
    private void PerformRaycast()
    {
        var hits = CreatePreciseRaycast();
        ValidatePlacement(hits);
        UpdateCellColors(hits);
        UpdateColliderTracking(hits);
    }

    private RaycastHit2D[] CreatePreciseRaycast()
    {
        for (int i = 0; i < _colliderCount; i++)
        {
            if (itemColliders[i] == null) continue;

            Vector2 rayOrigin = itemColliders[i].bounds.center;
            _raycastHitsCache[i] = Physics2D.Raycast(rayOrigin, Vector2.down, RAYCAST_DISTANCE, 1 << CELL_LAYER);
        }
        return _raycastHitsCache;
    }

    private void ValidatePlacement(RaycastHit2D[] hits)
    {
        _canBePlaced = hits.All(hit => hit.collider != null) &&
                      hits.All(hit =>
                      {
                          var cell = hit.collider.GetComponent<Cell>();
                          return cell == null || !cell.IsOccupied || cell.IsOccupiedBy(gameObject);
                      });
    }

    private void UpdateCellColors(RaycastHit2D[] hits)
    {
        ClearCurrentCells();

        for (int i = 0; i < _colliderCount; i++)
        {
            if (hits[i].collider == null) continue;

            var cell = hits[i].collider.GetComponent<Cell>();
            var renderer = hits[i].collider.GetComponent<SpriteRenderer>();

            if (cell == null || renderer == null) continue;

            if (cell.IsOccupied && !cell.IsOccupiedBy(gameObject))
            {
                renderer.color = Color.red;
                _currentRedCells.Add(cell);
            }
            else
            {
                renderer.color = Color.green;
                _currentGreenCells.Add(cell);
            }
        }
    }

    private void UpdateColliderTracking(RaycastHit2D[] currentHits)
    {
        // Оптимизация: используем HashSet для быстрого поиска
        var currentColliders = new HashSet<Collider2D>(
            currentHits.Where(hit => hit.collider != null)
                      .Select(hit => hit.collider)
        );

        // Удаляем старые коллайдеры
        for (int i = _previousHitColliders.Count - 1; i >= 0; i--)
        {
            var collider = _previousHitColliders[i];
            if (!currentColliders.Contains(collider))
            {
                ResetColliderColor(collider);
                _previousHitColliders.RemoveAt(i);
            }
        }

        // Добавляем новые коллайдеры
        foreach (var collider in currentColliders)
        {
            if (!_previousHitColliders.Contains(collider))
            {
                _previousHitColliders.Add(collider);
            }
        }
    }

    private bool CanBePlaced()
    {
        return _canBePlaced && _currentGreenCells.Count > 0 && _currentRedCells.Count == 0;
    }

    private void CommitPlacement()
    {
        FillCellNestedObjects();
        CorrectPosition();
        MoveToInventory();
        CacheOriginallyOccupiedCells();
    }

    private void RevertPlacement()
    {
        RestoreOriginallyOccupiedCells();
        ReturnToOriginalPosition();
    }

    private void FillCellNestedObjects()
    {
        foreach (var cell in _currentGreenCells)
        {
            if (cell != null)
            {
                cell.NestedObject = gameObject;
            }
        }
    }

    public virtual void CorrectPosition()
    {
        if (_currentGreenCells.Count == 0) return;

        Bounds cellsBounds = new Bounds(_currentGreenCells[0].transform.position, Vector3.zero);
        foreach (Cell cell in _currentGreenCells)
        {
            cellsBounds.Encapsulate(cell.transform.position);
        }

        Bounds itemBounds = new Bounds(itemColliders[0].bounds.center, Vector3.zero);
        foreach (var collider in itemColliders)
        {
            itemBounds.Encapsulate(collider.bounds);
        }

        Vector3 centerOffset = cellsBounds.center - itemBounds.center;
        transform.position += centerOffset;
    }

    public void ForceCorrectPosition()
    {
        CorrectPosition();
    }

    private void ReturnToOriginalPosition()
    {
        transform.SetParent(_originalParent);
        transform.position = _originalPosition;
        transform.rotation = Quaternion.identity;
    }

    private void MoveToInventory()
    {
        if (_currentGreenCells[0].transform.parent.gameObject == _backpackInventory)
        {
            transform.SetParent(_playerInventory.transform);
            ActivateItemAction();
        }
        else
        {
            transform.SetParent(_shopInventory.transform);
            DeActivateItemAction();
        }
    }

    private void RestoreOriginallyOccupiedCells()
    {
        ClearAllCellReferences();

        foreach (var cell in _originallyOccupiedCells.Where(cell => cell != null))
        {
            cell.NestedObject = gameObject;
        }
    }

    private void ClearAllCellReferences()
    {
        // Оптимизация: работаем только с ячейками, которые действительно содержат этот объект
        foreach (var cell in _originallyOccupiedCells.Where(cell => cell != null && cell.IsOccupiedBy(gameObject)))
        {
            cell.NestedObject = null;
        }
    }

    private void ClearCurrentCells()
    {
        _currentGreenCells.Clear();
        _currentRedCells.Clear();
    }
    #endregion

    #region Rotation
    private void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateItem(ROTATION_ANGLE);
        }
        else
        {
            float scrollData = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollData) > 0.01f)
            {
                RotateItem(Mathf.Sign(scrollData) * ROTATION_ANGLE);
            }
        }
    }

    private void RotateItem(float angle)
    {
        ResetAllColorsToDefault();
        transform.Rotate(0, 0, angle);
        Physics2D.SyncTransforms();

        // Немедленное обновление рейкаста после вращения
        var hits = CreatePreciseRaycast();
        ValidatePlacement(hits);
        UpdateCellColors(hits);
        UpdateColliderTracking(hits);
    }
    #endregion

    #region Color Management
    private void CacheOriginalColors()
    {
        _originalColors.Clear();
        var allColliders = FindObjectsOfType<Collider2D>().Where(c => c.gameObject.layer == CELL_LAYER);

        foreach (var collider in allColliders)
        {
            var renderer = collider.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                _originalColors[collider] = renderer.color;
            }
        }
    }

    private void ResetColliderColor(Collider2D collider)
    {
        if (collider == null) return;

        var renderer = collider.GetComponent<SpriteRenderer>();
        if (renderer == null) return;

        if (_originalColors.TryGetValue(collider, out Color originalColor))
        {
            renderer.color = originalColor;
        }
        else
        {
            renderer.color = Color.white;
        }
    }

    private void ResetAllColorsToDefault()
    {
        foreach (var kvp in _originalColors.Where(kvp => kvp.Key != null))
        {
            var renderer = kvp.Key.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = kvp.Value;
            }
        }

        foreach (var collider in _previousHitColliders.Where(c => c != null))
        {
            ResetColliderColor(collider);
        }

        _previousHitColliders.Clear();
        _originalColors.Clear();
    }
    #endregion

    #region Trade & Economy
    private bool TradeItem()
    {
        if (_currentGreenCells[0].transform.parent.gameObject == _backpackInventory && _originalParent.gameObject != _playerInventory)
        {
            if (PurchaseItem())
            {
                // УДАЛЯЕМ ВСЕ СВЯЗАННЫЕ КОПИИ ПРЕДМЕТА
                RemoveAllLinkedCopies();
                return true;
            }
            else
            {
                return false;
            }
        }

        if (_currentGreenCells[0].transform.parent.gameObject != _backpackInventory && _originalParent.gameObject == _playerInventory)
        {
            SaleItem();
            return true;
        }

        return true;
    }

    private bool PurchaseItem()
    {
        if (GetComponent<ItemTrade>() != null)
        {
            var price = GetComponent<ItemStats>().price;
            if (PlayerDataManager.Instance.Stats.Money >= price)
            {
                PlayerDataManager.Instance.Stats.Money -= price;
                return true;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    private void SaleItem()
    {
        if (GetComponent<ItemTrade>() != null)
        {
            var price = GetComponent<ItemStats>().price;
            PlayerDataManager.Instance.Stats.Money += price;
        }
    }

    /// <summary>
    /// Рассчитывает цену для части стака
    /// </summary>
    private float CalculatePartialPrice(int amount)
    {
        if (_itemStats == null) return 0;

        float pricePerItem = _itemStats.basePrice;
        return pricePerItem * amount;
    }

    /// <summary>
    /// Обрабатывает полное перемещение стака между инвентарями
    /// </summary>
    private void HandleCompleteStackTransfer(bool wasInPlayerInventory, bool targetInPlayerInventory)
    {
        if (wasInPlayerInventory && !targetInPlayerInventory)
        {
            // Полная продажа предмета
            DeActivateItemAction();
            RemoveAllLinkedCopies();
        }
        else if (!wasInPlayerInventory && targetInPlayerInventory)
        {
            // Полная покупка предмета
            ActivateItemAction();
        }
    }
    #endregion

    #region Item Actions & Effects
    private void ActivateItemAction()
    {
        if (GetComponent<ItemActionInfluenceWorldController>() != null)
        {
            GetComponent<ItemActionInfluenceWorldController>().InfluenceOnThePlayer();
        }
    }

    private void DeActivateItemAction()
    {
        if (GetComponent<ItemActionInfluenceWorldController>() != null)
        {
            GetComponent<ItemActionInfluenceWorldController>().ReverseInfluenceOnThePlayer();
        }
    }

    /// <summary>
    /// Удаляет все копии предмета через систему ссылок
    /// </summary>
    private void RemoveAllLinkedCopies()
    {
        // Находим TradeGenerator для использования его методов
        TradeController tradeGenerator = FindObjectOfType<TradeController>();
        if (tradeGenerator != null)
        {
            tradeGenerator.RemoveAllLinkedCopies(gameObject);
        }
    }
    #endregion

    #region Weight Management
    /// <summary>
    /// Проверить, находится ли предмет в инвентаре
    /// </summary>
    private bool IsPlacedInInventory()
    {
        return transform.parent == _playerInventory.transform || IsInInventoryArea();
    }

    private bool IsInInventoryArea()
    {
        // Проверяем родителя на наличие тега InventoryPlayer
        if (transform.parent != null)
        {
            return transform.parent.CompareTag("InventoryPlayer");
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Теоретический метод для удаления предмета со сцены с учетом веса
    /// </summary>
    public void RemoveItemFromScene()
    {
        // Очищаем ссылки в ячейках
        ClearAllCellReferences();

        // Уничтожаем объект
        Destroy(gameObject);
    }
    #endregion

    #region Star System
    private void FindOtherItemStarsInInventory()
    {
        var itemStars = GameObject.FindGameObjectsWithTag("Star").Where(e => HasMatchingItemType(e.GetComponent<ItemStar>().AllowedItemTypes));
        if (itemStars.Any())
        {
            otherItemStarsInInventory = new List<ItemStar>();
        }
        foreach (var itemStar in itemStars)
        {
            otherItemStarsInInventory.Add(itemStar.GetComponent<ItemStar>());
        }
    }

    private bool HasMatchingItemType(List<ItemType> itemTypesToCheck)
    {
        foreach (var itemType in itemTypesToCheck)
        {
            if (_itemStats.itemTypes.Contains(itemType))
                return true;
        }

        return false;
    }

    private void SetStarsVisibility(bool visible)
    {
        foreach (var star in itemStars)
        {
            if (star != null)
            {
                star.SetStarEnabled(visible);
            }
        }
        if (otherItemStarsInInventory != null)
        {
            foreach (var star in otherItemStarsInInventory)
            {
                if (star != null)
                {
                    star.SetStarEnabled(visible);
                }
            }
        }
    }

    public void StarsPerformRaycastCheck()
    {
        foreach (var star in itemStars)
        {
            star.PerformRaycastCheck();
        }
    }

    private bool IsCursorOverItem()
    {
        Vector2 mousePosition = GetMouseWorldPosition();
        PolygonCollider2D polygonCollider2DTemp = GetComponent<PolygonCollider2D>();
        if (polygonCollider2DTemp != null)
        {
            return GetComponent<PolygonCollider2D>().OverlapPoint(mousePosition);
        }
        else
            return false;
    }

    /// <summary>
    /// Проверяет, находится ли предмет в инвентаре игрока
    /// </summary>
    private bool IsInPlayerInventory()
    {
        return transform.parent == _playerInventory.transform ||
               (_currentGreenCells.Count > 0 && _currentGreenCells[0].transform.parent.gameObject == _backpackInventory);
    }
    #endregion

    #region Utility Methods
    private Bounds CalculateItemBounds()
    {
        var bounds = new Bounds();
        bool hasBounds = false;

        foreach (var collider in itemColliders.Where(c => c != null))
        {
            if (!hasBounds)
            {
                bounds = new Bounds(collider.bounds.center, Vector3.zero);
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(collider.bounds);
            }
        }

        return bounds;
    }

    private ItemMove FindOriginalItemUnderMouse()
    {
        // Ищем предметы в оригинальной позиции или рядом
        Vector2 originalPos = _originalPosition;
        var hit = Physics2D.Raycast(originalPos, Vector2.zero);

        if (hit.collider != null)
        {
            return hit.collider.GetComponentInParent<ItemMove>();
        }

        // Если не нашли рейкастом, ищем по близости
        var allItems = FindObjectsOfType<ItemMove>();
        foreach (var item in allItems)
        {
            if (item != this && item.ItemName == ItemName &&
                Vector2.Distance(item.transform.position, originalPos) < 0.5f)
            {
                return item;
            }
        }

        return null;
    }
    #endregion
}