using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class GenerateShopItems : MonoBehaviour
{
    public List<GameObject> generateItems;
    private GameObject[] prefabs;

    private Collider2D placeForItemCollider;

    public ShopData shopData;

    [SerializeField] private TextMeshPro priceTxt;


    private List<ShopSaveData> listShopData;

    public void LoadChestItems(string tagName)
    {
        if (prefabs == null)
        {
            prefabs = Resources.LoadAll<GameObject>("");
        }
        generateItems.AddRange(prefabs.Where(e => e.tag.ToUpper() == tagName).ToList());
    }

    private void Awake()
    {
        placeForItemCollider = GetComponent<RectTransform>().GetChild(0).GetComponent<Collider2D>();

        LoadChestItems("RAREWEAPON");



    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Generation(GameObject generationObject, Vector3 place)
    {
        var generationObjectShop = Instantiate(generationObject, place, Quaternion.identity, GetComponent<RectTransform>().parent.transform);
        for (int i = 0; i < generationObjectShop.transform.childCount; i++)
        {
            generationObjectShop.transform.GetChild(i).gameObject.name = generationObjectShop.transform.GetChild(i).gameObject.name + Random.Range(0, 10000);
        }
        generationObjectShop.name = generationObject.name + Random.Range(0, 10000);

        var item = generationObjectShop.GetComponent<Item>();
        item.prefabOriginalName = generationObject.name;
        
        generationObjectShop.AddComponent<ShopItem>();
        SetItemCost(item, generationObject.name);
    }


    void SetItemCost(Item item, string prefabName)
    {
        shopData = new ShopData(item, priceTxt, gameObject.name, prefabName);
        priceTxt.text = item.itemCost.ToString();
    }

    public void GenerateRandomItem()
    {
        int r;
        r = Random.Range(0, generateItems.Count);
        if (r < generateItems.Count)
        {
            Generation(generateItems[r], placeForItemCollider.bounds.center);
        }
        //else
        //{
        //    Generation(generateItems[r], placeForItemCollider.bounds.center);
        //}
    }

    void LoadSlot()
    {
        var shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<Shop>();
        shop.LoadData("Assets/Saves/shopData.json");
        listShopData = shop.listShopSaveData.listShopSaveData;
        if (listShopData != null)
        {
            foreach (var sd in listShopData.Where(e => e.slotName == gameObject.name))
            {
                Generation(Resources.Load<GameObject>(sd.prefabName), placeForItemCollider.bounds.center);
                GetComponent<Price>().LockItem(sd.locking);
            }
        }
    }

    void Start()
    {
        LoadSlot();
        if (listShopData == null || listShopData.Count == 0)
            GenerateRandomItem();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
