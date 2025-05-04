using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GenerateShopItems : MonoBehaviour
{
    public List<GameObject> generateItems;
    private GameObject[] prefabs;
    private Collider2D placeForItemCollider;
    private Collider2D placeForChanterelleCollider;
    private Collider2D placeForWhiteCollider;
    public ShopData shopData;

    public List<string> tagItems;

    [SerializeField] private TextMeshPro priceTxt;

    public string savePath = "shopData";


    private List<ShopSaveData> listShopData;

    public void LoadItems(string tagName)
    {
        if (prefabs == null)
        {
            prefabs = PrefabsManager._cachedPrefabs;//Resources.LoadAll<GameObject>("Items/");
        }
        generateItems.AddRange(prefabs.Where(e => e.tag.ToUpper() == tagName).ToList());
    }

    private void Awake()
    {
        placeForChanterelleCollider = GetComponent<RectTransform>().GetChild(0).GetComponent<Collider2D>();

        placeForWhiteCollider = GetComponent<RectTransform>().GetChild(0).GetComponent<Collider2D>();

        placeForItemCollider = GetComponent<RectTransform>().GetChild(0).GetComponent<Collider2D>();

        //placeForChanterelleCollider = GetComponent<RectTransform>().GetChild(0).GetComponent<Collider2D>();
        //placeForWhiteCollider = GetComponent<RectTransform>().GetChild(0).GetComponent<Collider2D>();
        foreach (var tag in tagItems)
        {
            LoadItems(tag);
        }
        //("RAREWEAPON");
        //LoadChestItems("BAG");
        //LoadChestItems("BLOCKMANAITEM");

        //LoadChestItems("STARTBAG");



    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Generation(GameObject generationObject, Vector3 place)
    {
        var generationObjectShop = Instantiate(generationObject, place, Quaternion.identity, GetComponent<RectTransform>().parent.transform);
        var animatorAS = generationObjectShop.GetComponent<AnimationStart>();
        if (animatorAS != null)
        {
            animatorAS.Play();
        }
        for (int i = 0; i < generationObjectShop.transform.childCount; i++)
        {
            generationObjectShop.transform.GetChild(i).gameObject.name = generationObjectShop.transform.GetChild(i).gameObject.name + Random.Range(0, 10000);
        }
        generationObjectShop.name = generationObject.name + Random.Range(0, 10000);

        var item = generationObjectShop.GetComponent<Item>();
        item.prefabOriginalName = generationObject.name;

        generationObjectShop.AddComponent<ShopItem>();
        generationObjectShop.GetComponent<ShopItem>().defaultPosition = place;
        //Debug.Log(generationObjectShop.transform.position);
        generationObjectShop.transform.position = place;
        SetItemCost(item, generationObject.name);
    }


    void SetItemCost(Item item, string prefabName)
    {
        shopData = new ShopData(item, priceTxt, gameObject.name, prefabName);
        priceTxt.text = item.itemCost.ToString();
    }

    private GameObject GenerateChanterelleItem()
    {
        foreach (GameObject item in generateItems)
        {
            if (item.name == "MushroomChanterelle")
            {
                return item;
            }
        }
        return generateItems[0];
    }

    private GameObject GenerateWhiteItem()
    {
        foreach (GameObject item in generateItems)
        {
            if (item.name == "MushroomWhite")
            {
                return item;
            }
        }
        return generateItems[0];
    }

    private GameObject GenerateFirstItem()
    {
        foreach (GameObject item in generateItems)
        {
            if (item.name == "HiddenDagger")
            {
                return item;
            }
        }
        return generateItems[0];
    }
    public void GenerateRandomItem()
    {
        int r;
        r = Random.Range(0, generateItems.Count);
        if (r < generateItems.Count)
        {
            {
                if (!PlayerPrefs.HasKey("FirstGenerationShopItem") && SceneManager.GetActiveScene().name == "BackPackShop" && name == "Price1")
                {
                    Generation(GenerateFirstItem(), placeForItemCollider.bounds.center);
                }
                else if (!PlayerPrefs.HasKey("FirstGenerationShopItem") && SceneManager.GetActiveScene().name == "BackPackShop" && name == "Price2")
                {
                    Generation(GenerateWhiteItem(), placeForWhiteCollider.bounds.center);
                }
                else if (!PlayerPrefs.HasKey("FirstGenerationShopItem") && SceneManager.GetActiveScene().name == "BackPackShop" && name == "Price3")
                {
                    Generation(GenerateChanterelleItem(), placeForChanterelleCollider.bounds.center);
                    PlayerPrefs.SetInt("FirstGenerationShopItem", 1);
                }
                else
                {
                    Generation(generateItems[r], placeForItemCollider.bounds.center);
                }
            }
            //else
            //{
            //    Generation(generateItems[r], placeForItemCollider.bounds.center);
            //}
        }
    }

    void LoadSlot()
    {
        var shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<Shop>();
        shop.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), savePath + ".json"));
        listShopData = shop.listShopSaveData.listShopSaveData;
        if (listShopData != null)
        {
            foreach (var sd in listShopData.Where(e => e.slotName == gameObject.name))
            {
                foreach (var generateItem in generateItems)
                {
                    if (generateItem.name == sd.prefabName)
                    {
                        Generation(generateItem, placeForItemCollider.bounds.center);
                        GetComponent<Price>().LockItem(sd.locking);
                    }
                }
                //
            }
        }
    }

    void StartInv()
    {
        LoadSlot();
        if (listShopData == null || listShopData.Count == 0)
            GenerateRandomItem();
    }

    void Start()
    {
        Invoke("StartInv", 0.01f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}