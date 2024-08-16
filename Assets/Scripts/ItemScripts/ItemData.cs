using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemData
{
    public string itemName;
    public Vector2 itemPosition;

    public ItemData(string itemName, Vector2 itemPosition)
    {
        this.itemName = itemName;
        this.itemPosition = itemPosition;
    }
}
