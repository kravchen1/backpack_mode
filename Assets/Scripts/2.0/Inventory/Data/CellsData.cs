using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellsData : MonoBehaviour
{
    [Header("Settings")]
    public string settingsKey = "InventoryData";

    [Header("References")]
    public List<Cell> cells = new List<Cell>();
    public Transform itemsParent;

    [Header("Prefabs")]
    public List<ItemStructure> itemPrefabs = new List<ItemStructure>();

    private DataJsonCellList dataJsonList = new DataJsonCellList();
    private const int GridWidth = 10;
    private bool skipNextSave = false;

    private void OnEnable()
    {
        if (settingsKey == "InventoryData")
        {
            if (!skipNextSave)
            {
                LoadData();
            }
            skipNextSave = false;
        }
    }

    private void OnDisable()
    {
        if (settingsKey == "InventoryData")
        {
            // Сохраняем только если это не временное отключение
            if (!gameObject.scene.isLoaded) return;

            SaveData();
            skipNextSave = true;
        }
    }

    private void OnApplicationQuit()
    {
        if (settingsKey == "InventoryData")
        {
            SaveData();
        }
    }

    public void SaveData()
    {
        dataJsonList.inventoryDataJsonList.Clear();

        Dictionary<GameObject, bool> savedItems = new Dictionary<GameObject, bool>();

        foreach (Cell cell in cells)
        {
            if (cell.nestedObject != null && !savedItems.ContainsKey(cell.nestedObject))
            {
                var item = cell.nestedObject.GetComponent<ItemStructure>();
                if (item != null)
                {
                    Cell mainCell = FindMainCellForItem(item);
                    if (mainCell != null)
                    {
                        dataJsonList.inventoryDataJsonList.Add(new DataCellJson(
                            mainCell.gameObject.name,
                            item.gameObject.name.Replace("(Clone)", ""),
                            item.transform.eulerAngles.z
                        ));

                        savedItems[cell.nestedObject] = true;
                    }
                }
            }
        }

        string jsonCellsSave = JsonUtility.ToJson(dataJsonList);
        PlayerPrefs.SetString(settingsKey, jsonCellsSave);
        PlayerPrefs.Save();

        Debug.Log($"Data saved. Unique items: {dataJsonList.inventoryDataJsonList.Count}");
    }

    public void LoadData()
    {
        ClearAllItems();
        string jsonData = PlayerPrefs.GetString(settingsKey, "");
        if (string.IsNullOrEmpty(jsonData))
        {
            Debug.Log("No saved data found");
            return;
        }

        try
        {
            dataJsonList = JsonUtility.FromJson<DataJsonCellList>(jsonData);
            ClearAllItems();

            Debug.Log($"Loading {dataJsonList.inventoryDataJsonList.Count} items...");

            foreach (DataCellJson cellData in dataJsonList.inventoryDataJsonList)
            {
                Cell mainCell = cells.Find(c => c.gameObject.name == cellData.cellName);
                if (mainCell == null)
                {
                    Debug.LogWarning($"Cell not found: {cellData.cellName}");
                    continue;
                }

                ItemStructure itemPrefab = itemPrefabs.Find(p => p.gameObject.name == cellData.cellNestedObjectName);
                if (itemPrefab == null)
                {
                    Debug.LogWarning($"Prefab not found: {cellData.cellNestedObjectName}");
                    continue;
                }

                ItemStructure newItem = Instantiate(itemPrefab, itemsParent);
                newItem.name = itemPrefab.gameObject.name;

                // Устанавливаем ротацию ДО размещения
                newItem.transform.rotation = Quaternion.Euler(0, 0, cellData.rotationZ);

                // Размещаем предмет с учетом поворота
                PlaceItemInCell(mainCell, newItem, cellData.rotationZ);

                Debug.Log($"Loaded item {cellData.cellNestedObjectName} in cell {cellData.cellName} with rotation {cellData.rotationZ}°");
            }

            Debug.Log("Data loaded successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading data: {e.Message}");
        }
    }

    private Cell FindMainCellForItem(ItemStructure item)
    {
        Cell mainCell = null;
        int minIndex = int.MaxValue;

        foreach (Cell cell in cells)
        {
            if (cell.nestedObject == item.gameObject)
            {
                int cellIndex = cells.IndexOf(cell);
                if (cellIndex < minIndex)
                {
                    minIndex = cellIndex;
                    mainCell = cell;
                }
            }
        }

        return mainCell;
    }

    private void PlaceItemInCell(Cell mainCell, ItemStructure item, float rotationZ)
    {
        int mainCellIndex = cells.IndexOf(mainCell);
        if (mainCellIndex == -1) return;

        int startX = mainCellIndex % GridWidth;
        int startY = mainCellIndex / GridWidth;

        // Получаем форму предмета с учетом поворота
        bool[,] rotatedShape = GetRotatedItemShape(item, rotationZ);
        Vector2Int rotatedSize = GetRotatedItemSize(item, rotationZ);
        Vector2Int rotatedOffset = GetRotatedItemOffset(rotatedShape, rotatedSize);

        // Занимаем ячейки согласно повернутой форме
        for (int y = 0; y < rotatedSize.y; y++)
        {
            for (int x = 0; x < rotatedSize.x; x++)
            {
                if (rotatedShape[x, y])
                {
                    int gridX = startX + x - rotatedOffset.x;
                    int gridY = startY + y - rotatedOffset.y;
                    int index = gridY * GridWidth + gridX;

                    if (index < cells.Count && cells[index] != null)
                    {
                        cells[index].nestedObject = item.gameObject;
                    }
                }
            }
        }

        // Позиционируем предмет
        Vector3 centerPosition = CalculateItemCenterPosition(mainCellIndex, rotatedShape, rotatedSize, rotatedOffset);
        Debug.Log($"Placing item in cell: {mainCell.name} at index {mainCellIndex}");
        Debug.Log($"Rotated size: {rotatedSize}, offset: {rotatedOffset}");
        item.transform.position = new Vector3(centerPosition.x, centerPosition.y, item.transform.position.z);
    }

    private bool[,] GetRotatedItemShape(ItemStructure item, float rotationZ)
    {
        // Нормализуем rotationZ к 0, 90, 180, 270 градусам
        int normalizedRotation = Mathf.RoundToInt(rotationZ / 90) % 4;
        if (normalizedRotation < 0) normalizedRotation += 4;

        bool[,] originalShape = new bool[item.Size.x, item.Size.y];
        for (int y = 0; y < item.Size.y; y++)
        {
            for (int x = 0; x < item.Size.x; x++)
            {
                originalShape[x, y] = item.GetCell(x, y);
            }
        }

        // Поворачиваем матрицу в зависимости от угла
        return RotateMatrix(originalShape, item.Size.x, item.Size.y, normalizedRotation);
    }

    private bool[,] RotateMatrix(bool[,] matrix, int width, int height, int rotation)
    {
        switch (rotation)
        {
            case 0: // 0 градусов
                return matrix;

            case 1: // 90 градусов
                var rotated90 = new bool[height, width];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        rotated90[y, width - 1 - x] = matrix[x, y];
                    }
                }
                return rotated90;

            case 2: // 180 градусов
                var rotated180 = new bool[width, height];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        rotated180[width - 1 - x, height - 1 - y] = matrix[x, y];
                    }
                }
                return rotated180;

            case 3: // 270 градусов
                var rotated270 = new bool[height, width];
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        rotated270[height - 1 - y, x] = matrix[x, y];
                    }
                }
                return rotated270;

            default:
                return matrix;
        }
    }

    private Vector2Int GetRotatedItemSize(ItemStructure item, float rotationZ)
    {
        int normalizedRotation = Mathf.RoundToInt(rotationZ / 90) % 4;
        if (normalizedRotation < 0) normalizedRotation += 4;

        // Для 0 и 180 градусов размеры не меняются
        // Для 90 и 270 градусов размеры меняются местами
        return (normalizedRotation == 1 || normalizedRotation == 3) ?
            new Vector2Int(item.Size.y, item.Size.x) :
            item.Size;
    }

    private Vector2Int GetRotatedItemOffset(bool[,] rotatedShape, Vector2Int rotatedSize)
    {
        int minX = rotatedSize.x;
        int minY = rotatedSize.y;
        bool foundOccupied = false;

        for (int y = 0; y < rotatedSize.y; y++)
        {
            for (int x = 0; x < rotatedSize.x; x++)
            {
                if (rotatedShape[x, y])
                {
                    foundOccupied = true;
                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                }
            }
        }

        // Если не нашли занятых ячеек, возвращаем (0,0)
        return new Vector2Int(foundOccupied ? minX : 0, foundOccupied ? minY : 0);
    }

    private Vector3 CalculateItemCenterPosition(int mainCellIndex, bool[,] rotatedShape, Vector2Int rotatedSize, Vector2Int rotatedOffset)
    {
        int startX = mainCellIndex % GridWidth;
        int startY = mainCellIndex / GridWidth;

        List<Vector3> occupiedPositions = new List<Vector3>();

        for (int y = 0; y < rotatedSize.y; y++)
        {
            for (int x = 0; x < rotatedSize.x; x++)
            {
                if (rotatedShape[x, y])
                {
                    int gridX = startX + x - rotatedOffset.x;
                    int gridY = startY + y - rotatedOffset.y;
                    int index = gridY * GridWidth + gridX;

                    if (index < cells.Count && cells[index] != null)
                    {
                        occupiedPositions.Add(cells[index].transform.position);
                        Debug.Log($"Occupying cell [{gridX}, {gridY}] at index {index}");
                    }
                }
            }
        }

        if (occupiedPositions.Count == 0)
            return cells[mainCellIndex].transform.position;

        // Используем Bounds для точного вычисления центра
        Bounds bounds = new Bounds(occupiedPositions[0], Vector3.zero);
        foreach (Vector3 pos in occupiedPositions)
        {
            bounds.Encapsulate(pos);
        }

        return new Vector3(bounds.center.x, bounds.center.y, 0f);
    }

    // Метод для очистки сохраненных данных
    public void ClearSavedData()
    {
        PlayerPrefs.DeleteKey(settingsKey);
        PlayerPrefs.Save();
        Debug.Log("Saved data cleared");
    }

    // Метод для отладки - показывает текущее состояние ячеек
    public void DebugCurrentState()
    {
        int occupiedCells = 0;
        foreach (Cell cell in cells)
        {
            if (cell.nestedObject != null)
            {
                occupiedCells++;
                Debug.Log($"Cell {cell.name}: {cell.nestedObject.name}");
            }
        }
        Debug.Log($"Occupied cells: {occupiedCells}/{cells.Count}");
    }

    public void ClearAllItems()
    {
        // Очищаем все ячейки
        foreach (Cell cell in cells)
        {
            cell.nestedObject = null;
        }

        // Удаляем все дочерние предметы
        for (int i = itemsParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(itemsParent.GetChild(i).gameObject);
        }
    }

    
}