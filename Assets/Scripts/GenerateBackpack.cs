using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class GenerateBackpack : MonoBehaviour
{
    private BackpackData backpackData;
    void Start()
    {
        backpackData = GetComponent<BackpackData>();
        backpackData.LoadData();
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
