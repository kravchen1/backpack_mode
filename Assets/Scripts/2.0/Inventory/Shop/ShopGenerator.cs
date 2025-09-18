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

    private List<ItemNew> spawnedItems = new List<ItemNew>();
    private const int GridWidth = 10;
    private const int GridHeight = 10;

    public void GenerateItems()
    {
        ClearItems();

        int itemsGenerated = 0;
        int currentIndex = 0; // Начинаем с нулевой ячейки

        while (itemsGenerated < maxShopItems && currentIndex < shopCells.Count)
        {
            // Пропускаем занятые ячейки
            if (shopCells[currentIndex].nestedObject != null)
            {
                currentIndex++;
                continue;
            }

            GameObject randomPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            ItemNew itemComponent = randomPrefab.GetComponent<ItemNew>();

            if (itemComponent == null)
            {
                currentIndex++;
                continue;
            }

            if (CanPlaceItem(currentIndex, itemComponent.WidthCell, itemComponent.HeightCell))
            {
                ItemNew spawnedItem = Instantiate(randomPrefab, shopCanvas.transform).GetComponent<ItemNew>();
                PlaceItem(currentIndex, spawnedItem);
                spawnedItems.Add(spawnedItem);
                itemsGenerated++;

                // Переходим к следующей свободной позиции сразу после текущего предмета
                currentIndex += itemComponent.WidthCell;
            }
            else
            {
                // Если не помещается, пробуем следующую ячейку
                currentIndex++;
            }
        }
    }

    public void ClearItems()
    {
        foreach (Cell cell in shopCells)
        {
            if (cell != null)
            {
                cell.nestedObject = null;
            }
        }

        foreach (ItemNew item in spawnedItems)
        {
            if (item != null && item.gameObject != null)
            {
                Destroy(item.gameObject);
            }
        }
        spawnedItems.Clear();
    }

    private bool CanPlaceItem(int startIndex, int width, int height)
    {
        int startX = startIndex % GridWidth;
        int startY = startIndex / GridWidth;

        // Проверяем выход за границы сетки
        if (startX + width > GridWidth || startY + height > GridHeight)
        {
            return false;
        }

        // Проверяем занятость ячеек
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (startY + y) * GridWidth + (startX + x);

                if (index >= shopCells.Count || shopCells[index] == null || shopCells[index].nestedObject != null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void PlaceItem(int startIndex, ItemNew item)
    {
        int startX = startIndex % GridWidth;
        int startY = startIndex / GridWidth;

        // Занимаем ячейки
        List<Cell> occupiedCells = new List<Cell>();
        for (int y = 0; y < item.HeightCell; y++)
        {
            for (int x = 0; x < item.WidthCell; x++)
            {
                int index = (startY + y) * GridWidth + (startX + x);
                if (index < shopCells.Count && shopCells[index] != null)
                {
                    shopCells[index].nestedObject = item.gameObject;
                    occupiedCells.Add(shopCells[index]);
                }
            }
        }

        // Вычисляем центр занятых ячеек
        Vector3 cellsCenter = CalculateCellsCenter(occupiedCells);

        // Вычисляем центр предмета
        Vector3 itemCenter = CalculateItemCenter(item);

        // Позиционируем предмет
        item.transform.position = cellsCenter - itemCenter + item.transform.position;
    }

    private Vector3 CalculateCellsCenter(List<Cell> cells)
    {
        if (cells.Count == 0) return Vector3.zero;

        Vector3 center = Vector3.zero;
        foreach (Cell cell in cells)
        {
            if (cell != null)
            {
                center += cell.transform.position;
            }
        }
        return center / cells.Count;
    }

    private Vector3 CalculateItemCenter(ItemNew item)
    {
        if (item.itemColliders == null || item.itemColliders.Count == 0)
            return item.transform.position;

        Bounds bounds = new Bounds(item.itemColliders[0].bounds.center, Vector3.zero);
        foreach (BoxCollider2D collider in item.itemColliders)
        {
            if (collider != null)
            {
                bounds.Encapsulate(collider.bounds);
            }
        }
        return bounds.center;
    }

    // Альтернативная версия - плотная упаковка по строкам
    public void GenerateItemsPacked()
    {
        ClearItems();

        int itemsGenerated = 0;
        int currentIndex = 0;

        while (itemsGenerated < maxShopItems && currentIndex < shopCells.Count)
        {
            // Пропускаем занятые ячейки
            if (shopCells[currentIndex].nestedObject != null)
            {
                currentIndex++;
                continue;
            }

            GameObject randomPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            ItemNew itemComponent = randomPrefab.GetComponent<ItemNew>();

            if (itemComponent == null)
            {
                currentIndex++;
                continue;
            }

            if (CanPlaceItem(currentIndex, itemComponent.WidthCell, itemComponent.HeightCell))
            {
                ItemNew spawnedItem = Instantiate(randomPrefab, shopCanvas.transform).GetComponent<ItemNew>();
                PlaceItem(currentIndex, spawnedItem);
                spawnedItems.Add(spawnedItem);
                itemsGenerated++;

                // Переходим к следующей ячейке в строке
                currentIndex += itemComponent.WidthCell;

                // Если достигли конца строки, переходим на новую
                if (currentIndex % GridWidth == 0)
                {
                    currentIndex = (currentIndex / GridWidth + 1) * GridWidth;
                }
            }
            else
            {
                currentIndex++;

                // Если достигли конца строки, переходим на новую
                if (currentIndex % GridWidth == 0)
                {
                    currentIndex = (currentIndex / GridWidth + 1) * GridWidth;

                    // Пропускаем строки если они уже заняты
                    while (currentIndex < shopCells.Count && shopCells[currentIndex].nestedObject != null)
                    {
                        currentIndex += GridWidth;
                    }
                }
            }
        }
    }
}