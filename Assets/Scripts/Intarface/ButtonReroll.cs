using System.IO;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonReroll : Button
{
    private void Awake()
    {
        
    }

    override public void OnMouseUpAsButton()
    {
        var listShopData = GameObject.FindObjectsByType<GenerateShopItems>(FindObjectsSortMode.None);
        foreach (var data in listShopData)
        {
            if (!data.GetComponent<Price>().lockForItem.activeSelf)
            {
                if (data.shopData.item != null)
                {
                    Destroy(data.shopData.item.gameObject);
                    data.GenerateRandomItem();
                }
                else
                {
                    data.GenerateRandomItem();
                }
            }
        }

    }
}
