using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridObjectManager : MonoBehaviour
{
    [Header("Grid Reference")]
    [SerializeField] private Grid tilemapGrid;

    [Header("Pre-Occupied Cells")]
    [SerializeField] private List<MultiCellObject> preOccupiedCells = new List<MultiCellObject>();

    [Header("Save Settings")]
    [SerializeField] private string saveKey = "GridWorldData";
    [SerializeField] private bool autoSave = true;

    [Header("Object Creation Settings")]
    [SerializeField] private GameObject defaultOccupiedObjectPrefab; // Префаб для создания
    [SerializeField] private Transform spawnedObjectsParent; // Родитель для созданных объектов

    private Dictionary<Vector3Int, GameObject> occupiedCells = new Dictionary<Vector3Int, GameObject>();
    private Dictionary<string, MultiCellObject> multiCellObjects = new Dictionary<string, MultiCellObject>();

    [HideInInspector] public List<GameObject> environmentPrefabs = new List<GameObject>();

    public static GridObjectManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (tilemapGrid == null)
        {
            tilemapGrid = FindObjectOfType<Grid>();
        }

        if (environmentPrefabs.Count == 0)
        {
            var prefabsContainer = GameObject.FindGameObjectWithTag("EnvironmentPrefabs");
            if (prefabsContainer != null)
            {
                environmentPrefabs = prefabsContainer.GetComponent<Prefabs>().prefabs;
            }
        }

        LoadWorldData();

        // Регистрация предзанятых клеток
        RegisterPreOccupiedCells();

        CreateObjectsFromData();
    }

    // Исправленный метод для регистрации предзанятых клеток
    private void RegisterPreOccupiedCells()
    {
        foreach (var preOccupied in preOccupiedCells)
        {
            if (preOccupied.gameObject != null && !IsPrefabAsset(preOccupied.gameObject))
            {
                Vector3Int mainCell = WorldToCellPosition(preOccupied.gameObject.transform.position);
                RegisterMultiCellObject(preOccupied.gameObject, mainCell, preOccupied.size, preOccupied.objectId);
            }
        }
    }

    // Проверка, является ли GameObject префабом в assets
    private bool IsPrefabAsset(GameObject obj)
    {
        return obj.scene.rootCount == 0 || obj.scene.name == null;
    }

    #region Object Creation Methods

    // Создание объектов на сцене по данным из multiCellObjects и occupiedCells
    [ContextMenu("Create Objects From Data")]
    public void CreateObjectsFromData()
    {
        ClearAllCreatedObjects();
        CreateMultiCellObjects();
        CreateSingleCellObjects();
    }

    // Создание многоклеточных объектов
    private void CreateMultiCellObjects()
    {
        if (!PlayerPrefs.HasKey(saveKey)) return;

        try
        {
            string jsonData = PlayerPrefs.GetString(saveKey);
            GridData saveData = JsonUtility.FromJson<GridData>(jsonData);

            foreach (var multiCellData in saveData.multiCellObjects)
            {
                CreateMultiCellObjectFromData(multiCellData);
            }

            Debug.Log($"Created {saveData.multiCellObjects.Count} multi-cell objects from data");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create multi-cell objects from data: {e.Message}");
        }
    }

    // Создание одного многоклеточного объекта из данных
    private void CreateMultiCellObjectFromData(MultiCellObjectData data)
    {
        // Ищем префаб по имени
        GameObject prefab = FindPrefabByName(data.objectName);
        if (prefab == null)
        {
            Debug.LogWarning($"Prefab not found for: {data.objectName}, using default");
            prefab = defaultOccupiedObjectPrefab;
        }

        if (prefab == null)
        {
            Debug.LogError($"Default prefab not set, cannot create object: {data.objectName}");
            return;
        }

        // Используем сохраненные координаты ячейки напрямую
        Vector3Int cellPosition = data.mainCell.ToVector3Int();
        Vector3 worldPosition = CalculateObjectCenter(cellPosition, data.size.ToVector2Int());

        Debug.Log($"Creating object '{data.objectName}' at saved cell {cellPosition}, calculated world position {worldPosition}");

        GameObject newObject = Instantiate(prefab, worldPosition, Quaternion.identity);
        newObject.name = data.objectName;

        // Восстанавливаем settingsItemsShopKey
        if (!string.IsNullOrEmpty(data.settingsKey))
        {
            SetSettingsKey(newObject, data.settingsKey);
        }

        // Восстанавливаем кастомную переменную isWasActive
        SetIsWasActiveToObject(newObject, data.isWasActive);

        // Устанавливаем родителя только если объект создан на сцене
        if (spawnedObjectsParent != null && !IsPrefabAsset(newObject))
        {
            newObject.transform.SetParent(spawnedObjectsParent);
        }

        // Регистрируем объект в системе - передаем сохраненные координаты ячейки
        if (RegisterMultiCellObject(newObject, cellPosition, data.size.ToVector2Int(), data.objectId))
        {
            Debug.Log($"Created multi-cell object: {data.objectName} at cell {cellPosition}");
        }
        else
        {
            Debug.LogWarning($"Failed to register multi-cell object: {data.objectName} at cell {cellPosition}");
            // Пытаемся найти свободное место
            Vector3Int? freeSpace = FindFreeSpaceForObject(data.size.ToVector2Int(), cellPosition);
            if (freeSpace.HasValue)
            {
                Vector3 newPosition = CalculateObjectCenter(freeSpace.Value, data.size.ToVector2Int());
                newObject.transform.position = newPosition;

                if (RegisterMultiCellObject(newObject, freeSpace.Value, data.size.ToVector2Int(), data.objectId))
                {
                    Debug.Log($"Created multi-cell object at alternative position: {data.objectName} at cell {freeSpace.Value}");
                }
                else
                {
                    Destroy(newObject);
                }
            }
            else
            {
                Destroy(newObject);
            }
        }
    }

    // Создание одиночных объектов
    private void CreateSingleCellObjects()
    {
        if (!PlayerPrefs.HasKey(saveKey)) return;

        try
        {
            string jsonData = PlayerPrefs.GetString(saveKey);
            GridData saveData = JsonUtility.FromJson<GridData>(jsonData);

            int createdCount = 0;
            foreach (var cellData in saveData.occupiedCells)
            {
                // Пропускаем ячейки, которые уже заняты многоклеточными объектами
                if (IsCellOccupied(cellData.cellPosition.ToVector3Int()))
                    continue;

                if (CreateSingleCellObjectFromData(cellData))
                {
                    createdCount++;
                }
            }

            Debug.Log($"Created {createdCount} single-cell objects from data");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create single-cell objects from data: {e.Message}");
        }
    }

    // Создание одного одиночного объекта из данных
    private bool CreateSingleCellObjectFromData(OccupiedCellData data)
    {
        // Ищем префаб по имени
        GameObject prefab = FindPrefabByName(data.objectName);
        if (prefab == null)
        {
            Debug.LogWarning($"Prefab not found for: {data.objectName}, using default");
            prefab = defaultOccupiedObjectPrefab;
        }

        if (prefab == null)
        {
            Debug.LogError($"Default prefab not set, cannot create object: {data.objectName}");
            return false;
        }

        // Создаем объект
        Vector3Int cellPosition = data.cellPosition.ToVector3Int();
        Vector3 worldPosition = CellToWorldPosition(cellPosition); // Используем позицию ячейки

        GameObject newObject = Instantiate(prefab, worldPosition, Quaternion.identity);
        newObject.name = data.objectName;

        // Восстанавливаем settingsItemsShopKey
        if (!string.IsNullOrEmpty(data.settingsKey))
        {
            SetSettingsKey(newObject, data.settingsKey);
        }

        // Восстанавливаем кастомную переменную isWasActive
        SetIsWasActiveToObject(newObject, data.isWasActive);

        // Устанавливаем родителя только если объект создан на сцене
        if (spawnedObjectsParent != null && !IsPrefabAsset(newObject))
        {
            newObject.transform.SetParent(spawnedObjectsParent);
        }

        // Регистрируем объект
        if (RegisterObjectAtCellPosition(cellPosition, newObject))
        {
            Debug.Log($"Created single-cell object: {data.objectName} at {worldPosition}");
            return true;
        }
        else
        {
            Debug.LogWarning($"Failed to register single-cell object: {data.objectName}");
            // Пытаемся найти свободную ячейку рядом
            Vector3Int? freeCell = FindNearestFreeCell(cellPosition);
            if (freeCell.HasValue)
            {
                Vector3 newPosition = CellToWorldPosition(freeCell.Value); // Используем позицию ячейки
                newObject.transform.position = newPosition;

                if (RegisterObjectAtCellPosition(freeCell.Value, newObject))
                {
                    Debug.Log($"Created single-cell object at alternative position: {data.objectName} at {newPosition}");
                    return true;
                }
                else
                {
                    Destroy(newObject);
                }
            }
            else
            {
                Destroy(newObject);
            }
        }

        return false;
    }

    // Поиск префаба по имени в environmentPrefabs
    private GameObject FindPrefabByName(string prefabName)
    {
        // Убираем возможные суффиксы "(Clone)" если объект был клонирован
        string cleanName = prefabName.Replace("(Clone)", "").Trim();

        foreach (GameObject prefab in environmentPrefabs)
        {
            if (prefab != null && prefab.name == cleanName)
            {
                return prefab;
            }
        }

        // Если не нашли по точному имени, попробуем найти по частичному совпадению
        foreach (GameObject prefab in environmentPrefabs)
        {
            if (prefab != null && prefab.name.Contains(cleanName))
            {
                return prefab;
            }
        }

        return null;
    }

    // Очистка всех созданных объектов
    [ContextMenu("Clear Created Objects")]
    public void ClearAllCreatedObjects()
    {
        // Очищаем только созданные объекты, но сохраняем preOccupiedCells
        var objectsToRemove = new List<string>();

        foreach (var multiCellObj in multiCellObjects.Values)
        {
            // Удаляем только объекты, которые были созданы через систему (не preOccupied)
            if (multiCellObj.gameObject != null &&
                !preOccupiedCells.Any(pre => pre.objectId == multiCellObj.objectId))
            {
                Destroy(multiCellObj.gameObject);
                objectsToRemove.Add(multiCellObj.objectId);
            }
        }

        // Удаляем из словаря
        foreach (string objectId in objectsToRemove)
        {
            multiCellObjects.Remove(objectId);
        }

        // Очищаем occupiedCells от созданных объектов
        var cellsToRemove = new List<Vector3Int>();
        foreach (var cell in occupiedCells)
        {
            if (cell.Value != null && IsCreatedObject(cell.Value))
            {
                cellsToRemove.Add(cell.Key);
            }
        }

        foreach (var cell in cellsToRemove)
        {
            occupiedCells.Remove(cell);
        }

        Debug.Log($"Cleared {objectsToRemove.Count} created objects");
    }

    // Проверка, был ли объект создан системой (не preOccupied)
    private bool IsCreatedObject(GameObject obj)
    {
        return !preOccupiedCells.Any(pre => pre.gameObject == obj);
    }

    private string GetSettingsKey(GameObject obj)
    {
        // Попробуйте разные компоненты, где может храниться settingsItemsShopKey
        var shopComponent = obj.GetComponent<EnvironmentTrigger>();
        if (shopComponent != null) return shopComponent.settingsKey;

        //var environmentObject = obj.GetComponent<EnvironmentObject>();
        //if (environmentObject != null) return environmentObject.settingsItemsShopKey;

        //var lootContainer = obj.GetComponent<LootContainer>();
        //if (lootContainer != null) return lootContainer.settingsItemsShopKey;

        // Добавьте другие компоненты по необходимости
        return string.Empty;
    }

    private void SetSettingsKey(GameObject obj, string key)
    {
        if (string.IsNullOrEmpty(key)) return;

        // Устанавливаем значение в первый найденный подходящий компонент
        var shopComponent = obj.GetComponent<EnvironmentTrigger>();
        if (shopComponent != null)
        {
            shopComponent.settingsKey = key;
            return;
        }

        //var environmentObject = obj.GetComponent<EnvironmentObject>();
        //if (environmentObject != null)
        //{
        //    environmentObject.settingsItemsShopKey = shopKey;
        //    return;
        //}

        //var lootContainer = obj.GetComponent<LootContainer>();
        //if (lootContainer != null)
        //{
        //    lootContainer.settingsItemsShopKey = shopKey;
        //    return;
        //}

        // Если компонент не найден, можно добавить его автоматически
        Debug.LogWarning($"No component found to set settingsItemsShopKey for {obj.name}");
    }

    #endregion

    #region Multi-Cell Object Methods

    // Исправленный метод регистрации с проверкой на префабы
    public bool RegisterMultiCellObject(GameObject obj, Vector3Int mainCell, Vector2Int size, string objectId = null)
    {
        // Проверяем, свободны ли все нужные ячейки
        if (!AreCellsFreeForObject(mainCell, size))
        {
            Debug.LogWarning($"Cells are occupied for object {obj.name} at position {mainCell}");
            return false;
        }

        // Создаем ID если не предоставлен
        string id = objectId ?? System.Guid.NewGuid().ToString();

        // Создаем запись об объекте
        MultiCellObject multiCellObj = new MultiCellObject(id, obj, mainCell, size);
        multiCellObjects[id] = multiCellObj;

        // Занимаем все ячейки
        foreach (var cell in multiCellObj.occupiedCells)
        {
            occupiedCells[cell] = obj;
        }

        // Устанавливаем родителя только если объект не префаб
        if (spawnedObjectsParent != null && obj.transform.parent == null && !IsPrefabAsset(obj))
        {
            obj.transform.SetParent(spawnedObjectsParent);
        }

        // Позиционируем объект в центре занимаемой области
        Vector3 objectCenter = CalculateObjectCenter(mainCell, size);
        obj.transform.position = objectCenter;

        if (autoSave) SaveWorldData();

        Debug.Log($"Multi-cell object registered: {obj.name} ({size.x}x{size.y}) at {mainCell}");
        return true;
    }

    // Проверка свободны ли ячейки для объекта заданного размера
    public bool AreCellsFreeForObject(Vector3Int mainCell, Vector2Int size)
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int checkCell = mainCell + new Vector3Int(x, y, 0);
                if (IsCellOccupied(checkCell))
                {
                    return false;
                }
            }
        }
        return true;
    }

    // Поиск свободного места для объекта заданного размера
    public Vector3Int? FindFreeSpaceForObject(Vector2Int size, Vector3Int searchAround, int maxRadius = 20)
    {
        // Проверяем центральную позицию
        if (AreCellsFreeForObject(searchAround, size))
            return searchAround;

        // Поиск по спирали
        for (int radius = 1; radius <= maxRadius; radius++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (Mathf.Abs(x) == radius || Mathf.Abs(y) == radius)
                    {
                        Vector3Int checkPos = searchAround + new Vector3Int(x, y, 0);
                        if (AreCellsFreeForObject(checkPos, size))
                            return checkPos;
                    }
                }
            }
        }

        return null;
    }

    // Расчет центра объекта для позиционирования
    public Vector3 CalculateObjectCenter(Vector3Int mainCell, Vector2Int size)
    {
        Vector3 centerWorldPos = CellToWorldPosition(mainCell);

        Debug.Log($"CalculateObjectCenter: cell {mainCell} -> world {centerWorldPos}, grid: {tilemapGrid}");

        // Если объект занимает одну ячейку - просто возвращаем позицию ячейки
        if (size.x == 1 && size.y == 1)
        {
            return centerWorldPos;
        }

        // Для многоклеточных объектов вычисляем центр занимаемой области
        Vector3Int lastCell = mainCell + new Vector3Int(size.x - 1, size.y - 1, 0);
        Vector3 lastCellWorldPos = CellToWorldPosition(lastCell);

        // Центр между первой и последней ячейкой
        Vector3 objectCenter = (centerWorldPos + lastCellWorldPos) * 0.5f;

        return objectCenter;
    }

    // Удаление многоклеточного объекта
    public void UnregisterMultiCellObject(string objectId)
    {
        if (multiCellObjects.TryGetValue(objectId, out MultiCellObject obj))
        {
            // Освобождаем все занятые ячейки
            foreach (var cell in obj.occupiedCells)
            {
                occupiedCells.Remove(cell);
            }

            multiCellObjects.Remove(objectId);

            if (autoSave) SaveWorldData();
        }
    }

    // Получение объекта по ID
    public MultiCellObject GetMultiCellObject(string objectId)
    {
        multiCellObjects.TryGetValue(objectId, out MultiCellObject obj);
        return obj;
    }

    #endregion

    #region Save/Load Methods

    [ContextMenu("Save World Data")]
    public void SaveWorldData()
    {
        try
        {
            GridData saveData = new GridData();
            saveData.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Сохраняем занятые ячейки
            foreach (var cell in occupiedCells)
            {
                if (cell.Value != null)
                {
                    // Получаем значение isWasActive из объекта
                    bool isWasActive = GetIsWasActiveFromObject(cell.Value);
                    var cellData = new OccupiedCellData(cell.Key, cell.Value, isWasActive);
                    cellData.settingsKey = GetSettingsKey(cell.Value); // Добавляем ключ
                    saveData.occupiedCells.Add(cellData);
                }
            }

            // И для многоклеточных объектов:
            foreach (var multiCellObj in multiCellObjects.Values)
            {
                if (multiCellObj.gameObject != null)
                {
                    // Получаем значение isWasActive из объекта
                    bool isWasActive = GetIsWasActiveFromObject(multiCellObj.gameObject);
                    var multiCellData = new MultiCellObjectData(
                        multiCellObj.objectId,
                        multiCellObj.gameObject,
                        multiCellObj.mainCell,
                        multiCellObj.size,
                        multiCellObj.occupiedCells,
                        isWasActive
                    );
                    multiCellData.settingsKey = GetSettingsKey(multiCellObj.gameObject); // Добавляем ключ
                    saveData.multiCellObjects.Add(multiCellData);
                }
            }

            string jsonData = JsonUtility.ToJson(saveData, true);
            PlayerPrefs.SetString(saveKey, jsonData);
            PlayerPrefs.Save();

            Debug.Log($"World data saved: {saveData.occupiedCells.Count} occupied cells, {saveData.multiCellObjects.Count} multi-cell objects");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save world data: {e.Message}");
        }
    }

    [ContextMenu("Load World Data")]
    public void LoadWorldData()
    {
        try
        {
            if (!PlayerPrefs.HasKey(saveKey))
            {
                Debug.Log("No world data found to load");
                return;
            }

            string jsonData = PlayerPrefs.GetString(saveKey);
            GridData saveData = JsonUtility.FromJson<GridData>(jsonData);

            // Очищаем текущие данные (кроме preOccupiedCells)
            var objectsToRemove = new List<string>();
            foreach (var kvp in multiCellObjects)
            {
                if (!preOccupiedCells.Any(pre => pre.objectId == kvp.Key))
                {
                    objectsToRemove.Add(kvp.Key);
                }
            }

            foreach (string objectId in objectsToRemove)
            {
                multiCellObjects.Remove(objectId);
            }

            // Очищаем occupiedCells от созданных объектов
            var cellsToRemove = new List<Vector3Int>();
            foreach (var cell in occupiedCells)
            {
                if (!preOccupiedCells.Any(pre => pre.occupiedCells.Contains(cell.Key)))
                {
                    cellsToRemove.Add(cell.Key);
                }
            }

            foreach (var cell in cellsToRemove)
            {
                occupiedCells.Remove(cell);
            }

            // Восстанавливаем многоклеточные объекты
            foreach (var multiCellData in saveData.multiCellObjects)
            {
                // Пропускаем если объект уже существует как preOccupied
                if (preOccupiedCells.Any(pre => pre.objectId == multiCellData.objectId))
                    continue;

                GameObject obj = GameObject.Find(multiCellData.objectName + "(Clone)");
                if (obj == null)
                {
                    // Объект еще не создан на сцене, пропускаем - он будет создан через CreateObjectsFromData
                    continue;
                }

                MultiCellObject multiCellObj = new MultiCellObject(
                    multiCellData.objectId,
                    obj,
                    multiCellData.mainCell.ToVector3Int(),
                    multiCellData.size.ToVector2Int()
                );

                multiCellObjects[multiCellData.objectId] = multiCellObj;

                // Занимаем ячейки
                foreach (var cell in multiCellObj.occupiedCells)
                {
                    occupiedCells[cell] = obj;
                }

                // Позиционируем объект
                obj.transform.position = CalculateObjectCenter(multiCellData.mainCell.ToVector3Int(), multiCellData.size.ToVector2Int());
            }

            Debug.Log($"World data loaded: {saveData.multiCellObjects.Count} multi-cell objects");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load world data: {e.Message}");
        }
    }

    #endregion

    #region Basic Grid Methods

    private bool GetIsWasActiveFromObject(GameObject obj)
    {
        // Проверяем различные компоненты, где может быть isWasActive
        var interactiveObject = obj.GetComponent<EnvironmentTrigger>();
        if (interactiveObject != null) return interactiveObject.isWasActive;

        
        // Добавьте другие компоненты по необходимости
        Debug.LogWarning($"No component with isWasActive found for {obj.name}");
        return false;
    }

    private void SetIsWasActiveToObject(GameObject obj, bool isWasActive)
    {
        // Устанавливаем значение в первый найденный подходящий компонент
        var interactiveObject = obj.GetComponent<EnvironmentTrigger>();
        if (interactiveObject != null)
        {
            interactiveObject.isWasActive = isWasActive;
            return;
        }
        // Если компонент не найден, логируем предупреждение
        Debug.LogWarning($"No component found to set isWasActive for {obj.name}");
    }

    public Vector3Int WorldToCellPosition(Vector3 worldPosition)
    {
        return tilemapGrid?.WorldToCell(worldPosition) ?? Vector3Int.FloorToInt(worldPosition);
    }

    public Vector3 CellToWorldPosition(Vector3Int cellPosition)
    {
        if (tilemapGrid != null)
        {
            Vector3 worldPos = tilemapGrid.GetCellCenterWorld(cellPosition);
            Debug.Log($"CellToWorldPosition: {cellPosition} -> {worldPos}, grid pos: {tilemapGrid.transform.position}");
            return worldPos;
        }
        else
        {
            Debug.LogWarning("Grid reference is null!");
            return (Vector3)cellPosition;
        }
    }

    public bool IsCellOccupied(Vector3Int cellPosition)
    {
        return occupiedCells.ContainsKey(cellPosition);
    }

    public bool RegisterObjectAtCellPosition(Vector3Int cellPosition, GameObject obj)
    {
        if (IsCellOccupied(cellPosition)) return false;

        occupiedCells[cellPosition] = obj;

        // Устанавливаем родителя если указан
        if (spawnedObjectsParent != null && obj.transform.parent == null)
        {
            obj.transform.SetParent(spawnedObjectsParent);
        }

        if (autoSave) SaveWorldData();
        return true;
    }

    public Vector3Int? FindNearestFreeCell(Vector3Int center, int maxRadius = 10)
    {
        if (!IsCellOccupied(center))
            return center;

        for (int radius = 1; radius <= maxRadius; radius++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (Mathf.Abs(x) == radius || Mathf.Abs(y) == radius)
                    {
                        Vector3Int checkPos = center + new Vector3Int(x, y, 0);
                        if (!IsCellOccupied(checkPos))
                            return checkPos;
                    }
                }
            }
        }

        return null;
    }


    // Основной метод для создания объекта после смерти монстра
    public GameObject SpawnObjectAfterDeath(GameObject objectPrefab, Vector3 deathPosition, string settingsKey = null, string objectId = null, bool isWasActive = false)
    {
        Vector3Int deathCell = WorldToCellPosition(deathPosition);

        if (AreCellsFreeForObject(deathCell, new Vector2Int(1, 1)))
        {
            return CreateObjectAtCell(objectPrefab, deathCell, objectId, settingsKey, isWasActive);
        }
        else
        {
            Vector3Int? freeCell = FindNearestFreeCell(deathCell);
            if (freeCell.HasValue)
            {
                Debug.Log($"Position occupied at {deathCell}, spawning at nearby free cell {freeCell.Value}");
                return CreateObjectAtCell(objectPrefab, freeCell.Value, objectId, settingsKey, isWasActive);
            }
            else
            {
                Debug.LogWarning($"No free cells found near death position {deathPosition}");
                return null;
            }
        }
    }

    // Вспомогательный метод для создания объекта в конкретной ячейке
    private GameObject CreateObjectAtCell(GameObject prefab, Vector3Int cellPosition, string objectId = null, string settingsKey = null, bool isWasActive = false)
    {
        Vector3 worldPosition = CellToWorldPosition(cellPosition);
        GameObject newObject = Instantiate(prefab, worldPosition, Quaternion.identity, spawnedObjectsParent);
        newObject.name = prefab.name;

        // Устанавливаем settingsItemsShopKey если передан
        if (!string.IsNullOrEmpty(settingsKey))
        {
            SetSettingsKey(newObject, settingsKey);
        }
        // Устанавливаем кастомную переменную isWasActive
        SetIsWasActiveToObject(newObject, isWasActive);

        // Регистрируем объект
        if (RegisterObjectAtCellPosition(cellPosition, newObject))
        {
            Debug.Log($"Created object {prefab.name} at cell {cellPosition}");
            return newObject;
        }
        else
        {
            Debug.LogError($"Failed to register object {prefab.name} at cell {cellPosition}");
            Destroy(newObject);
            return null;
        }
    }

    // Для многоклеточных объектов (если нужно создать объект размером больше 1x1)
    public GameObject SpawnMultiCellObjectAfterDeath(GameObject objectPrefab, Vector3 deathPosition, Vector2Int size, string settingsKey = null, string objectId = null, bool isWasActive = false)
    {
        Vector3Int deathCell = WorldToCellPosition(deathPosition);

        // Пытаемся создать объект на позиции смерти
        if (AreCellsFreeForObject(deathCell, size))
        {
            return CreateMultiCellObjectAtCell(objectPrefab, deathCell, size, objectId, settingsKey, isWasActive);
        }
        else
        {
            // Ищем ближайшее свободное место для объекта заданного размера
            Vector3Int? freeSpace = FindFreeSpaceForObject(size, deathCell);
            if (freeSpace.HasValue)
            {
                Debug.Log($"Position occupied at {deathCell}, spawning at nearby free space {freeSpace.Value}");
                return CreateMultiCellObjectAtCell(objectPrefab, freeSpace.Value, size, objectId, settingsKey, isWasActive);
            }
            else
            {
                Debug.LogWarning($"No free space found for {size.x}x{size.y} object near death position {deathPosition}");
                return null;
            }
        }
    }

    // Вспомогательный метод для создания многоклеточного объекта
    private GameObject CreateMultiCellObjectAtCell(GameObject prefab, Vector3Int mainCell, Vector2Int size, string objectId = null, string settingsKey = null, bool isWasActive = false)
    {
        Vector3 worldPosition = CalculateObjectCenter(mainCell, size);
        GameObject newObject = Instantiate(prefab, worldPosition, Quaternion.identity, spawnedObjectsParent);
        newObject.name = prefab.name;

        // Устанавливаем settingsItemsShopKey если передан
        if (!string.IsNullOrEmpty(settingsKey))
        {
            SetSettingsKey(newObject, settingsKey);
        }

        // Устанавливаем кастомную переменную isWasActive
        SetIsWasActiveToObject(newObject, isWasActive);

        // Регистрируем многоклеточный объект
        if (RegisterMultiCellObject(newObject, mainCell, size, objectId))
        {
            Debug.Log($"Created multi-cell object {prefab.name} ({size.x}x{size.y}) at cell {mainCell}");
            return newObject;
        }
        else
        {
            Debug.LogError($"Failed to register multi-cell object {prefab.name} at cell {mainCell}");
            Destroy(newObject);
            return null;
        }
    }


    [ContextMenu("Clear All Data")]
    public void ClearAllData()
    {
        occupiedCells.Clear();
        multiCellObjects.Clear();

        if (autoSave) SaveWorldData();
    }

    #endregion

    private void OnApplicationQuit()
    {
        SaveWorldData();
    }
}