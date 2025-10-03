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

    [HideInInspector] public List<GameObject> itemPrefabs = new List<GameObject>();

    private DataJsonCellList dataJsonList = new DataJsonCellList();
    private const int GridWidth = 10;
    private bool skipNextSave = false;

    private void Awake()
    {
        itemPrefabs = GameObject.FindGameObjectWithTag("ItemsPrefabs").GetComponent<ItemPrefabs>().itemPrefabs;
    }

    private void OnEnable()
    {
        if (settingsKey == "InventoryData")
        {
            StartCoroutine(LoadDataDelayed());
        }
    }

    private IEnumerator LoadDataDelayed()
    {
        yield return null;
        yield return null;
        yield return null;
        LoadData();
    }

    private IEnumerator StarsPerformRaycastCheck(List<GameObject> loadedObjects)
    {
        yield return null;
        yield return null;
        yield return null;
        //проставл€ем звЄзды в предметах
        foreach (var loadedObject in loadedObjects)
        {
            loadedObject.GetComponent<ItemMove>().StarsPerformRaycastCheck();
        }
    }

    private void OnDisable()
    {
        if (settingsKey == "InventoryData")
        {
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
            if (cell.NestedObject != null && !savedItems.ContainsKey(cell.NestedObject))
            {
                var itemStructure = cell.NestedObject.GetComponent<ItemStructure>();
                var itemStats = cell.NestedObject.GetComponent<ItemStats>();
                var itemMove = cell.NestedObject.GetComponent<ItemMove>();
                if (itemStructure != null)
                {
                    // Ќаходим все €чейки, зан€тые этим предметом
                    List<string> occupiedCellNames = FindAllOccupiedCellsForItem(itemStructure);

                    if (occupiedCellNames.Count > 0)
                    {
                        if (itemMove.IsStackable)
                        {
                            dataJsonList.inventoryDataJsonList.Add(new DataCellJson(
                                cell.gameObject.name, // главна€ €чейка
                                itemStats.itemNameKey,
                                itemStructure.transform.eulerAngles.z,
                                occupiedCellNames,
                                itemStats.itemQuality,
                                itemStats.durability,
                                itemMove.StackCount
                            ));
                        }
                        else
                        {
                            dataJsonList.inventoryDataJsonList.Add(new DataCellJson(
                                                            cell.gameObject.name, // главна€ €чейка
                                                            itemStats.itemNameKey,
                                                            itemStructure.transform.eulerAngles.z,
                                                            occupiedCellNames,
                                                            itemStats.itemQuality,
                                                            itemStats.durability
                                                        ));
                        }

                        savedItems[cell.NestedObject] = true;
                    }
                }
            }
        }

        string jsonCellsSave = JsonUtility.ToJson(dataJsonList);
        PlayerPrefs.SetString(settingsKey, jsonCellsSave);
        PlayerPrefs.Save();

        Debug.Log($"Data saved. Unique items: {dataJsonList.inventoryDataJsonList.Count}");
    }

    private List<string> FindAllOccupiedCellsForItem(ItemStructure item)
    {
        List<string> occupiedCellNames = new List<string>();

        foreach (Cell cell in cells)
        {
            if (cell.NestedObject == item.gameObject)
            {
                occupiedCellNames.Add(cell.gameObject.name);
            }
        }

        Debug.Log($"Item {item.name} occupies cells: {string.Join(", ", occupiedCellNames)}");
        return occupiedCellNames;
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

            List<GameObject> loadedObjects = new List<GameObject>();

            foreach (DataCellJson cellData in dataJsonList.inventoryDataJsonList)
            {
                GameObject itemPrefab = itemPrefabs.Find(p => p.gameObject.name == cellData.cellNestedObjectName);
                if (itemPrefab == null)
                {
                    Debug.LogWarning($"Prefab not found: {cellData.cellNestedObjectName}");
                    continue;
                }

                GameObject newItem = Instantiate(itemPrefab, itemsParent);


                newItem.name = itemPrefab.gameObject.name;
                newItem.transform.rotation = Quaternion.Euler(0, 0, cellData.rotationZ);


                newItem.GetComponent<ItemStats>().durability = cellData.durability;
                newItem.GetComponent<ItemStats>().itemQuality = cellData.qualityKey;
                newItem.GetComponent<ItemStats>().InitializeQuality();

                if (cellData.countStack > 0)
                {
                    newItem.GetComponent<ItemMove>().AddToStack(cellData.countStack-1);
                }

                // –азмещаем предмет в сохраненных €чейках
                PlaceItemInOccupiedCells(newItem, cellData.occupiedCells, cellData.rotationZ);
                loadedObjects.Add(newItem);

                Debug.Log($"Loaded item {cellData.cellNestedObjectName} with rotation {cellData.rotationZ}∞ in {cellData.occupiedCells.Count} cells");
            }

            StartCoroutine(
                        StarsPerformRaycastCheck(loadedObjects));
            Debug.Log("Data loaded successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading data: {e.Message}");
        }
    }

    private void PlaceItemInOccupiedCells(GameObject item, List<string> occupiedCellNames, float rotationZ)
    {
        // ќчищаем €чейки от старых ссылок
        foreach (Cell cell in cells)
        {
            if (cell.NestedObject == item.gameObject)
            {
                cell.NestedObject = null;
            }
        }

        // «анимаем указанные €чейки
        foreach (string cellName in occupiedCellNames)
        {
            Cell cell = cells.Find(c => c.gameObject.name == cellName);
            if (cell != null)
            {
                cell.NestedObject = item.gameObject;
            }
            else
            {
                Debug.LogWarning($"Cell not found: {cellName}");
            }
        }

        // ¬ычисл€ем центр позиции дл€ предмета
        Vector3 centerPosition = CalculateItemCenterPositionFromOccupiedCells(occupiedCellNames);
        item.transform.position = new Vector3(centerPosition.x, centerPosition.y, item.transform.position.z);
    }

    private Vector3 CalculateItemCenterPositionFromOccupiedCells(List<string> occupiedCellNames)
    {
        List<Vector3> occupiedPositions = new List<Vector3>();

        foreach (string cellName in occupiedCellNames)
        {
            Cell cell = cells.Find(c => c.gameObject.name == cellName);
            if (cell != null)
            {
                occupiedPositions.Add(cell.transform.position);
            }
        }

        if (occupiedPositions.Count == 0)
            return Vector3.zero;

        // »спользуем Bounds дл€ точного вычислени€ центра
        Bounds bounds = new Bounds(occupiedPositions[0], Vector3.zero);
        foreach (Vector3 pos in occupiedPositions)
        {
            bounds.Encapsulate(pos);
        }

        return new Vector3(bounds.center.x, bounds.center.y, 0f);
    }

    // ћетод дл€ очистки сохраненных данных
    public void ClearSavedData()
    {
        PlayerPrefs.DeleteKey(settingsKey);
        PlayerPrefs.Save();
        Debug.Log("Saved data cleared");
    }

    // ћетод дл€ отладки - показывает текущее состо€ние €чеек
    public void DebugCurrentState()
    {
        int occupiedCells = 0;
        foreach (Cell cell in cells)
        {
            if (cell.NestedObject != null)
            {
                occupiedCells++;
                Debug.Log($"Cell {cell.name}: {cell.NestedObject.name}");
            }
        }
        Debug.Log($"Occupied cells: {occupiedCells}/{cells.Count}");
    }

    public void ClearAllItems()
    {
        // ќчищаем все €чейки
        foreach (Cell cell in cells)
        {
            cell.NestedObject = null;
        }

        // ”дал€ем все дочерние предметы
        for (int i = itemsParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(itemsParent.GetChild(i).gameObject);
        }
    }
}