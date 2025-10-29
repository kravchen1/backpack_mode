using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TradeController : MonoBehaviour
{
    #region Serialized Fields
    [Header("Trade References")]
    public GameObject tradeCanvas;
    public GameObject tradeCanvasAll;
    public GameObject tradeCanvasWeapon;
    public GameObject tradeCanvasArmor;
    public GameObject tradeCanvasPatrons;
    public GameObject tradeCanvasWeaponMods;
    public GameObject tradeCanvasOther;

    [Header("Item Generation")]
    public int maxShopItems = 30;

    [Header("Dependencies")]
    public CellsData tradeCellsData;
    #endregion

    #region Private Variables
    private List<Cell> tradeCanvasAllCells = new List<Cell>();
    private List<Cell> tradeCanvasWeaponCells = new List<Cell>();
    private List<Cell> tradeCanvasArmorCells = new List<Cell>();
    private List<Cell> tradeCanvasPatronsCells = new List<Cell>();
    private List<Cell> tradeCanvasWeaponModsCells = new List<Cell>();
    private List<Cell> tradeCanvasOtherCells = new List<Cell>();

    private List<ItemStructure> spawnedItems = new List<ItemStructure>();
    private const int GridWidth = 10;
    private const int GridHeight = 10;

    [HideInInspector] public List<GameObject> itemPrefabs = new List<GameObject>();

    private Dictionary<ItemType, GameObject> typeToCanvasMap;
    private Dictionary<ItemType, List<Cell>> typeToCellsMap;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        itemPrefabs = GameObject.FindGameObjectWithTag("ItemsPrefabs").GetComponent<Prefabs>().prefabs;

        tradeCanvasAllCells = tradeCanvasAll.GetComponent<CellsData>().cells;
        tradeCanvasWeaponCells = tradeCanvasWeapon.GetComponent<CellsData>().cells;
        tradeCanvasArmorCells = tradeCanvasArmor.GetComponent<CellsData>().cells;
        tradeCanvasPatronsCells = tradeCanvasPatrons.GetComponent<CellsData>().cells;
        tradeCanvasWeaponModsCells = tradeCanvasWeaponMods.GetComponent<CellsData>().cells;
        tradeCanvasOtherCells = tradeCanvasOther.GetComponent<CellsData>().cells;

        InitializeTypeMappings();
    }
    #endregion

    #region Public Methods
    public void StartTrade(float boost, string settigsKey)
    {
        tradeCellsData.settingsKey = settigsKey;
        //PreparationCanvases();
        tradeCanvas.SetActive(true);
        ClearItems();
        StartCoroutine(LoadDataDelayed(boost));
        
    }

    private IEnumerator LoadDataDelayed(float boost)
    {
        yield return null;
        if (tradeCellsData.HasSavedData)
        {
            // Если есть сохраненные данные - загружаем их и создаем копии
            LoadAndCreateCopies();
        }
        else
        {
            // Если нет сохраненных данных - генерируем новые предметы
            GenerateItems(boost);
        }
        //PlayerDataManager.Instance.Stats.InitializeCurrentWeight(settingsKey);
    }

    public void EndTrade()
    {
        tradeCanvas.SetActive(false);
        tradeCellsData.SaveData();
        ClearItems();
    }

    public void GenerateItems(float rarityBoost = 0f)
    {
        GenerateItemsSmartPacking(rarityBoost);
        SortItemsToSpecializedCanvases();
        HideCanvases();
    }

    public void HideCanvases()
    {
        tradeCanvasAll.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
        tradeCanvasWeapon.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(4000f, 0f, 0f);
        tradeCanvasArmor.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(4000f, 0f, 0f);
        tradeCanvasPatrons.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(4000f, 0f, 0f);
        tradeCanvasWeaponMods.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(4000f, 0f, 0f);
        tradeCanvasOther.transform.parent.GetComponent<RectTransform>().localPosition = new Vector3(4000f, 0f, 0f);
    }

    public void RemoveItemCopies(ItemTrade itemToRemove)
    {
        string itemBaseName = itemToRemove.gameObject.name.Replace("(Clone)", "").Trim();

        GameObject[] canvasesToCheck = {
        tradeCanvasAll, tradeCanvasWeapon, tradeCanvasArmor,
        tradeCanvasPatrons, tradeCanvasWeaponMods, tradeCanvasOther
    };

        foreach (var canvas in canvasesToCheck)
        {
            if (canvas == null) continue;
            RemoveItemFromCanvas(canvas, itemBaseName, itemToRemove);
        }
    }

    public void RemoveAllLinkedCopies(GameObject itemToRemove)
    {
        ItemTrade stats = itemToRemove.GetComponent<ItemTrade>();
        if (stats == null) return;

        var copiesToRemove = new List<GameObject>(stats.linkedCopies);

        foreach (var copy in copiesToRemove)
        {
            if (copy != null)
            {
                RemoveItemAndCleanCells(copy);
            }
        }

        if (stats.originalItem != null)
        {
            ItemTrade originalStats = stats.originalItem.GetComponent<ItemTrade>();
            if (originalStats != null)
            {
                originalStats.linkedCopies.Remove(itemToRemove);
            }
        }

        stats.linkedCopies.Clear();
        stats.originalItem = null;
    }
    #endregion

    #region Initialization
    private void InitializeTypeMappings()
    {
        typeToCanvasMap = new Dictionary<ItemType, GameObject>
        {
            { ItemType.MeleeWeapon, tradeCanvasWeapon },
            { ItemType.RangeWeapon, tradeCanvasWeapon },
            { ItemType.Armor, tradeCanvasArmor },
            { ItemType.Patron9x19, tradeCanvasPatrons },
            { ItemType.Patron_45ACP, tradeCanvasPatrons },
            { ItemType.Patron5x45, tradeCanvasPatrons },
            { ItemType.Patron7x62, tradeCanvasPatrons },
            { ItemType.Patron12x70, tradeCanvasPatrons },
            { ItemType.Patron_44Magnum, tradeCanvasPatrons },
            { ItemType.Sight, tradeCanvasWeaponMods },
            { ItemType.Magazine, tradeCanvasWeaponMods },
            { ItemType.Grip, tradeCanvasWeaponMods },
            { ItemType.Muzzle, tradeCanvasWeaponMods },
            { ItemType.Stock, tradeCanvasWeaponMods }
        };

        typeToCellsMap = new Dictionary<ItemType, List<Cell>>
        {
            { ItemType.MeleeWeapon, tradeCanvasWeaponCells },
            { ItemType.RangeWeapon, tradeCanvasWeaponCells },
            { ItemType.Armor, tradeCanvasArmorCells },
            { ItemType.Patron9x19, tradeCanvasPatronsCells },
            { ItemType.Patron_45ACP, tradeCanvasPatronsCells },
            { ItemType.Patron5x45, tradeCanvasPatronsCells },
            { ItemType.Patron7x62, tradeCanvasPatronsCells },
            { ItemType.Patron12x70, tradeCanvasPatronsCells },
            { ItemType.Patron_44Magnum, tradeCanvasPatronsCells },
            { ItemType.Sight, tradeCanvasWeaponModsCells },
            { ItemType.Magazine, tradeCanvasWeaponModsCells },
            { ItemType.Grip, tradeCanvasWeaponModsCells },
            { ItemType.Muzzle, tradeCanvasWeaponModsCells },
            { ItemType.Stock, tradeCanvasWeaponModsCells }
        };
    }

    //private void PreparationCanvases()
    //{
    //    tradeCanvasAll.transform.parent.localScale = Vector3.one;
    //    tradeCanvasWeapon.transform.parent.localScale = Vector3.one;
    //    tradeCanvasArmor.transform.parent.localScale = Vector3.one;
    //    tradeCanvasPatrons.transform.parent.localScale = Vector3.one;
    //    tradeCanvasWeaponMods.transform.parent.localScale = Vector3.one;
    //    tradeCanvasOther.transform.parent.localScale = Vector3.one;
    //}
    //private void OpenMainCanvas()
    //{
    //    tradeCanvasAll.transform.localScale = Vector3.one;
    //    tradeCanvasWeapon.transform.localScale = Vector3.zero;
    //    tradeCanvasArmor.transform.localScale = Vector3.zero;
    //    tradeCanvasPatrons.transform.localScale = Vector3.zero;
    //    tradeCanvasWeaponMods.transform.localScale = Vector3.zero;
    //    tradeCanvasOther.transform.localScale = Vector3.zero;
    //}

    #endregion

    #region Item Generation & Loading
    private void LoadAndCreateCopies()
    {
        // Загружаем данные из CellsData
        tradeCellsData.LoadData();

        // Создаем копии загруженных предметов
        CreateCopiesFromLoadedItems();

        HideCanvases();
    }

    private void CreateCopiesFromLoadedItems()
    {
        foreach (var loadedItem in tradeCellsData.LoadedItems)
        {
            var itemStructure = loadedItem.GetComponent<ItemStructure>();
            if (itemStructure != null)
            {
                // Создаем копии для специализированных канвасов
                CreateItemCopies(itemStructure);
            }
        }
    }

    private void GenerateItemsSmartPacking(float rarityBoost = 0f)
    {
        int itemsGenerated = 0;
        int currentIndex = 0;

        while (itemsGenerated < maxShopItems && currentIndex < tradeCanvasAllCells.Count)
        {
            if (IsCellOccupied(currentIndex))
            {
                currentIndex++;
                continue;
            }

            GameObject randomPrefab = GetRandomItemPrefab();
            ItemStructure itemComponent = randomPrefab.GetComponent<ItemStructure>();

            if (itemComponent == null)
            {
                currentIndex++;
                continue;
            }

            if (CanPlaceItem(currentIndex, itemComponent))
            {
                ItemStructure spawnedItem = Instantiate(randomPrefab, tradeCanvasAll.transform).GetComponent<ItemStructure>();
                spawnedItem.GetComponent<ItemStats>().itemQuality = ItemQualityGenerator.GetRandomQuality(rarityBoost);
                spawnedItem.GetComponent<ItemStats>().Initialized();

                var itemMove = spawnedItem.GetComponent<ItemMove>();

                if (itemMove.IsStackable)
                {
                    spawnedItem.GetComponent<ItemMove>().StackCount = Random.Range(10, itemMove.MaxStackSize);
                }

                spawnedItem.AddComponent<ItemTrade>();

                PlaceItem(currentIndex, spawnedItem);
                spawnedItems.Add(spawnedItem);
                itemsGenerated++;

                currentIndex = FindNextFreeCell(currentIndex + 1);
            }
            else
            {
                currentIndex++;
            }
        }
    }
    #endregion

    #region Item Copy Management
    private void CreateItemCopies(ItemStructure originalItem)
    {
        var itemStats = originalItem.GetComponent<ItemStats>();
        if (itemStats == null || itemStats.itemTypes == null || itemStats.itemTypes.Count == 0)
            return;

        foreach (var itemType in itemStats.itemTypes)
        {
            GameObject targetCanvas = GetTargetCanvasForType(itemType);
            if (targetCanvas == null || targetCanvas == tradeCanvasAll)
                continue;

            CreateItemCopy(originalItem, targetCanvas);
            break; // Создаем копию только для первого подходящего типа
        }
    }

    private void CreateItemCopy(ItemStructure originalItem, GameObject targetCanvas)
    {
        List<Cell> targetCells = GetCellsForCanvas(targetCanvas);
        int currentIndex = FindNextFreeCellInList(0, targetCells);

        if (currentIndex >= targetCells.Count)
            return;

        // Создаем копию предмета
        ItemStructure copiedItem = Instantiate(originalItem.gameObject, targetCanvas.transform).GetComponent<ItemStructure>();
        copiedItem.AddComponent<ItemTrade>();

        // Настраиваем связи между оригиналом и копией
        ItemTrade originalStats = originalItem.GetComponent<ItemTrade>();
        ItemTrade copiedStats = copiedItem.GetComponent<ItemTrade>();

        if (originalStats != null && copiedStats != null)
        {
            if (!originalStats.linkedCopies.Contains(copiedItem.gameObject))
            {
                originalStats.linkedCopies.Add(copiedItem.gameObject);
            }
            copiedStats.originalItem = originalItem.gameObject;

            if (!copiedStats.linkedCopies.Contains(originalItem.gameObject))
            {
                copiedStats.linkedCopies.Add(originalItem.gameObject);
            }
        }

        // Размещаем копию в целевом канвасе
        if (CanPlaceItemInList(currentIndex, copiedItem, targetCells))
        {
            PlaceItemInList(currentIndex, copiedItem, targetCells);
        }
        else
        {
            CleanupItemLinks(copiedStats);
            DestroyImmediate(copiedItem.gameObject);
        }
    }

    private void SortItemsToSpecializedCanvases()
    {
        var itemsByType = new Dictionary<GameObject, List<ItemStructure>>();

        foreach (var item in spawnedItems)
        {
            var itemStats = item.GetComponent<ItemStats>();
            if (itemStats == null || itemStats.itemTypes == null || itemStats.itemTypes.Count == 0)
                continue;

            foreach (var itemType in itemStats.itemTypes)
            {
                GameObject targetCanvas = GetTargetCanvasForType(itemType);
                if (targetCanvas == null)
                    targetCanvas = tradeCanvasOther;

                if (!itemsByType.ContainsKey(targetCanvas))
                    itemsByType[targetCanvas] = new List<ItemStructure>();

                itemsByType[targetCanvas].Add(item);
                break;
            }
        }

        foreach (var kvp in itemsByType)
        {
            PlaceItemsInCanvas(kvp.Value, kvp.Key);
        }
    }

    private GameObject GetTargetCanvasForType(ItemType itemType)
    {
        if (typeToCanvasMap.TryGetValue(itemType, out GameObject canvas))
            return canvas;

        return tradeCanvasOther;
    }

    private List<Cell> GetCellsForCanvas(GameObject canvas)
    {
        if (canvas == tradeCanvasWeapon) return tradeCanvasWeaponCells;
        if (canvas == tradeCanvasArmor) return tradeCanvasArmorCells;
        if (canvas == tradeCanvasPatrons) return tradeCanvasPatronsCells;
        if (canvas == tradeCanvasWeaponMods) return tradeCanvasWeaponModsCells;
        if (canvas == tradeCanvasOther) return tradeCanvasOtherCells;

        return tradeCanvasAllCells;
    }

    private void PlaceItemsInCanvas(List<ItemStructure> items, GameObject targetCanvas)
    {
        if (targetCanvas == tradeCanvasAll) return;

        List<Cell> targetCells = GetCellsForCanvas(targetCanvas);
        int currentIndex = 0;

        foreach (var item in items)
        {
            ItemStructure copiedItem = Instantiate(item.gameObject, targetCanvas.transform).GetComponent<ItemStructure>();
            copiedItem.AddComponent<ItemTrade>();

            ItemTrade originalStats = item.GetComponent<ItemTrade>();
            ItemTrade copiedStats = copiedItem.GetComponent<ItemTrade>();

            if (originalStats != null && copiedStats != null)
            {
                if (!originalStats.linkedCopies.Contains(copiedItem.gameObject))
                {
                    originalStats.linkedCopies.Add(copiedItem.gameObject);
                }

                copiedStats.originalItem = item.gameObject;

                if (!copiedStats.linkedCopies.Contains(item.gameObject))
                {
                    copiedStats.linkedCopies.Add(item.gameObject);
                }
            }

            currentIndex = FindNextFreeCellInList(currentIndex, targetCells);
            if (currentIndex >= targetCells.Count)
                break;

            if (CanPlaceItemInList(currentIndex, copiedItem, targetCells))
            {
                PlaceItemInList(currentIndex, copiedItem, targetCells);
                currentIndex = FindNextFreeCellInList(currentIndex + 1, targetCells);
            }
            else
            {
                CleanupItemLinks(copiedStats);
                DestroyImmediate(copiedItem.gameObject);
                currentIndex++;
            }
        }
    }
    #endregion

    #region Cell Management
    private int FindNextFreeCell(int startFrom)
    {
        for (int i = startFrom; i < tradeCanvasAllCells.Count; i++)
        {
            if (!IsCellOccupied(i))
            {
                return i;
            }
        }
        return tradeCanvasAllCells.Count;
    }

    private int FindNextFreeCellInList(int startFrom, List<Cell> cells)
    {
        for (int i = startFrom; i < cells.Count; i++)
        {
            if (!IsCellOccupiedInList(i, cells))
            {
                return i;
            }
        }
        return cells.Count;
    }

    private bool IsCellOccupied(int index)
    {
        if (index < 0 || index >= tradeCanvasAllCells.Count)
            return true;

        return tradeCanvasAllCells[index].NestedObject != null;
    }

    private bool IsCellOccupiedInList(int index, List<Cell> cells)
    {
        if (index < 0 || index >= cells.Count)
            return true;

        return cells[index].NestedObject != null;
    }

    private bool CanPlaceItem(int startIndex, ItemStructure item)
    {
        int startX = startIndex % GridWidth;
        int startY = startIndex / GridWidth;
        Vector2Int itemOffset = GetItemOffset(item);

        for (int y = 0; y < item.Size.y; y++)
        {
            for (int x = 0; x < item.Size.x; x++)
            {
                if (item.GetCell(x, y))
                {
                    int gridX = startX + x - itemOffset.x;
                    int gridY = startY + y - itemOffset.y;
                    int index = gridY * GridWidth + gridX;

                    if (gridX < 0 || gridX >= GridWidth || gridY < 0 || gridY >= GridHeight)
                        return false;

                    if (index >= tradeCanvasAllCells.Count || IsCellOccupied(index))
                        return false;
                }
            }
        }

        return true;
    }

    private bool CanPlaceItemInList(int startIndex, ItemStructure item, List<Cell> cells)
    {
        int startX = startIndex % GridWidth;
        int startY = startIndex / GridWidth;
        Vector2Int itemOffset = GetItemOffset(item);

        for (int y = 0; y < item.Size.y; y++)
        {
            for (int x = 0; x < item.Size.x; x++)
            {
                if (item.GetCell(x, y))
                {
                    int gridX = startX + x - itemOffset.x;
                    int gridY = startY + y - itemOffset.y;
                    int index = gridY * GridWidth + gridX;

                    if (gridX < 0 || gridX >= GridWidth || gridY < 0 || gridY >= GridHeight)
                        return false;

                    if (index >= cells.Count || IsCellOccupiedInList(index, cells))
                        return false;
                }
            }
        }

        return true;
    }

    private void PlaceItem(int startIndex, ItemStructure item)
    {
        int startX = startIndex % GridWidth;
        int startY = startIndex / GridWidth;
        Vector2Int itemOffset = GetItemOffset(item);

        List<Cell> occupiedCells = new List<Cell>();

        for (int y = 0; y < item.Size.y; y++)
        {
            for (int x = 0; x < item.Size.x; x++)
            {
                if (item.GetCell(x, y))
                {
                    int gridX = startX + x - itemOffset.x;
                    int gridY = startY + y - itemOffset.y;
                    int index = gridY * GridWidth + gridX;

                    if (index < tradeCanvasAllCells.Count && tradeCanvasAllCells[index] != null)
                    {
                        tradeCanvasAllCells[index].NestedObject = item.gameObject;
                        occupiedCells.Add(tradeCanvasAllCells[index]);
                    }
                }
            }
        }

        if (occupiedCells.Count > 0)
        {
            Vector3 centerPosition = CalculateCellsCenter(occupiedCells);
            item.transform.position = new Vector3(centerPosition.x, centerPosition.y, item.transform.position.z);
        }
    }

    private void PlaceItemInList(int startIndex, ItemStructure item, List<Cell> cells)
    {
        int startX = startIndex % GridWidth;
        int startY = startIndex / GridWidth;
        Vector2Int itemOffset = GetItemOffset(item);

        List<Cell> occupiedCells = new List<Cell>();

        for (int y = 0; y < item.Size.y; y++)
        {
            for (int x = 0; x < item.Size.x; x++)
            {
                if (item.GetCell(x, y))
                {
                    int gridX = startX + x - itemOffset.x;
                    int gridY = startY + y - itemOffset.y;
                    int index = gridY * GridWidth + gridX;

                    if (index < cells.Count && cells[index] != null)
                    {
                        cells[index].NestedObject = item.gameObject;
                        occupiedCells.Add(cells[index]);
                    }
                }
            }
        }

        if (occupiedCells.Count > 0)
        {
            Vector3 centerPosition = CalculateCellsCenter(occupiedCells);
            item.transform.position = new Vector3(centerPosition.x, centerPosition.y, item.transform.position.z);
        }
    }

    private Vector2Int GetItemOffset(ItemStructure item)
    {
        int minX = item.Size.x;
        int minY = item.Size.y;

        for (int y = 0; y < item.Size.y; y++)
        {
            for (int x = 0; x < item.Size.x; x++)
            {
                if (item.GetCell(x, y))
                {
                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                }
            }
        }

        return new Vector2Int(minX == item.Size.x ? 0 : minX, minY == item.Size.y ? 0 : minY);
    }

    private Vector3 CalculateCellsCenter(List<Cell> cells)
    {
        if (cells.Count == 0) return Vector3.zero;

        Bounds bounds = new Bounds(cells[0].transform.position, Vector3.zero);
        foreach (Cell cell in cells)
        {
            if (cell != null)
            {
                bounds.Encapsulate(cell.transform.position);
            }
        }

        return new Vector3(bounds.center.x, bounds.center.y, 0f);
    }
    #endregion

    #region Item Management
    private GameObject GetRandomItemPrefab()
    {
        if (itemPrefabs.Count == 0) return null;
        return itemPrefabs[Random.Range(0, itemPrefabs.Count)];
    }

    public void ClearItems()
    {
        ClearCanvasItems(tradeCanvasAll, tradeCanvasAllCells);
        ClearCanvasItems(tradeCanvasWeapon, tradeCanvasWeaponCells);
        ClearCanvasItems(tradeCanvasArmor, tradeCanvasArmorCells);
        ClearCanvasItems(tradeCanvasPatrons, tradeCanvasPatronsCells);
        ClearCanvasItems(tradeCanvasWeaponMods, tradeCanvasWeaponModsCells);
        ClearCanvasItems(tradeCanvasOther, tradeCanvasOtherCells);

        spawnedItems.Clear();
    }

    private void ClearCanvasItems(GameObject canvas, List<Cell> cells)
    {
        foreach (Cell cell in cells)
        {
            if (cell != null)
            {
                cell.NestedObject = null;
            }
        }
        for (int i = canvas.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(canvas.transform.GetChild(i).gameObject);
        }
    }

    private void CleanupItemLinks(ItemTrade stats)
    {
        if (stats == null) return;

        foreach (var copy in stats.linkedCopies)
        {
            if (copy != null)
            {
                ItemTrade copyStats = copy.GetComponent<ItemTrade>();
                if (copyStats != null)
                {
                    copyStats.linkedCopies.Remove(stats.gameObject);
                    if (copyStats.originalItem == stats.gameObject)
                    {
                        copyStats.originalItem = null;
                    }
                }
            }
        }

        if (stats.originalItem != null)
        {
            ItemTrade originalStats = stats.originalItem.GetComponent<ItemTrade>();
            if (originalStats != null)
            {
                originalStats.linkedCopies.Remove(stats.gameObject);
            }
        }

        stats.linkedCopies.Clear();
        stats.originalItem = null;
    }

    private void RemoveItemAndCleanCells(GameObject item)
    {
        if (item == null) return;

        var allCells = FindObjectsOfType<Cell>();
        foreach (var cell in allCells)
        {
            if (cell != null && cell.NestedObject == item)
            {
                cell.NestedObject = null;
            }
        }

        ItemTrade itemStats = item.GetComponent<ItemTrade>();
        CleanupItemLinks(itemStats);

        DestroyImmediate(item);
    }

    private void RemoveItemFromCanvas(GameObject canvas, string itemBaseName, ItemTrade originalItem)
    {
        for (int i = canvas.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = canvas.transform.GetChild(i);
            if (child != null && child != originalItem.transform)
            {
                string childName = child.name.Replace("(Clone)", "").Trim();
                if (childName == itemBaseName)
                {
                    ItemStats childStats = child.GetComponent<ItemStats>();
                    ItemStats originalStats = originalItem.GetComponent<ItemStats>();

                    if (childStats != null && originalStats != null &&
                        childStats.itemQuality == originalStats.itemQuality &&
                        childStats.price == originalStats.price)
                    {
                        var cells = canvas.GetComponent<CellsData>()?.cells;
                        if (cells != null)
                        {
                            foreach (var cell in cells)
                            {
                                if (cell != null && cell.NestedObject == child.gameObject)
                                {
                                    cell.NestedObject = null;
                                }
                            }
                        }

                        DestroyImmediate(child.gameObject);
                    }
                }
            }
        }
    }
    #endregion

    public bool IsTrading()
    {
        return tradeCanvas != null && tradeCanvas.activeInHierarchy;
    }
}