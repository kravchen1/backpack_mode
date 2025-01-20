using System;
using System.Collections.Generic;

[Serializable]
public class ForgeData
{
    public int indexFrame;

    public string prefabName;
    
    public ForgeData(int indexFrame, string prefabName)
    {
        this.indexFrame = indexFrame;
        this.prefabName = prefabName;
    }
}

public class ListForgeData
{
    public List<ForgeData> listForgeData;
}
