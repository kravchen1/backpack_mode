using System;
using System.Collections.Generic;

[Serializable]
public class DataCellJson
{
    public string cellName;
    public string cellNestedObjectName;
    public float rotationZ;
    public List<string> occupiedCells = new List<string>();

    public DataCellJson(string cellName, string cellNestedObjectName, float rotationZ, List<string> occupiedCells)
    {
        this.cellName = cellName;
        this.cellNestedObjectName = cellNestedObjectName;
        this.rotationZ = rotationZ;
        this.occupiedCells = occupiedCells;
    }
}