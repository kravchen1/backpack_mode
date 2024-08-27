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

    public TextMeshProUGUI textPrice;
    
    public ShopData(Item item, TextMeshProUGUI textPrice)
    {
        this.item = item;
        this.textPrice = textPrice;
    }
}
