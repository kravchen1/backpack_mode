using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TradeButton : MonoBehaviour
{
    public TradeGenerator tradeGenerator;

    public List<GameObject> backpacks;
    public GameObject backpackCanvasForThisButton;

    public void ChooseTradeInventory()
    {
        foreach (var backpack in backpacks)
        {
            backpack.transform.localScale = Vector3.zero;
        }
        backpackCanvasForThisButton.transform.localScale = Vector3.one;
    }

    public void ClearAllItems()
    {
        tradeGenerator.ClearItems();
    }

    public void Generate(float boost = 0f)
    {
        tradeGenerator.GenerateItems(boost);
    }

}