using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NotShopZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("ShopEnter");
        var shopItem = collision.gameObject.GetComponent<ShopItem>();
        if (shopItem != null)
        {
            shopItem.isNotShopZone = true;
        }
    }
}

