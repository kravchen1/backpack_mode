//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;
//using static Item;


//public class SellFilters : MonoBehaviour
//{
//    public GameObject storage;
//    public GameObject filteredItemPrefab;


//    public ItemRarity rarity;

//    public List<Item> filteredItems = new List<Item>();
//    List<ItemType> excludedTypes = new List<ItemType> { ItemType.Bag, ItemType.Stone, ItemType.Food };
//    public void FilterByRarity(ItemRarity rarity)
//    {
//        filteredItems.Clear();
//        filteredItems = storage.GetComponentsInChildren<Item>().Where(item => item.rarity == rarity && !excludedTypes.Contains(item.itemType)).OrderBy(item => item.originalName).ToList();
//    }


//    public void FilterByItemType(ItemType itemType)
//    {
//        filteredItems.Clear();
//        filteredItems = storage.GetComponentsInChildren<Item>().Where(item => item.itemType == itemType).OrderBy(item => item.originalName).ToList();
//    }

//    public virtual void GetFilter()
//    {
//        FilterByRarity(rarity);
//        foreach (Transform child in transform)
//        {
//            Destroy(child.gameObject);
//        }
//        foreach (var item in filteredItems)
//        {
//            filteredItemPrefab.GetComponentInChildren<TextMeshProUGUI>().text = item.originalName;
//            var newItem = Instantiate(filteredItemPrefab, gameObject.transform);
//            newItem.GetComponent<ItemInFilter>().item = item.gameObject;
//        }
//    }
//}

