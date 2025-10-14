using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class TradeGenerator : MonoBehaviour
{
    [Header("Trade References")]
    public GameObject tradeCanvasAll;
    public GameObject tradeCanvasWeapon;
    public GameObject tradeCanvasArmor;
    public GameObject tradeCanvasPatrons;
    public GameObject tradeCanvasWeaponMods;
    public GameObject tradeCanvasOther;

    private List<Cell> tradeCanvasAllCells = new List<Cell>();
    private List<Cell> tradeCanvasWeaponCells = new List<Cell>();
    private List<Cell> tradeCanvasArmorCells = new List<Cell>();
    private List<Cell> tradeCanvasPatronsCells = new List<Cell>();
    private List<Cell> tradeCanvasWeaponModsCells = new List<Cell>();
    private List<Cell> tradeCanvasOtherCells = new List<Cell>();

    [Header("Item Generation")]
    public int maxShopItems = 30;

    private List<ItemStructure> spawnedItems = new List<ItemStructure>();
    private const int GridWidth = 10;
    private const int GridHeight = 10;

    [HideInInspector] public List<GameObject> itemPrefabs = new List<GameObject>();

    // Словарь для связи типов предметов с соответствующими канвасами
    private Dictionary<ItemType, GameObject> typeToCanvasMap;
    private Dictionary<ItemType, List<Cell>> typeToCellsMap;

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

    private void InitializeTypeMappings()
    {
        // Инициализация маппинга типов предметов на канвасы
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

        // Инициализация маппинга типов предметов на списки ячеек
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

    public void GenerateItems(float rarityBoost = 0f)
    {
        GenerateItemsSmartPacking(rarityBoost);
        SortItemsToSpecializedCanvases();
        HideCanvases();
    }

    public void HideCanvases()
    {
        tradeCanvasWeapon.transform.parent.localScale = Vector3.zero;
        tradeCanvasArmor.transform.parent.localScale = Vector3.zero;
        tradeCanvasPatrons.transform.parent.localScale = Vector3.zero;
        tradeCanvasWeaponMods.transform.parent.localScale = Vector3.zero;
        tradeCanvasOther.transform.parent.localScale = Vector3.zero;
    }

    private void GenerateItemsSmartPacking(float rarityBoost = 0f)
    {
        int itemsGenerated = 0;
        int currentIndex = 0;

        while (itemsGenerated < maxShopItems && currentIndex < tradeCanvasAllCells.Count)
        {
            // Пропускаем занятые ячейки
            if (IsCellOccupied(currentIndex))
            {
                currentIndex++;
                continue;
            }

            // Берем случайный префаб (могут повторяться)
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
                spawnedItem.AddComponent<ItemTrade>();

                PlaceItem(currentIndex, spawnedItem);
                spawnedItems.Add(spawnedItem);
                itemsGenerated++;

                // Переходим к следующей свободной ячейке
                currentIndex = FindNextFreeCell(currentIndex + 1);
            }
            else
            {
                currentIndex++;
            }
        }
    }

    private void SortItemsToSpecializedCanvases()
    {
        // Группируем предметы по типам
        var itemsByType = new Dictionary<GameObject, List<ItemStructure>>();

        foreach (var item in spawnedItems)
        {
            var itemStats = item.GetComponent<ItemStats>();
            if (itemStats == null || itemStats.itemTypes == null || itemStats.itemTypes.Count == 0)
                continue;

            // Для каждого типа предмета определяем целевой канвас
            foreach (var itemType in itemStats.itemTypes)
            {
                GameObject targetCanvas = GetTargetCanvasForType(itemType);
                if (targetCanvas == null)
                    targetCanvas = tradeCanvasOther; // По умолчанию в "Other"

                if (!itemsByType.ContainsKey(targetCanvas))
                    itemsByType[targetCanvas] = new List<ItemStructure>();

                itemsByType[targetCanvas].Add(item);
                break; // Берем только первый подходящий тип
            }
        }

        // Размещаем предметы в соответствующих канвасах
        foreach (var kvp in itemsByType)
        {
            PlaceItemsInCanvas(kvp.Value, kvp.Key);
        }
    }

    private GameObject GetTargetCanvasForType(ItemType itemType)
    {
        if (typeToCanvasMap.TryGetValue(itemType, out GameObject canvas))
            return canvas;

        // Для типов, не попавших в маппинг, используем канвас "Other"
        return tradeCanvasOther;
    }

    private List<Cell> GetCellsForCanvas(GameObject canvas)
    {
        if (canvas == tradeCanvasWeapon) return tradeCanvasWeaponCells;
        if (canvas == tradeCanvasArmor) return tradeCanvasArmorCells;
        if (canvas == tradeCanvasPatrons) return tradeCanvasPatronsCells;
        if (canvas == tradeCanvasWeaponMods) return tradeCanvasWeaponModsCells;
        if (canvas == tradeCanvasOther) return tradeCanvasOtherCells;

        return tradeCanvasAllCells; // fallback
    }

    private void PlaceItemsInCanvas(List<ItemStructure> items, GameObject targetCanvas)
    {
        if (targetCanvas == tradeCanvasAll) return;

        List<Cell> targetCells = GetCellsForCanvas(targetCanvas);
        int currentIndex = 0;

        foreach (var item in items)
        {
            // Создаем копию предмета для специализированного канваса
            ItemStructure copiedItem = Instantiate(item.gameObject, targetCanvas.transform).GetComponent<ItemStructure>();
            copiedItem.AddComponent<ItemTrade>();

            // ПОЛУЧАЕМ ItemTrade ОБОИХ ПРЕДМЕТОВ
            ItemTrade originalStats = item.GetComponent<ItemTrade>();
            ItemTrade copiedStats = copiedItem.GetComponent<ItemTrade>();

            // СОЗДАЕМ ССЫЛКИ МЕЖДУ КОПИЯМИ
            if (originalStats != null && copiedStats != null)
            {
                // Добавляем копию в список оригинального предмета
                if (!originalStats.linkedCopies.Contains(copiedItem.gameObject))
                {
                    originalStats.linkedCopies.Add(copiedItem.gameObject);
                }

                // Устанавливаем ссылку на оригинал у копии
                copiedStats.originalItem = item.gameObject;

                // Также добавляем оригинал в список копий (для двусторонней связи)
                if (!copiedStats.linkedCopies.Contains(item.gameObject))
                {
                    copiedStats.linkedCopies.Add(item.gameObject);
                }
            }

            // Находим свободное место в целевом канвасе
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
                // Если не поместилось, очищаем ссылки и уничтожаем копию
                CleanupItemLinks(copiedStats);
                DestroyImmediate(copiedItem.gameObject);
                currentIndex++;
            }
        }
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

    private bool IsCellOccupiedInList(int index, List<Cell> cells)
    {
        if (index < 0 || index >= cells.Count)
            return true;

        return cells[index].NestedObject != null;
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

    // Остальные методы остаются без изменений
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

    private bool IsCellOccupied(int index)
    {
        if (index < 0 || index >= tradeCanvasAllCells.Count)
            return true;

        return tradeCanvasAllCells[index].NestedObject != null;
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

    public void ClearItems()
    {
        // Оригинальный код очистки остается без изменений
        #region tradeCanvasAllCells
        foreach (Cell cell in tradeCanvasAllCells)
        {
            if (cell != null)
            {
                cell.NestedObject = null;
            }
        }
        for (int i = tradeCanvasAll.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(tradeCanvasAll.transform.GetChild(i).gameObject);
        }
        #endregion

        #region tradeCanvasWeaponCells
        foreach (Cell cell in tradeCanvasWeaponCells)
        {
            if (cell != null)
            {
                cell.NestedObject = null;
            }
        }
        for (int i = tradeCanvasWeapon.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(tradeCanvasWeapon.transform.GetChild(i).gameObject);
        }
        #endregion

        #region tradeCanvasArmorCells
        foreach (Cell cell in tradeCanvasArmorCells)
        {
            if (cell != null)
            {
                cell.NestedObject = null;
            }
        }
        for (int i = tradeCanvasArmor.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(tradeCanvasArmor.transform.GetChild(i).gameObject);
        }
        #endregion

        #region tradeCanvasPatronsCells
        foreach (Cell cell in tradeCanvasPatronsCells)
        {
            if (cell != null)
            {
                cell.NestedObject = null;
            }
        }
        for (int i = tradeCanvasPatrons.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(tradeCanvasPatrons.transform.GetChild(i).gameObject);
        }
        #endregion

        #region tradeCanvasWeaponModsCells
        foreach (Cell cell in tradeCanvasWeaponModsCells)
        {
            if (cell != null)
            {
                cell.NestedObject = null;
            }
        }
        for (int i = tradeCanvasWeaponMods.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(tradeCanvasWeaponMods.transform.GetChild(i).gameObject);
        }
        #endregion

        #region tradeCanvasOtherCells
        foreach (Cell cell in tradeCanvasOtherCells)
        {
            if (cell != null)
            {
                cell.NestedObject = null;
            }
        }
        for (int i = tradeCanvasOther.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(tradeCanvasOther.transform.GetChild(i).gameObject);
        }
        #endregion

        spawnedItems.Clear();
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

    private GameObject GetRandomItemPrefab()
    {
        if (itemPrefabs.Count == 0) return null;
        return itemPrefabs[Random.Range(0, itemPrefabs.Count)];
    }



    // В класс TradeGenerator добавь метод
    public void RemoveItemCopies(ItemTrade itemToRemove)
    {
        string itemBaseName = itemToRemove.gameObject.name.Replace("(Clone)", "").Trim();

        // Список всех канвасов для проверки
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

    /// <summary>
    /// Очищает ссылки предмета перед удалением
    /// </summary>
    private void CleanupItemLinks(ItemTrade stats)
    {
        if (stats == null) return;

        // Удаляем этот предмет из списков linkedCopies всех связанных предметов
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

        // Удаляем этот предмет как оригинал у копий
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

    /// <summary>
    /// Удаляет все связанные копии предмета
    /// </summary>
    public void RemoveAllLinkedCopies(GameObject itemToRemove)
    {
        ItemTrade stats = itemToRemove.GetComponent<ItemTrade>();
        if (stats == null) return;

        // Создаем копию списка, т.к. мы будем удалять элементы во время итерации
        var copiesToRemove = new List<GameObject>(stats.linkedCopies);

        foreach (var copy in copiesToRemove)
        {
            if (copy != null)
            {
                RemoveItemAndCleanCells(copy);
            }
        }

        // Также удаляем из оригинального предмета, если это копия
        if (stats.originalItem != null)
        {
            ItemTrade originalStats = stats.originalItem.GetComponent<ItemTrade>();
            if (originalStats != null)
            {
                originalStats.linkedCopies.Remove(itemToRemove);
            }
        }

        // Очищаем собственный список
        stats.linkedCopies.Clear();
        stats.originalItem = null;
    }

    /// <summary>
    /// Удаляет предмет и очищает занимаемые им ячейки
    /// </summary>
    private void RemoveItemAndCleanCells(GameObject item)
    {
        if (item == null) return;

        // Очищаем ячейки
        var allCells = FindObjectsOfType<Cell>();
        foreach (var cell in allCells)
        {
            if (cell != null && cell.NestedObject == item)
            {
                cell.NestedObject = null;
            }
        }

        // Очищаем ссылки
        ItemTrade itemStats = item.GetComponent<ItemTrade>();
        CleanupItemLinks(itemStats);

        // Удаляем объект
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
                        // Очищаем ячейки
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
}
