using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class GenerateShopItems : MonoBehaviour
{
    public List<GameObject> generateItems;
    private GameObject axeCommon2Hand;
    private GameObject sword;
    private GameObject bag4x4;
    private GameObject bag4x4_;
    private GameObject curseSword1Hand;
    private GameObject decoratedLates;
    private GameObject potionHPCommon;
    private GameObject glovesBase;

    private Collider2D placeForItemCollider;

    public List<ShopData> shopData;

    [SerializeField] private TextMeshPro priceTxt;

    

    private void Awake()
    {
        placeForItemCollider = GetComponent<RectTransform>().GetChild(0).GetComponent<Collider2D>();


        axeCommon2Hand = Resources.Load<GameObject>("AxeCommon2Hand");
        generateItems.Add(axeCommon2Hand);
        sword = Resources.Load<GameObject>("SwordStandart");
        generateItems.Add(sword);
        bag4x4 = Resources.Load<GameObject>("bagStandart4x4");
        generateItems.Add(bag4x4);
        curseSword1Hand = Resources.Load<GameObject>("CurseSword1Hand");
        generateItems.Add(curseSword1Hand);
        decoratedLates = Resources.Load<GameObject>("DecoratedLates");
        generateItems.Add(decoratedLates);
        potionHPCommon = Resources.Load<GameObject>("PotionHPCommon");
        generateItems.Add(potionHPCommon);
        glovesBase = Resources.Load<GameObject>("GlovesBase");
        generateItems.Add(glovesBase);
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
        SetItemCost(item);
    }


    void SetItemCost(Item item)
    {
        shopData.Add(new ShopData(item, priceTxt));
        priceTxt.text = item.itemCost.ToString();
    }

    void Start()
    {
       int r;
       //for (int i = 0; i < collidersArray.Length; i++)
       // {
            
            //if(i == 0)
            //{
            //    Generation(sword, collidersArray[i].bounds.center);
            //}
            //else
            //{
            //    Generation(bag4x4, collidersArray[i].bounds.center);
            //}

            //Generation(bag4x4_, collidersArray[i].bounds.center - new Vector3(0,10,0));


            
            r = Random.Range(0, 12);
            if (r < generateItems.Count)
            {
                Generation(generateItems[r], placeForItemCollider.bounds.center);
            }
            else
            {
                Generation(bag4x4, placeForItemCollider.bounds.center);
            }
            
            

        //}

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
