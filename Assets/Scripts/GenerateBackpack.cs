using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class GenerateBackpack : MonoBehaviour
{
    private BackpackData backpackData;

    void Start()
    {
        backpackData = GetComponent<BackpackData>();
        backpackData.LoadData();
        //Invoke("GenerationBackpack", 1.0f);
        GenerationBackpack();
    }
    void Generation(GameObject generationObject, Vector3 place, Quaternion rotation)//уволен
    {
        var generationObjectItem = Instantiate(generationObject, place, rotation, gameObject.transform);
        for (int i = 0; i < generationObject.transform.childCount; i++)
        {
            generationObjectItem.transform.GetChild(i).gameObject.name = generationObjectItem.transform.GetChild(i).gameObject.name + Random.Range(0, 10000);
        }
        generationObjectItem.name = generationObject.name + Random.Range(0, 10000);

        generationObjectItem.GetComponent<Item>().prefabOriginalName = generationObject.name;

        generationObjectItem.transform.SetParent(GetComponent<RectTransform>());
        generationObjectItem.transform.localPosition = place;
        //var z = GameObject.Find("backpack");
        generationObjectItem.GetComponent<Item>().hits = new List<RaycastHit2D>();
        Physics2D.SyncTransforms();
        if (generationObjectItem.GetComponent<Item>().prefabOriginalName.ToUpper().Contains("BAG"))
        {
            generationObjectItem.GetComponent<Bag>().RaycastEvent();
            generationObjectItem.GetComponent<Bag>().SetNestedObject();
        }
        else
        {
            generationObjectItem.GetComponent<Item>().hitsForBackpack = new List<RaycastHit2D>();
            generationObjectItem.GetComponent<Item>().RaycastEvent();
            generationObjectItem.GetComponent<Item>().SetNestedObject();
            generationObjectItem.GetComponent<Item>().ChangeColorToDefault();
        }
        //generationObjectItem.GetComponent<Bag>().SetNestedObject();
        //generationObjectItem.GetComponent<Item>().RaycastEvent();
    }
    public void GenerationBackpack()
    {
        if(backpackData.itemData.items.Count != 0 )
        {
            foreach(var item in backpackData.itemData.items)
            {
                Generation(Resources.Load<GameObject>(item.name), item.position, item.rotation);
            }
        }
    }

}
