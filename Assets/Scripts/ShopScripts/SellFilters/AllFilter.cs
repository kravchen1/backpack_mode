using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AllFilter : MonoBehaviour
{
    public List<GameObject> listFilter;
    private bool allSelected = false;


    public void SelectAllItemInFilter()
    {
        var allFilterItems = new List<ItemInFilter>();
        foreach (var filter in listFilter)
        {
            allFilterItems.AddRange(filter.GetComponentsInChildren<ItemInFilter>().ToList());
        }
        var activeItems = allFilterItems.Where(item => item.coinImage.enabled).ToList();


        if(activeItems.Count == allFilterItems.Count())
            allSelected = true;
        else
            allSelected = false;

        if (activeItems.Count > 0 && !allSelected)
        {
            var unselectedItems = allFilterItems.Except(activeItems).ToList();

            foreach (var item in unselectedItems)
            {
                item.coinImage.enabled = true;
                ItemsForSale.items.Add(item.item);
            }
            allSelected = true;
        }
        else
        {
            if (!allSelected)
            {
                foreach (var item in allFilterItems)
                {
                    item.coinImage.enabled = true;
                    ItemsForSale.items.Add(item.item);
                }
                allSelected = true;
            }
            else
            {
                foreach (var item in allFilterItems)
                {
                    item.coinImage.enabled = false;
                    ItemsForSale.items.Remove(item.item);
                }
                allSelected = false;
            }
        }

    }
}