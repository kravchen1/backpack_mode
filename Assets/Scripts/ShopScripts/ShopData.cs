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
    
    public ShopData(Item item, TextMeshPro textPrice)
    {
        this.item = item;
        this.textPrice = textPrice;
    }
}
