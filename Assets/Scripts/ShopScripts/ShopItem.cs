using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ShopItem : MonoBehaviour
{
    public CharacterStats characterStats;
    public bool isNotShopZone = false;

    public Vector3 defaultPosition;

    public void BuyItem(Item item)
    {
        var listCharacterStats = GameObject.FindObjectsByType<CharacterStats>(FindObjectsSortMode.None);
        characterStats = listCharacterStats[0];
        var buyItemSound = GameObject.FindGameObjectWithTag("BuyItemSound");
        buyItemSound.GetComponent<AudioSource>().Play();
        characterStats.playerCoins = characterStats.playerCoins - item.itemCost;

        characterStats.coinsText.text = characterStats.playerCoins.ToString();
        var listShopData = GameObject.FindObjectsByType<GenerateShopItems>(FindObjectsSortMode.None);
        foreach (var data in listShopData)
        {
            if (data.shopData.item == item)
            {
                data.shopData.textPrice.text = "";
                data.GetComponent<Price>().LockItem(false);
                data.shopData.item = null;
                data.shopData.prefabName = "";
                data.shopData.slotName = "";
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
