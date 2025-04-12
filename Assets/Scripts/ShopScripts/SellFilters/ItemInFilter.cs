using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public static class ItemsForSale
{
    public static List<GameObject> items = new List<GameObject>();
}
public class ItemInFilter:MonoBehaviour
{
    public GameObject item;

    public Image coinImage;
    public void SelectItem()
    {
        coinImage.enabled = !coinImage.enabled;
        if(coinImage.enabled)
        {
            ItemsForSale.items.Add(item);
        }
        else
        {
            ItemsForSale.items.Remove(item);
        }

        //Debug.Log(ItemsForSale.items.Count());
    }
}

