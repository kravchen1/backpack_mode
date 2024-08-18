using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class GenerateShopItems : MonoBehaviour
{
    private GameObject axe;
    private GameObject sword;
    private GameObject bag4x4;
    private GameObject bag4x4_;

    private Collider2D[] collidersArray;

    private void Awake()
    {
        collidersArray = GetComponent<RectTransform>().GetComponents<Collider2D>();


        axe = Resources.Load<GameObject>("Axe"); 
        sword = Resources.Load<GameObject>("SwordStandart");
        bag4x4 = Resources.Load<GameObject>("bagStandart4x4");
        bag4x4_ = Resources.Load<GameObject>("bagStandart4x4_1");


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

        generationObjectShop.GetComponent<Item>().prefabOriginalName = generationObject.name;


    }


    void Start()
    {
       int r;
       for (int i = 0; i < collidersArray.Length; i++)
        {
            
            if(i == 0)
            {
                Generation(sword, collidersArray[i].bounds.center);
            }
            else
            {
                Generation(bag4x4, collidersArray[i].bounds.center);
            }

            //Generation(bag4x4_, collidersArray[i].bounds.center - new Vector3(0,10,0));

            //r = Random.Range(0, 3);
            //switch (r)
            //{

            //    case 0:
            //        Generation(axe, collidersArray[i].bounds.center);
            //        break;
            //    case 1:
            //        Generation(sword, collidersArray[i].bounds.center);
            //        break;
            //    case 2:
            //        Generation(bag4x4, collidersArray[i].bounds.center);
            //        break;
            //}

        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
