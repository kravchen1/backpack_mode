using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[Serializable]
public class ShopSaveData
{
    public string slotName;

    public string prefabName;

    public bool locking;

    public ShopSaveData(string slotName, string prefabName, bool locking)
    {
        this.slotName = slotName;
        this.prefabName = prefabName;
        this.locking = locking;
    }
}

[Serializable]
public class ListShopSaveData
{
    public List<ShopSaveData> listShopSaveData;
}
