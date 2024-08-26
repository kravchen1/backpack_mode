using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopData : MonoBehaviour
{
    public List<Item> shopItems = new List<Item>();

    public CharacterStats characterStats;
    public void BuyItem(Item item)
    {
        var listCharacterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None);
        characterStats = listCharacterStats[0];

        characterStats.playerCoins = characterStats.playerCoins - item.itemCost;

        characterStats.coinsText.text = characterStats.playerCoins.ToString();
    }

    public bool CanBuy(Item item)
    {
        if (characterStats.playerCoins - item.itemCost < 0)
        {
            return false;
        }
        else
            return true;
    }
}
