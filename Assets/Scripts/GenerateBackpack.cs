using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GenerateBackpack : MonoBehaviour
{
    protected BackpackData backpackData;

    protected GameObject[] prefabs;
    public List<GameObject> generateItems;//any prefab

    protected List<GameObject> ItemsGenerated;//loaded prefabs

    public EnemyData enemy;
    public void LoadChestItems(string tagName)
    {
        if (prefabs == null)
        {
            prefabs = Resources.LoadAll<GameObject>("Items/");
        }
        generateItems.AddRange(prefabs.Where(e => e.tag.ToUpper() == tagName).ToList());
    }

    protected void Initialization()
    {
        LoadChestItems("BAGNOCOMMON");
        LoadChestItems("STARTBAG");
        LoadChestItems("BAG");



        LoadChestItems("ITEMEAT");



        LoadChestItems("ITEMCRYSTALL");


        LoadChestItems("ITEMCLOTH");


        LoadChestItems("ITEMFIRE");
        LoadChestItems("ITEMFIREWEAPON");
        LoadChestItems("ITEMFIRECLOTH");
        LoadChestItems("ITEMFIRECLOTHGLOVES");


        LoadChestItems("ITEMMANA");
        LoadChestItems("ITEMMANAWEAPON");
        LoadChestItems("ITEMMANACLOTH");
        LoadChestItems("ITEMMANACLOTHGLOVES");
        LoadChestItems("ITEMMANABLOCK");


        LoadChestItems("ITEMMUSHROOM");


        LoadChestItems("ITEMKEYSTONE");


        LoadChestItems("ITEMSTUFF");


        LoadChestItems("ITEMVAMPIRE");
        LoadChestItems("ITEMVAMPIREWEAPON");
        LoadChestItems("ITEMVAMPIRECLOTH");
        LoadChestItems("ITEMVAMPIRECLOTHGLOVES");


        LoadChestItems("ITEMWEAPON");
        LoadChestItems("ITEMWITHCRAFT");

        LoadChestItems("ITEMPET");
        LoadChestItems("ITEMPETBLOCK");

        LoadChestItems("ITEMDRAGONWEAPON");
    }

    void Start()
    {
        Time.timeScale = 1f;
        backpackData = GetComponent<BackpackData>();
        ItemsGenerated = new List<GameObject>();
        switch (gameObject.name)
        {
            case "backpack":
                backpackData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "backpackData.json"));
                break;
            case "Storage":
                backpackData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json"));
                break;
            case "CaveStone":
                backpackData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "caveStoneData.json"));
                break;
            case "warehouse1":
                backpackData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "backpackWarehouse1.json"));
                break;
            case "warehouse2":
                backpackData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "backpackWarehouse2.json"));
                break;
            case "warehouse3":
                backpackData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "backpackWarehouse3.json"));
                break;
            case "warehouse4":
                backpackData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "backpackWarehouse4.json"));
                break;
            case "warehouse5":
                backpackData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "backpackWarehouse5.json"));
                break;


            case "backpackEnemyCanvas":
                backpackData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "backpackData.json"));
                break;


            case "backpackEnemy":
                getEnemy();
                break;

        }


        Initialization();

        GenerationBackpack();
    }

    void getEnemy()
    {
        string enemyJSON = "";
        if (PlayerPrefs.HasKey("enemyBackpackJSON"))
        {
            enemyJSON = PlayerPrefs.GetString("enemyBackpackJSON");
        }
        else
        {
            enemyJSON = "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":39.6099853515625,\"y\":-81.04045104980469,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}";
        }
        backpackData.LoadDataEnemy(enemyJSON);
    }
    void Generation(GameObject generationObject, Vector3 place, Quaternion rotation)//уволен
    {
        GameObject generationObjectItem = Instantiate(generationObject, place, rotation, gameObject.transform);
        ItemsGenerated.Add(generationObjectItem);

        for (int i = 0; i < generationObject.transform.childCount; i++)
        {
            generationObjectItem.transform.GetChild(i).gameObject.name = generationObjectItem.transform.GetChild(i).gameObject.name + Random.Range(0, 10000);
        }
        generationObjectItem.name = generationObject.name + Random.Range(0, 10000);

        var componentItem = generationObjectItem.GetComponent<Item>();
        var componentBag = generationObjectItem.GetComponent<Bag>();
        componentItem.prefabOriginalName = generationObject.name;

        generationObjectItem.transform.SetParent(GetComponent<RectTransform>());
        generationObjectItem.transform.localPosition = place;
        //var z = GameObject.Find("backpack");
        componentItem.hits = new List<HitsStructure>();
        Physics2D.SyncTransforms();
        if (componentItem.prefabOriginalName.ToUpper().Contains("BAG"))
        {
            componentBag.RaycastEvent();
            componentBag.SetNestedObject();
            componentBag.updateColorCells();
            gameObject.transform.SetAsFirstSibling();
        }
        else
        {
            componentItem.hitsForBackpack = new List<RaycastHit2D>();
            componentItem.RaycastEvent();
            componentItem.updateColorCells();
            if (componentItem.CorrectEndPoint() && ObjectInBag(componentItem))
                componentItem.SetNestedObject();
            componentItem.ChangeColorToDefault();
        }
        //generationObjectItem.GetComponent<Bag>().SetNestedObject();
        //generationObjectItem.GetComponent<Item>().RaycastEvent();
        if(generationObjectItem.transform.parent.name == "Storage")
        {
            componentItem.needToDynamic = true;
            componentItem.Impulse = true;
            //componentItem.MoveObjectOnEndDrag();
            //componentBag.MoveObjectOnEndDrag();
            if(!ObjectInBag(componentItem))
            {
                var pos = gameObject.transform.position;
                componentItem.CouratineMove(new Vector3(pos.x, pos.y, -2));
            }
        }
        else
        {
            componentItem.rb.excludeLayers = (1 << 9) | (1 << 10);
        }

        //componentItem.needToDynamic = false; todo
        componentItem.timerStatic = 6f;

    }
    public void GenerationBackpack()
    {
        if (backpackData.itemData.items.Count != 0)
        {
            foreach (var item in backpackData.itemData.items)
            {
                foreach (var generateItem in generateItems)
                {
                    if (generateItem.name == item.name)
                    {
                        Generation(generateItem, item.position, item.rotation);
                    }
                }
                
            }
        }
    }

    public bool ObjectInBag(Item item)
    {
        var rect = gameObject.GetComponent<RectTransform>().rect;
        if (item.transform.localPosition.x > rect.max.x || item.transform.localPosition.y > rect.max.y || item.transform.localPosition.x < rect.min.x || item.transform.localPosition.y < rect.min.y)
        {
            return false;
        }
        else
            return true;
    }

    

}


