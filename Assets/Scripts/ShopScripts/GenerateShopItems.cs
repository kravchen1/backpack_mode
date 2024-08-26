using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class GenerateShopItems : ShopData
{
    public List<GameObject> generateItems;
    private GameObject axeCommon2Hand;
    private GameObject sword;
    private GameObject bag4x4;
    private GameObject bag4x4_;

    private Collider2D[] collidersArray;


    [SerializeField] private TextMeshProUGUI leftPrice;
    [SerializeField] private TextMeshProUGUI rightPrice;

    

    private void Awake()
    {
        collidersArray = GetComponent<RectTransform>().GetComponents<Collider2D>();


        axeCommon2Hand = Resources.Load<GameObject>("AxeCommon2Hand"); 
        sword = Resources.Load<GameObject>("SwordStandart");
        bag4x4 = Resources.Load<GameObject>("bagStandart4x4");
        bag4x4_ = Resources.Load<GameObject>("bagStandart4x4_1");


    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Generation(GameObject generationObject, Vector3 place, int colliderId)
    {
        var generationObjectShop = Instantiate(generationObject, place, Quaternion.identity, GetComponent<RectTransform>().parent.transform);
        for (int i = 0; i < generationObjectShop.transform.childCount; i++)
        {
            generationObjectShop.transform.GetChild(i).gameObject.name = generationObjectShop.transform.GetChild(i).gameObject.name + Random.Range(0, 10000);
        }
        generationObjectShop.name = generationObject.name + Random.Range(0, 10000);

        var item = generationObjectShop.GetComponent<Item>();
        item.prefabOriginalName = generationObject.name;
        shopItems.Add(item);
        SetItemCost(item, colliderId);
    }


    void SetItemCost(Item item, int colliderId)
    {
        switch (colliderId)
        {
            case 0:
                leftPrice.text = item.itemCost.ToString();
                break;
            case 1:
                rightPrice.text = item.itemCost.ToString();
                break;
        }

    }

    void Start()
    {
       int r;
       for (int i = 0; i < collidersArray.Length; i++)
        {
            
            //if(i == 0)
            //{
            //    Generation(sword, collidersArray[i].bounds.center);
            //}
            //else
            //{
            //    Generation(bag4x4, collidersArray[i].bounds.center);
            //}

            //Generation(bag4x4_, collidersArray[i].bounds.center - new Vector3(0,10,0));

            r = Random.Range(0, generateItems.Count);
            Generation(generateItems[r], collidersArray[i].bounds.center, i);
            
            

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
