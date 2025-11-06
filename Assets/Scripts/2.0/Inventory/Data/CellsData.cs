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
    public GameObject stashPrefab; // Префаб сундука, который появится при создании схрона

    [HideInInspector] public List<GameObject> itemPrefabs = new List<GameObject>();

    private DataJsonCellList dataJsonList = new DataJsonCellList();

    #region Public Properties
    public bool HasSavedData => PlayerPrefs.HasKey(settingsKey) && !string.IsNullOrEmpty(PlayerPrefs.GetString(settingsKey, ""));
    public List<GameObject> LoadedItems { get; private set; } = new List<GameObject>();
    #endregion

    private void Awake()
    {
        itemPrefabs = GameObject.FindGameObjectWithTag("ItemsPrefabs").GetComponent<Prefabs>().prefabs;
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
        PlayerDataManager.Instance.Stats.InitializeCurrentWeight(settingsKey);
    }

    private IEnumerator StarsPerformRaycastCheck(List<GameObject> loadedObjects)
    {
        yield return null;
        yield return null;
        yield return null;
        //проставляем звёзды в предметах
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
        }
    }

    //private void OnApplicationQuit()
    //{
    //    if (settingsKey == "InventoryData")
    //    {
    //        SaveData();
    //    }
    //}

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
                    // Находим все ячейки, занятые этим предметом
                    List<string> occupiedCellNames = FindAllOccupiedCellsForItem(itemStructure);

                    if (occupiedCellNames.Count > 0)
                    {
                        if (itemMove.IsStackable)
                        {
                            dataJsonList.inventoryDataJsonList.Add(new DataCellJson(
                                cell.gameObject.name, // главная ячейка
                                itemStats.itemNameKey,
                                itemStructure.transform.eulerAngles.z,
                                occupiedCellNames,
                                itemStats.itemQuality,
                                itemStats.durability,
                                itemMove.StackCount,
                                itemStats.weight,
                                itemStats.isUseFight
                            ));
                        }
                        else
                        
                        {
                            dataJsonList.inventoryDataJsonList.Add(new DataCellJson(
                                                            cell.gameObject.name, // главная ячейка
                                                            itemStats.itemNameKey,
                                                            itemStructure.transform.eulerAngles.z,
                                                            occupiedCellNames,
                                                            itemStats.itemQuality,
                                                            itemStats.durability,
                                                            itemStats.weight,
                                                            itemStats.isUseFight
                                                        ));
                        }

                        savedItems[cell.NestedObject] = true;
                    }
                }
            }
        }

        string jsonCellsSave = JsonUtility.ToJson(dataJsonList);
        PlayerPrefsMigrationManager.Instance.RegisterStringPref(settingsKey);
        PlayerPrefs.SetString(settingsKey, jsonCellsSave);
        PlayerPrefs.Save();
        
        //Debug.Log($"Data saved. Unique items: {dataJsonList.inventoryDataJsonList.Count}");
        //Debug.Log(jsonCellsSave);
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

        //Debug.Log($"Item {item.name} occupies cells: {string.Join(", ", occupiedCellNames)}");
        return occupiedCellNames;
    }

    public void LoadData()
    {
        ClearAllItems();
        LoadedItems.Clear();
        string jsonData = PlayerPrefs.GetString(settingsKey, "");
        if (string.IsNullOrEmpty(jsonData))
        {
            Debug.Log("No saved data found");
            return;
        }

        try
        {
            dataJsonList = JsonUtility.FromJson<DataJsonCellList>(jsonData);

            Debug.Log($"Loading {dataJsonList.inventoryDataJsonList.Count} items...");

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
                newItem.GetComponent<ItemStats>().isUseFight = cellData.isUseFight;
                newItem.GetComponent<ItemStats>().Initialized();

                if (cellData.countStack > 0)
                {
                    newItem.GetComponent<ItemMove>().AddToStack(cellData.countStack-1);
                }

                if (gameObject.name == "InventoryTradeData" || gameObject.name == "TradeDataAll")
                {
                    newItem.AddComponent<ItemTrade>();
                }

                // Размещаем предмет в сохраненных ячейках
                PlaceItemInOccupiedCells(newItem, cellData.occupiedCells, cellData.rotationZ);
                LoadedItems.Add(newItem);

                //Debug.Log($"Loaded item {cellData.cellNestedObjectName} with rotation {cellData.rotationZ}° in {cellData.occupiedCells.Count} cells");
            }

            StartCoroutine(
                        StarsPerformRaycastCheck(LoadedItems));
            //Debug.Log("Data loaded successfully!");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading data: {e.Message}");
        }
    }

    private void PlaceItemInOccupiedCells(GameObject item, List<string> occupiedCellNames, float rotationZ)
    {
        // Очищаем ячейки от старых ссылок
        foreach (Cell cell in cells)
        {
            if (cell.NestedObject == item.gameObject)
            {
                cell.NestedObject = null;
            }
        }

        // Занимаем указанные ячейки
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

        // Вычисляем центр позиции для предмета
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
        // Очищаем все ячейки
        foreach (Cell cell in cells)
        {
            cell.NestedObject = null;
        }

        // Более надежное удаление всех дочерних объектов
        List<GameObject> childrenToDestroy = new List<GameObject>();
        foreach (Transform child in itemsParent)
        {
            childrenToDestroy.Add(child.gameObject);
        }

        foreach (GameObject child in childrenToDestroy)
        {
            if (child != null)
            {
                Destroy(child);
            }
        }

        // Дополнительная проверка
        if (itemsParent.childCount > 0)
        {
            Debug.LogWarning($"After ClearAllItems, still {itemsParent.childCount} children remaining!");
        }
    }

    public void CreateStash()
    {
        //

        if (stashPrefab != null)
        {

            GameObject chest;
            string _settingsKey = settingsKey;

            _settingsKey += PlayerPrefs.GetFloat("stashID", 1.0f).ToString();
            PlayerPrefsMigrationManager.Instance.RegisterFloatPref("stashID");
            PlayerPrefs.SetFloat("stashID", PlayerPrefs.GetFloat("stashID", 1.0f) + 1f);
            chest = GridObjectManager.Instance.SpawnObject(
                stashPrefab,
                transform.position,
                _settingsKey
            );

            var chestTrigger = chest.GetComponent<ChestDeathTrigger>();//todo
            chestTrigger.settingsKey = _settingsKey;

        }
    }
}