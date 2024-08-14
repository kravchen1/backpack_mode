using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class GenerateShopItems : MonoBehaviour
{
    private GameObject axe;
    private GameObject sword;
    private GameObject bag4x4;

    private Collider2D[] collidersArray;

    private void Awake()
    {
        collidersArray = GetComponent<RectTransform>().GetComponents<Collider2D>();


        axe = Resources.Load<GameObject>("Axe"); 
        sword = Resources.Load<GameObject>("SwordStandart");
        bag4x4 = Resources.Load<GameObject>("bagStandart4x4");


    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Generation(GameObject generationObject, Vector3 place)
    {
        var generationObjectShop = Instantiate(generationObject, place, Quaternion.identity, GetComponent<RectTransform>());
        
    }


    void Start()
    {
       int r;
       for (int i = 0; i < collidersArray.Length; i++)
        {
            r = Random.Range(0, 3);
            switch (r)
            {
                case 0:
                    Generation(axe, collidersArray[i].bounds.center);
                    break;
                case 1:
                    Generation(sword, collidersArray[i].bounds.center);
                    break;
                case 2:
                    Generation(bag4x4, collidersArray[i].bounds.center);
                    break;
            }
           
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
