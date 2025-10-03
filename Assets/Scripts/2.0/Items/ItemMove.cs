using Steamworks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ItemMove : MonoBehaviour
{
    [Header("Stacking Settings")]
    [SerializeField] private bool _isStackable = false;
    [SerializeField] private int _stackCount = 1;
    [SerializeField] private int _maxStackSize = 64;

    [Header("References")]
    [SerializeField] private TextMeshPro _textMeshProCountStack;

    public List<BoxCollider2D> itemColliders = new List<BoxCollider2D>();
    public List<ItemStar> itemStars = new List<ItemStar>();

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

    // Properties
    public bool IsStackable => _isStackable;
    public int StackCount => _stackCount;
    public int MaxStackSize => _maxStackSize;
    public string ItemName => gameObject.name.Replace("(Clone)", "").Trim();

    // Constants
    private const int CELL_LAYER = 8;
    private const float RAYCAST_DISTANCE = 0.1f;
    private const float ROTATION_ANGLE = 90f;

    void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _mainCamera = Camera.main;
        _playerInventory = GameObject.Find("InventoryData");
        _shopInventory = GameObject.Find("ShopData");
        _backpackInventory = GameObject.Find("BackpackInventroy");
        _backpackShop = GameObject.Find("BackpackShop");
        _itemStats = GetComponent<ItemStats>();
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
        Debug.Log(_originalParent);
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

    public virtual void OnMouseDown()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _isStackable && _stackCount > 1)
        {
            SplitStack();
            return;
        }

        StartDragging();
    }


    private void SplitStack()
    {
        int halfCount = Mathf.CeilToInt(_stackCount / 2f);
        int remainingCount = _stackCount - halfCount;

        if (halfCount <= 0) return;

        // Если уже есть разделенный предмет, отменяем создание нового
        if (_splitItem != null && _splitItem._isSplitItem)
        {
            // Возвращаем стаки существующему разделенному предмету
            _splitItem.ReturnToOriginalAndDestroy();
            _splitItem = null;
        }

        CacheOriginallyOccupiedCells();
        _stackCount = remainingCount;
        UpdateStackVisual();

        GameObject newStack = Instantiate(gameObject, transform.position, Quaternion.identity, transform.parent);
        ItemMove newItemMove = newStack.GetComponent<ItemMove>();

        if (newItemMove != null)
        {
            newItemMove.SaveOriginalState();
            newItemMove._stackCount = halfCount;
            newItemMove._isDragging = true;
            newItemMove._offset = transform.position - GetMouseWorldPosition();
            newItemMove.CacheOriginalColors();
            newItemMove.ClearCurrentCells();
            newItemMove._canBePlaced = true;
            newItemMove._originallyOccupiedCells.Clear();
            newItemMove._isSplitItem = true;
            newItemMove._originalItem = this;

            newItemMove._playerInventory = GameObject.Find("InventoryData");
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

    // Новый метод для возврата и уничтожения разделенного предмета
    private void ReturnToOriginalAndDestroy()
    {
        if (_originalItem != null)
        {
            _originalItem._stackCount += _stackCount;
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


    private void StartDragging()
    {
        _offset = transform.position - GetMouseWorldPosition();
        _isDragging = true;

        CacheOriginalColors();
        ClearCurrentCells();
        _canBePlaced = true;
        SaveOriginalState();
        ClearAllCellReferences();
        Physics2D.SyncTransforms();
    }

    private List<ItemStar> otherItemStarsInInventory;
    private void OnMouseEnter()
    {
        if(otherItemStarsInInventory == null)
        {
            FindOtherItemStarsInInventory();
        }

        SetStarsVisibility(true);

    }

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

    private void OnMouseExit()
    {
        if (!_isDragging)
        {
            SetStarsVisibility(false);
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

        //itemColliders.Any(collider =>
        //collider != null && collider.OverlapPoint(mousePosition));
    }

    public virtual void OnMouseUp()
    {
        if (!_isDragging) return;

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

        if (CanBePlaced())
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

    private void ReturnStackToOriginalItem()
    {
        if (_originalItem != null)
        {
            // Возвращаем стаки оригинальному предмету
            _originalItem._stackCount += _stackCount;
            _originalItem.UpdateStackVisual();
            _originalItem._splitItem = null; // Очищаем ссылку
        }
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

    private bool TryMergeWithStackedItem()
    {
        if (!_isStackable) return false;

        ItemMove targetItem = FindStackableItemUnderMouse();

        // Проверяем, что targetItem существует, не является этим же объектом, и имена совпадают
        if (targetItem == null || targetItem == this || targetItem.ItemName != ItemName) return false;

        // Разрешаем объединение разделенного предмета с оригиналом и наоборот
        // Убираем блокировку объединения между связанными предметами

        int availableSpace = targetItem._maxStackSize - targetItem._stackCount;
        if (availableSpace <= 0) return false;

        int amountToTransfer = Mathf.Min(_stackCount, availableSpace);
        targetItem._stackCount += amountToTransfer;
        targetItem.UpdateStackVisual();

        ChangeWeightStackable();

        _stackCount -= amountToTransfer;

        // Если стаки полностью перенесены, уничтожаем этот предмет
        if (_stackCount <= 0)
        {
            // Если это разделенный предмет, очищаем ссылку у оригинала
            if (_isSplitItem && _originalItem != null)
            {
                _originalItem._splitItem = null;
            }
            return true;
        }

        return false;
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

    private void UpdateStackVisualization()
    {
        if (!_isStackable || _previousCountStack == _stackCount) return;

        _previousCountStack = _stackCount;

        if (_textMeshProCountStack != null)
        {
            _textMeshProCountStack.text = _stackCount > 1 ? _stackCount.ToString() : string.Empty;
        }

        // Оптимизация: обновляем прозрачность только если изменилось состояние стека
        var renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color color = renderer.color;
            color.a = _stackCount > 1 ? 0.9f : 1.0f;
            renderer.color = color;
        }
    }

    private void UpdateDragPosition()
    {
        transform.position = GetMouseWorldPosition() + _offset;
    }

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
        }
        else
        {
            transform.SetParent(_shopInventory.transform);
        }
        ChangeWeight();
    }

    private void ChangeWeight()
    {
        if (_currentGreenCells[0].transform.parent.gameObject == _backpackInventory)
        {
            if (_originalParent != _playerInventory.transform)
            {
                AddWeightToPlayer();
            }
        }
        else
        {
            if (_originalParent == _playerInventory.transform)
            {
                RemoveWeightFromPlayer();
            }
        }
    }

    private void ChangeWeightStackable()
    {
        if (_currentGreenCells.Count > 0)
        {
            if (_currentGreenCells[0].transform.parent.gameObject == _backpackInventory)
            {
                if (_originalParent != _playerInventory.transform)
                {
                    AddWeightToPlayer();
                }
            }
            else
            {
                if (_originalParent == _playerInventory.transform)
                {
                    RemoveWeightFromPlayer();
                }
            }
        }
        else
        {
            if (_currentRedCells[0].transform.parent.gameObject == _backpackInventory)
            {
                if (_originalParent != _playerInventory.transform)
                {
                    AddWeightToPlayer();
                }
            }
            else
            {
                if (_originalParent == _playerInventory.transform)
                {
                    RemoveWeightFromPlayer();
                }
            }
        }
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

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -_mainCamera.transform.position.z;
        return _mainCamera.ScreenToWorldPoint(mousePosition);
    }

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

    public void UpdateStackVisual()
    {
        UpdateStackVisualization();
    }

    public void ForceCorrectPosition()
    {
        CorrectPosition();
    }

    public bool CanAddToStack(int amount)
    {
        return _isStackable && (_stackCount + amount) <= _maxStackSize;
    }

    public void AddToStack(int amount)
    {
        if (_isStackable)
        {
            _stackCount = Mathf.Min(_stackCount + amount, _maxStackSize);
            UpdateStackVisual();
        }
    }

    // ==================== WEIGHT MANAGEMENT METHODS ====================

    /// <summary>
    /// Получить общий вес предмета с учетом стаков
    /// </summary>
    private float GetTotalWeight()
    {
        if (_itemStats == null) return 0f;

        float singleItemWeight = _itemStats.weight;
        return singleItemWeight * _stackCount;
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
    /// Проверить, находится ли предмет в инвентаре
    /// </summary>
    private bool IsPlacedInInventory()
    {
        return transform.parent == _playerInventory.transform || IsInInventoryArea();
    }

    /// <summary>
    /// Добавить вес предмета к игроку
    /// </summary>
    private void AddWeightToPlayer(int specificStackCount = -1)
    {
        if (PlayerDataManager.Instance == null || PlayerDataManager.Instance.Stats == null) return;

        float weightToAdd = GetTotalWeight();
        if (specificStackCount > 0)
        {
            // Для частичного добавления веса (при объединении стаков)
            float singleItemWeight = _itemStats != null ? _itemStats.weight : 0f;
            weightToAdd = singleItemWeight * specificStackCount;
        }

        PlayerDataManager.Instance.Stats.CurrentWeight += weightToAdd;

        Debug.Log($"Added weight: {weightToAdd}. Total weight: {PlayerDataManager.Instance.Stats.CurrentWeight}");
    }

    /// <summary>
    /// Убрать вес предмета у игрока
    /// </summary>
    private void RemoveWeightFromPlayer(int specificStackCount = -1)
    {
        if (PlayerDataManager.Instance == null || PlayerDataManager.Instance.Stats == null) return;

        float weightToRemove = GetTotalWeight();
        if (specificStackCount > 0)
        {
            // Для частичного удаления веса (при объединении стаков)
            float singleItemWeight = _itemStats != null ? _itemStats.weight : 0f;
            weightToRemove = singleItemWeight * specificStackCount;
        }

        PlayerDataManager.Instance.Stats.CurrentWeight -= weightToRemove;

        Debug.Log($"Removed weight: {weightToRemove}. Total weight: {PlayerDataManager.Instance.Stats.CurrentWeight}");
    }

    /// <summary>
    /// Теоретический метод для удаления предмета со сцены с учетом веса
    /// </summary>
    public void RemoveItemFromScene()
    {
        // Убираем вес если предмет был в инвентаре
        if (IsInInventoryArea())
        {
            RemoveWeightFromPlayer();
        }

        // Очищаем ссылки в ячейках
        ClearAllCellReferences();

        // Уничтожаем объект
        Destroy(gameObject);
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
        _stackCount = Mathf.Clamp(_stackCount, 1, _maxStackSize);
        if (!_isStackable)
        {
            _stackCount = 1;
        }
    }


#endif
}