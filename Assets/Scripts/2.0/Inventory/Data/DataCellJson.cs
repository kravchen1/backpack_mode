using System;
using System.Collections.Generic;

[Serializable]
public class DataCellJson
{
    public string cellName;
    public string cellNestedObjectName;
    public float rotationZ;

    public DataCellJson(string cellName, string cellNestedObjectName, float rotationZ)
    {
        this.cellName = cellName;
        this.cellNestedObjectName = cellNestedObjectName;
        this.rotationZ = rotationZ;
    }
}