using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ShopItem : MonoBehaviour
{
    public CharacterStats characterStats;
    public void BuyItem(Item item)
    {
        var listCharacterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None);
        characterStats = listCharacterStats[0];
        
        characterStats.playerCoins = characterStats.playerCoins - item.itemCost;

        characterStats.coinsText.text = characterStats.playerCoins.ToString();
        var listShopData = GameObject.FindObjectsByType<GenerateShopItems>(FindObjectsSortMode.None);
        foreach (var data in listShopData)
        {
            foreach (var obj in data.shopData.Where(e => e.textPrice.isActiveAndEnabled))
            {
                if(obj.item == item)
                    obj.textPrice.gameObject.SetActive(false);
            }
        }
        Destroy(GetComponent<ShopItem>());
    }

    public bool CanBuy(Item item)
    {
        var listCharacterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None);
        characterStats = listCharacterStats[0];
        if (characterStats.playerCoins - item.itemCost < 0)
        {
            return false;
        }
        else
            return true;
    }
}
