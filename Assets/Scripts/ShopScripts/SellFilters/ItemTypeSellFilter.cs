using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Item;


public class ItemTypeSellFilter : SellFilters
{
    public ItemType itemType;

    //private void Start()
    //{
    //    GetFilter();
    //}

    public override void GetFilter()
    {
        FilterByItemType(itemType);
        foreach (Transform child in transform)
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

