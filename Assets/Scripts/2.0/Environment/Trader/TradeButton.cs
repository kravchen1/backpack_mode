using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TradeButton : MonoBehaviour
{
    public TradeController tradeGenerator;

    public List<GameObject> backpacks;
    public GameObject backpackCanvasForThisButton;

    public void ChooseTradeInventory()
    {
        foreach (var backpack in backpacks)
        {
            backpack.GetComponent<RectTransform>().localPosition = new Vector3(4000f, 0f, 0f);
        }
        backpackCanvasForThisButton.GetComponent<RectTransform>().localPosition = new Vector3(0f, 0f, 0f);
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