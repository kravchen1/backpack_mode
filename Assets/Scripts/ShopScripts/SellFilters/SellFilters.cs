using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Item;


public class SellFilters : MonoBehaviour
{
    public GameObject storage;
    public GameObject filteredItemPrefab;


    public ItemRarity rarity;
    public string itemTag = "";

    //public List<Item> allItems = new List<Item>();
    public List<Item> filteredItems = new List<Item>();

    public void FilterByRarity(ItemRarity rarity)
    {
        filteredItems.Clear();
        filteredItems = storage.GetComponentsInChildren<Item>().Where(item => item.rarity == rarity && !item.CompareTag("ItemKeyStone") && !item.CompareTag("ItemEat")).ToList();
        filteredItems = filteredItems.OrderBy(item => item.originalName).ToList();
    }

    public void FilterByTags(string itemTag)
    {
        filteredItems.Clear();
        filteredItems = storage.GetComponentsInChildren<Item>().Where(item => item.CompareTag(itemTag)).ToList();
        filteredItems = filteredItems.OrderBy(item => item.originalName).ToList();
    }


    //private void Start()
    //{
    //    GetFilter();
    //}

    public void GetFilter()
    {
        if (itemTag != "")
            FilterByTags(itemTag);
        else
            FilterByRarity(rarity);
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var item in filteredItems)
        {
            filteredItemPrefab.GetComponentInChildren<TextMeshProUGUI>().text = item.originalName;
            var newItem = Instantiate(filteredItemPrefab, gameObject.transform);
            newItem.GetComponent<ItemInFilter>().item = item.gameObject;
        }
    }
    //public void FilterByCost(float itemCost)
    //{
    //    filteredItems.Clear();
    //    filteredItems = storage.GetComponentsInChildren<Item>().Where(item => item.itemCost <= itemCost).ToList();
    //}

    //public void FilterCommon() => FilterByRarity(ItemRarity.Common);
    //public void FilterRare() => FilterByRarity(ItemRarity.Rare);
    //public void FilterEpic() => FilterByRarity(ItemRarity.Epic);
    //public void FilterLegendary() => FilterByRarity(ItemRarity.Legendary);
    //public void FilterCaveStones() => FilterByTags("ItemKeyStone");
    //public void FilterFood() => FilterByTags("ItemEat");
}

