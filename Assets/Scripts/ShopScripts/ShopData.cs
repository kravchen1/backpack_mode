using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[Serializable]
public class ShopData
{
    public Item item;

    public TextMeshPro textPrice;

    public string slotName;

    public string prefabName;
    
    public ShopData(Item item, TextMeshPro textPrice, string slotName, string prefabName)
    {
        this.item = item;
        this.textPrice = textPrice;
        this.slotName = slotName;
        this.prefabName = prefabName;
    }
}


