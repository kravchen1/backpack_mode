using System;
using System.Collections.Generic;

[Serializable]
public class DataCellJson
{
    public string cellName;
    public string cellNestedObjectName;
    public float rotationZ;
    public List<string> occupiedCells = new List<string>();

    public ItemQuality qualityKey;
    public float durability;
    public int countStack = 0;
    public float weight = 0;

    public bool isUseFight = true;

    public DataCellJson(string cellName, string cellNestedObjectName, float rotationZ, List<string> occupiedCells, ItemQuality qualityKey, float durability, int countStack, float weight, bool isUseFight)
    {
        this.cellName = cellName;
        this.cellNestedObjectName = cellNestedObjectName;
        this.rotationZ = rotationZ;
        this.occupiedCells = occupiedCells;
        this.qualityKey = qualityKey;
        this.durability = durability;
        this.countStack = countStack;
        this.weight = weight;
        this.isUseFight = isUseFight;
    }


    public DataCellJson(string cellName, string cellNestedObjectName, float rotationZ, List<string> occupiedCells, ItemQuality qualityKey, float durability, float weight, bool isUseFight)
    {
        this.cellName = cellName;
        this.cellNestedObjectName = cellNestedObjectName;
        this.rotationZ = rotationZ;
        this.occupiedCells = occupiedCells;
        this.qualityKey = qualityKey;
        this.durability = durability;
        this.weight = weight;
        this.isUseFight = isUseFight;
    }
}