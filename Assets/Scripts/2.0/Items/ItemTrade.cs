using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemTrade : MonoBehaviour
{
    private ItemStats itemStats;
    [SerializeField] private Color _PriceColor = Color.yellow;
    private TextMeshPro priceText;

    [Header("Copy System")]
    public List<GameObject> linkedCopies = new List<GameObject>();
    public GameObject originalItem; // ссылка на оригинальный предмет

    private void Awake()
    {
        itemStats = GetComponent<ItemStats>();
        itemStats.isShowDurability = false;
        priceText = transform.Find("Durability")?.GetComponent<TextMeshPro>();

        if( priceText != null )
        {
            priceText.color = _PriceColor;
            priceText.text = $"{itemStats.price:0}";
        }
    }
}