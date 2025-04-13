using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;



public class SellFiltersController : MonoBehaviour
{
    public GameObject canvasFilters;
    public GameObject storage;
    public GameObject backpack;
    public GameObject shop;

    public GameObject content;

    public CharacterStats characterStats;

    public List<Item> listForDestroy;

    public void ShowHideFilters()
    {
        ItemsForSale.items.Clear();
        canvasFilters.SetActive(!canvasFilters.activeSelf);
        storage.SetActive(!storage.activeSelf);
        backpack.SetActive(!backpack.activeSelf);
        if(shop != null)
            shop.SetActive(!shop.activeSelf);
    }

    private void SellItem(Item item)
    {
        //characterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None)[0];
        decimal preciseWeight = (decimal)characterStats.storageWeight - (decimal)item.weight;
        characterStats.storageWeight = (float)Math.Round(preciseWeight, 2);
        if (SceneManager.GetActiveScene().name != "BackPack")
        {
            characterStats.playerCoins = characterStats.playerCoins + (int)(item.itemCost / 2);
            characterStats.coinsText.text = characterStats.playerCoins.ToString();
        }
        Destroy(item.gameObject);
        Destroy(item.CanvasDescription.gameObject);
    }

    public void SellSelectedItems()
    {
        
        for (int i = 0; i < storage.transform.childCount; i++)
        {
            var child = storage.transform.GetChild(i);
            if (ItemsForSale.items.Contains(child.gameObject))
            {
                listForDestroy.Add(child.GetComponent<Item>());
                ItemsForSale.items.Remove(child.gameObject);
                var prefabList = content.GetComponentsInChildren<ItemInFilter>().Where(e => e.item == child.gameObject).ToList();
                try
                {
                    Destroy(prefabList[0].gameObject);
                }
                catch(Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }
        }
        foreach(var item in listForDestroy)
        {
            SellItem(item);
        }
    }

}

