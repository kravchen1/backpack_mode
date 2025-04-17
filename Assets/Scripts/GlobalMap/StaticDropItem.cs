using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static Item;


public class StaticDropItem : DropItem
{
    private void Awake()
    {
        if (PlayerPrefs.GetInt("FindStaticDropItem" + gameObject.name, 0) > 0)
        {
            PlayerPrefs.SetInt("FindStaticDropItem" + gameObject.name, PlayerPrefs.GetInt("FindStaticDropItem" + gameObject.name) + 1);
            if(PlayerPrefs.GetInt("FindStaticDropItem" + gameObject.name) > 10)
            {
                PlayerPrefs.SetInt("FindStaticDropItem" + gameObject.name, 0);
            }
            Destroy(gameObject);
        }
        else
        {
            gameObject.GetComponent<CircleCollider2D>().enabled = true;
        }
    }

    public override void Activate()
    {
        PlayerPrefs.SetInt("FindStaticDropItem" + gameObject.name, 1);
        giveItem(item.name);
        SetStorageWeigth(item.GetComponent<Item>().weight);
        //player.animator.play("giveItem");
        Destroy(gameObject);
    }
}

