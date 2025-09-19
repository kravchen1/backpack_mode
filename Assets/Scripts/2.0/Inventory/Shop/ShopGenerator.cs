using System.Collections.Generic;
using UnityEngine;

public class ShopGenerator : MonoBehaviour
{
    [Header("Shop References")]
    public GameObject shopCanvas;
    public List<Cell> shopCells = new List<Cell>();

    [Header("Item Generation")]
    public GameObject[] itemPrefabs;
    public int maxShopItems = 8;
    public bool useSmartPacking = true;

    private List<ItemStructure> spawnedItems = new List<ItemStructure>();
    private const int GridWidth = 10;
    private const int GridHeight = 10;

    public void GenerateItems()
    {
        ClearItems();

        if (useSmartPacking)
        {
            GenerateItemsSmartPacking();
        }
        else
        {
            GenerateItemsSimple();
        }
    }

    private void GenerateItemsSimple()
    {
        int itemsGenerated = 0;
        int currentIndex = 0;

        while (itemsGenerated < maxShopItems && currentIndex < shopCells.Count)
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
                ItemStructure spawnedItem = Instantiate(randomPrefab, shopCanvas.transform).GetComponent<ItemStructure>();
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

    private void GenerateItemsSmartPacking()
    {
        int itemsGenerated = 0;
        int currentIndex = 0;

        while (itemsGenerated < maxShopItems && currentIndex < shopCells.Count)
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
                ItemStructure spawnedItem = Instantiate(randomPrefab, shopCanvas.transform).GetComponent<ItemStructure>();
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

    private int FindNextFreeCell(int startFrom)
    {
        for (int i = startFrom; i < shopCells.Count; i++)
        {
            if (!IsCellOccupied(i))
            {
                return i;
            }
        }
        return shopCells.Count;
    }

    private bool IsCellOccupied(int index)
    {
        if (index < 0 || index >= shopCells.Count)
            return true;

        return shopCells[index].nestedObject != null;
    }

    private Vector2Int GetItemActualSize(ItemStructure item)
    {
        int minX = item.Size.x, maxX = -1;
        int minY = item.Size.y, maxY = -1;
        bool hasOccupiedCells = false;

        for (int y = 0; y < item.Size.y; y++)
        {
            for (int x = 0; x < item.Size.x; x++)
            {
                if (item.GetCell(x, y))
                {
                    hasOccupiedCells = true;
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        if (!hasOccupiedCells)
            return new Vector2Int(1, 1);

        return new Vector2Int(maxX - minX + 1, maxY - minY + 1);
    }

    private Vector2Int GetItemOffset(ItemStructure item)
    {
        int minX = item.Size.x, minY = item.Size.y;

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

        return new Vector2Int(minX, minY);
    }

    public void ClearItems()
    {
        // Очищаем все ячейки
        foreach (Cell cell in shopCells)
        {
            if (cell != null)
            {
                cell.nestedObject = null;
            }
        }

        // Удаляем все созданные предметы
        foreach (ItemStructure item in spawnedItems)
        {
            if (item != null && item.gameObject != null)
            {
                DestroyImmediate(item.gameObject);
            }
        }
        spawnedItems.Clear();
    }

    private bool CanPlaceItem(int startIndex, ItemStructure item)
    {
        int startX = startIndex % GridWidth;
        int startY = startIndex / GridWidth;
        Vector2Int itemOffset = GetItemOffset(item);

        // Проверяем каждую ячейку предмета
        for (int y = 0; y < item.Size.y; y++)
        {
            for (int x = 0; x < item.Size.x; x++)
            {
                if (item.GetCell(x, y))
                {
                    int gridX = startX + x - itemOffset.x;
                    int gridY = startY + y - itemOffset.y;
                    int index = gridY * GridWidth + gridX;

                    // Проверяем границы и занятость
                    if (gridX < 0 || gridX >= GridWidth || gridY < 0 || gridY >= GridHeight)
                        return false;

                    if (index >= shopCells.Count || IsCellOccupied(index))
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

        // Занимаем ячейки согласно форме предмета
        for (int y = 0; y < item.Size.y; y++)
        {
            for (int x = 0; x < item.Size.x; x++)
            {
                if (item.GetCell(x, y))
                {
                    int gridX = startX + x - itemOffset.x;
                    int gridY = startY + y - itemOffset.y;
                    int index = gridY * GridWidth + gridX;

                    if (index < shopCells.Count && shopCells[index] != null)
                    {
                        shopCells[index].nestedObject = item.gameObject;
                        occupiedCells.Add(shopCells[index]);
                    }
                }
            }
        }

        // Позиционируем предмет в центр занятых ячеек с сохранением Z
        if (occupiedCells.Count > 0)
        {
            Vector3 centerPosition = CalculateCellsCenter(occupiedCells);
            item.transform.position = new Vector3(centerPosition.x, centerPosition.y, item.transform.position.z);
        }
    }

    private Vector3 CalculateCellsCenter(List<Cell> cells)
    {
        if (cells.Count == 0) return Vector3.zero;

        Vector3 center = Vector3.zero;
        foreach (Cell cell in cells)
        {
            if (cell != null)
            {
                // Используем только X и Y координаты ячейки, Z остается неизменным
                center.x += cell.transform.position.x;
                center.y += cell.transform.position.y;
            }
        }

        return new Vector3(center.x / cells.Count, center.y / cells.Count, 0f);
    }

    private GameObject GetRandomItemPrefab()
    {
        if (itemPrefabs.Length == 0) return null;
        return itemPrefabs[Random.Range(0, itemPrefabs.Length)];
    }

    // Метод для плотной упаковки (всегда с начала)
    public void GenerateItemsDensePacking()
    {
        ClearItems();

        int itemsGenerated = 0;
        int currentIndex = 0;

        while (itemsGenerated < maxShopItems && currentIndex < shopCells.Count)
        {
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
                ItemStructure spawnedItem = Instantiate(randomPrefab, shopCanvas.transform).GetComponent<ItemStructure>();
                PlaceItem(currentIndex, spawnedItem);
                spawnedItems.Add(spawnedItem);
                itemsGenerated++;

                // После размещения ищем следующую свободную ячейку с начала
                currentIndex = FindNextFreeCell(0);
            }
            else
            {
                currentIndex++;

                // Если дошли до конца, но еще нужно создать предметы - начинаем сначала
                if (currentIndex >= shopCells.Count && itemsGenerated < maxShopItems)
                {
                    currentIndex = FindNextFreeCell(0);
                    if (currentIndex >= shopCells.Count) break;
                }
            }
        }
    }

    // Визуализация занятых ячеек для отладки
    public void DebugOccupiedCells()
    {
        string grid = "Sostoyanie setki:\n";
        for (int y = 0; y < GridHeight; y++)
        {
            for (int x = 0; x < GridWidth; x++)
            {
                int index = y * GridWidth + x;
                if (index < shopCells.Count)
                {
                    grid += shopCells[index].nestedObject != null ? "[X] " : "[ ] ";
                }
            }
            grid += "\n";
        }
        Debug.Log(grid);
    }

    // Новый метод: пытается создать максимальное количество предметов
    public void GenerateMaxItems()
    {
        ClearItems();

        int itemsGenerated = 0;
        int currentIndex = 0;
        int attempts = 0;
        const int maxAttempts = 1000; // Защита от бесконечного цикла

        while (itemsGenerated < maxShopItems && currentIndex < shopCells.Count && attempts < maxAttempts)
        {
            attempts++;

            if (IsCellOccupied(currentIndex))
            {
                currentIndex++;
                continue;
            }

            // Случайный префаб (повторения разрешены)
            GameObject randomPrefab = GetRandomItemPrefab();
            ItemStructure itemComponent = randomPrefab.GetComponent<ItemStructure>();

            if (itemComponent == null)
            {
                currentIndex++;
                continue;
            }

            if (CanPlaceItem(currentIndex, itemComponent))
            {
                ItemStructure spawnedItem = Instantiate(randomPrefab, shopCanvas.transform).GetComponent<ItemStructure>();
                PlaceItem(currentIndex, spawnedItem);
                spawnedItems.Add(spawnedItem);
                itemsGenerated++;

                // Ищем следующую свободную ячейку
                currentIndex = FindNextFreeCell(0);
            }
            else
            {
                currentIndex++;

                // Если дошли до конца сетки, начинаем сначала
                if (currentIndex >= shopCells.Count)
                {
                    currentIndex = FindNextFreeCell(0);
                    if (currentIndex >= shopCells.Count) break;
                }
            }
        }

        Debug.Log($"Sozdano {itemsGenerated} predmetov iz {maxShopItems}");
    }
}