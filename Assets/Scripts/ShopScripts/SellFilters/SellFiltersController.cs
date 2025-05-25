using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;



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
        if (shop != null)
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


        if (item.CanvasDescription != null)
            Destroy(item.CanvasDescription.gameObject);


    }

    public void SellSelectedItems()
    {
        listForDestroy.Clear();
        for (int i = 0; i < storage.transform.childCount; i++)
        {
            var child = storage.transform.GetChild(i);
            if (ItemsForSale.items.Contains(child.gameObject))
            {
                listForDestroy.Add(child.GetComponent<Item>());
                ItemsForSale.items.Remove(child.gameObject);
                var prefabList = content.GetComponentsInChildren<ItemInFilter>().Where(e => e.item == child.gameObject).ToList();
                Destroy(prefabList[0].gameObject);
            }
        }
        foreach (var item in listForDestroy.ToList())
        {

            SellItem(item);

        }
        
        ObjectsDynamic();
        ShowHideFilters();
    }


    void ObjectsDynamic()
    {

        for (int i = 0; i < storage.transform.childCount; i++)
        {

            if (storage.transform.GetChild(i).GetComponent<Item>() != null)
            {
                var item = storage.transform.GetChild(i).GetComponent<Item>();
                item.needToDynamic = true;
                item.timerStatic_locked_out = true;
            }
        }
    }
}
