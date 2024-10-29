using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class GenerateShopItems : MonoBehaviour
{
    public List<GameObject> generateItems;
    private GameObject axeCommon2Hand;
    private GameObject sword;
    private GameObject bagStartFire, bagStartIce;
    private GameObject curseSword1Hand;
    private GameObject decoratedLates;
    private GameObject potionHPCommon;
    private GameObject glovesBase;

    private Collider2D placeForItemCollider;

    public ShopData shopData;

    [SerializeField] private TextMeshPro priceTxt;


    private List<ShopSaveData> listShopData;
    

    private void Awake()
    {
        placeForItemCollider = GetComponent<RectTransform>().GetChild(0).GetComponent<Collider2D>();


        axeCommon2Hand = Resources.Load<GameObject>("AxeCommon2Hand");
        generateItems.Add(axeCommon2Hand);
        sword = Resources.Load<GameObject>("SwordStandart");
        generateItems.Add(sword);
        bagStartFire = Resources.Load<GameObject>("bagStartFire");
        generateItems.Add(bagStartFire);
        bagStartIce = Resources.Load<GameObject>("bagStartIce");
        generateItems.Add(bagStartFire);
        curseSword1Hand = Resources.Load<GameObject>("CurseSword1Hand");
        generateItems.Add(curseSword1Hand);
        decoratedLates = Resources.Load<GameObject>("DecoratedLates");
        generateItems.Add(decoratedLates);
        potionHPCommon = Resources.Load<GameObject>("PotionHPCommon");
        generateItems.Add(potionHPCommon);
        glovesBase = Resources.Load<GameObject>("GlovesBase");
        generateItems.Add(glovesBase);
        generateItems.Add(Resources.Load<GameObject>("CurseDoubleSword"));
        
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
        r = Random.Range(0, 12);
        if (r < generateItems.Count)
        {
            Generation(generateItems[r], placeForItemCollider.bounds.center);
        }
        else
        {
            Generation(bagStartIce, placeForItemCollider.bounds.center);
        }
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
