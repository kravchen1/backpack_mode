//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TMPro;
//using UnityEngine;

//public class TotalGroupPrice : MonoBehaviour
//{
//    public TextMeshProUGUI totalPriceText;

//    public GameObject filter;
//    public Sprite spriteToInsert;
//    private int totalPrice = 0;


//    public void GetTotalPrice()
//    {
//        totalPrice = filter.GetComponentsInChildren<ItemInFilter>()
//                    .Where(e => e.coinImage.enabled)
//                    .Sum(e => (int)(e.item.GetComponent<Item>().itemCost / 2));
//        //int spriteIndex = totalPriceText.spriteAsset.GetSpriteIndexFromName(spriteToInsert.name);
//        totalPriceText.text = $"{totalPrice.ToString()} <sprite name={spriteToInsert.name}>";
//    }
    
//    private void Update()
//    {
//        GetTotalPrice();
//    }
//}
