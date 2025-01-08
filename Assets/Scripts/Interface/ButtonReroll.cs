using System.IO;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ButtonReroll : Button
{
    private int countRerollBeforePriceIncrease = 10;
    private int countIncrease = 1;
    public TextMeshProUGUI textPrice;

    private CharacterStats stat;
    private void Awake()
    {
        stat = GameObject.FindGameObjectWithTag("Stat").GetComponent<CharacterStats>();


        if (!PlayerPrefs.HasKey("countRerollBeforePriceIncrease"))
        {
            PlayerPrefs.SetInt("countRerollBeforePriceIncrease", 0);
        }

        if (!PlayerPrefs.HasKey("priceReroll"))
        {
            PlayerPrefs.SetInt("priceReroll", 1);
        }
        else
        {
            textPrice.text = PlayerPrefs.GetInt("priceReroll").ToString();
        }
    }

    override public void OnMouseUpAsButton()
    {
        if (stat.playerCoins - PlayerPrefs.GetInt("priceReroll") >= 0)
        {
            var listShopData = GameObject.FindObjectsByType<GenerateShopItems>(FindObjectsSortMode.None);
            bool rerolling = false;
            foreach (var data in listShopData)
            {
                if (!data.GetComponent<Price>().lockForItem.activeSelf)
                {
                    if (data.shopData.item != null)
                    {
                        Destroy(data.shopData.item.gameObject);
                        data.GenerateRandomItem();
                        rerolling = true;
                    }
                    else
                    {
                        data.GenerateRandomItem();
                        rerolling = true;
                    }
                }
            }
            if (rerolling)
            {
                stat.playerCoins -= PlayerPrefs.GetInt("priceReroll");
                PlayerPrefs.SetInt("countRerollBeforePriceIncrease", PlayerPrefs.GetInt("countRerollBeforePriceIncrease") + 1);

                if (PlayerPrefs.GetInt("countRerollBeforePriceIncrease") == countRerollBeforePriceIncrease)
                {
                    PlayerPrefs.SetInt("countRerollBeforePriceIncrease", 0);
                    PlayerPrefs.SetInt("priceReroll", PlayerPrefs.GetInt("priceReroll") + countIncrease);
                }


                textPrice.text = PlayerPrefs.GetInt("priceReroll").ToString();
            }


        }
    }
}
