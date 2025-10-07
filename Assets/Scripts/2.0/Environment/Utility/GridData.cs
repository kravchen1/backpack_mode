using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridData
{
    public List<OccupiedCellData> occupiedCells = new List<OccupiedCellData>();
    public List<MultiCellObjectData> multiCellObjects = new List<MultiCellObjectData>();
    public string timestamp;
}

[System.Serializable]
public class MultiCellObjectData
{
    public string objectId;
    public string objectName;
    public Vector3IntData mainCell;
    public List<Vector3IntData> occupiedCells;
    public Vector3Data worldPosition;
    public Vector2IntData size;
    public string settingsKey; // Добавляем сохранение ключа

    public MultiCellObjectData() { }

    public MultiCellObjectData(string id, GameObject obj, Vector3Int mainCellPos, Vector2Int objectSize, List<Vector3Int> cells)
    {
        objectId = id;
        objectName = obj.name;
        mainCell = new Vector3IntData(mainCellPos);
        size = new Vector2IntData(objectSize);
        worldPosition = new Vector3Data(obj.transform.position);

        // Сохраняем settingsItemsShopKey если есть компонент
        var shopComponent = obj.GetComponent<EnvironmentTrigger>(); // или другой компонент, где хранится settingsItemsShopKey
        if (shopComponent != null)
        {
            settingsKey = shopComponent.settingsKey;
        }

        occupiedCells = new List<Vector3IntData>();
        foreach (var cell in cells)
        {
            occupiedCells.Add(new Vector3IntData(cell));
        }
    }
}

[System.Serializable]
public class Vector2IntData
{
    public int x;
    public int y;

    public Vector2IntData() { }

    public Vector2IntData(Vector2Int vector)
    {
        x = vector.x;
        y = vector.y;
    }

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int(x, y);
    }
}
[System.Serializable]
public class OccupiedCellData
{
    public Vector3IntData cellPosition;
    public string objectName;
    public Vector3Data worldPosition;
    public string settingsKey; // Добавляем для одиночных объектов

    public OccupiedCellData() { }

    public OccupiedCellData(Vector3Int cellPos, GameObject obj)
    {
        cellPosition = new Vector3IntData(cellPos);
        objectName = obj.name;
        worldPosition = new Vector3Data(obj.transform.position);

        // Сохраняем settingsItemsShopKey если есть
        var shopComponent = obj.GetComponent<EnvironmentTrigger>(); // или другой компонент
        if (shopComponent != null)
        {
            settingsKey = shopComponent.settingsKey;
        }
    }
}

[System.Serializable]
public class Vector3IntData
{
    public int x;
    public int y;
    public int z;

    public Vector3IntData() { }

    public Vector3IntData(Vector3Int vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3Int ToVector3Int()
    {
        return new Vector3Int(x, y, z);
    }
}

[System.Serializable]
public class Vector3Data
{
    public float x;
    public float y;
    public float z;

    public Vector3Data() { }

    public Vector3Data(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

[System.Serializable]
public class PreOccupiedCell
{
    public Vector3Int cellPosition;
    public GameObject gameObject;
}


[System.Serializable]
    public class MultiCellObject
    {
        public string objectId;
        public GameObject gameObject;
        public Vector3Int mainCell;
        public Vector2Int size;
        public List<Vector3Int> occupiedCells;

        public MultiCellObject(string id, GameObject obj, Vector3Int mainCellPos, Vector2Int objectSize)
        {
            objectId = id;
            gameObject = obj;
            mainCell = mainCellPos;
            size = objectSize;
            occupiedCells = CalculateOccupiedCells(mainCellPos, objectSize);
        }

        private List<Vector3Int> CalculateOccupiedCells(Vector3Int mainCell, Vector2Int size)
        {
            List<Vector3Int> cells = new List<Vector3Int>();

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    cells.Add(mainCell + new Vector3Int(x, y, 0));
                }
            }

            return cells;
        }
    }