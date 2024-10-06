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
        Time.timeScale = 1f;
        backpackData = GetComponent<BackpackData>();

        switch (gameObject.name)
        {
            case "backpack":
                backpackData.LoadData("Assets/Saves/backpackData.json");
                break;
            case "Storage":
                backpackData.LoadData("Assets/Saves/storageData.json");
                break;
            case "backpackEnemy":
                getEnemy(1);
                break;
        }

        GenerationBackpack();
    }

    int getEnemy(int lvl)
    {
        int r = 0;
        r = Random.Range(1, 4);//toDo 20
        backpackData.LoadData("Assets/Enemys/backpackDataEnemy" + lvl + "_" + r + ".json");
        return 0;
    }
    void Generation(GameObject generationObject, Vector3 place, Quaternion rotation)//уволен
    {
        var generationObjectItem = Instantiate(generationObject, place, rotation, gameObject.transform);
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
        componentItem.hits = new List<RaycastHit2D>();
        Physics2D.SyncTransforms();
        if (componentItem.prefabOriginalName.ToUpper().Contains("BAG"))
        {
            componentBag.RaycastEvent();
            componentBag.SetNestedObject();
            gameObject.transform.SetAsFirstSibling();
        }
        else
        {
            componentItem.hitsForBackpack = new List<RaycastHit2D>();
            componentItem.RaycastEvent();
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
            componentItem.MoveObjectOnEndDrag();
            //componentBag.MoveObjectOnEndDrag();
        }
    }
    public void GenerationBackpack()
    {
        if (backpackData.itemData.items.Count != 0)
        {
            foreach (var item in backpackData.itemData.items)
            {
                Generation(Resources.Load<GameObject>(item.name), item.position, item.rotation);
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
