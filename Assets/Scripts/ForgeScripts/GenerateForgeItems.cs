using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GenerateForgeItems: MonoBehaviour
{
    public List<GameObject> forgeItems;
    private GameObject[] prefabs;
    public List<GameObject> frames;
    [HideInInspector] public List<ForgeData> listForgeData;

    private void Awake()
    {
        if (prefabs == null)
        {
            prefabs = PrefabsManager._cachedPrefabs;//Resources.LoadAll<GameObject>("");
        }
        forgeItems.AddRange(prefabs.Where(e => e.tag == "ForgeItem").ToList());

    }

    void Generation(GameObject generationObject, GameObject place, int index)
    {
        var generationObjectForge = Instantiate(generationObject, new Vector3(0,0,0), Quaternion.identity, place.GetComponent<RectTransform>());
        generationObjectForge.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        listForgeData.Add(new ForgeData(index, generationObject.name));
    }

    void GenerationFromFile()
    {
        foreach (var forgeData in listForgeData)
        {
            foreach (var forgeItem in forgeItems)
            {
                if (forgeItem.name == forgeData.prefabName)
                {
                    var generationObjectForge = Instantiate(forgeItem, new Vector3(0, 0, 0), Quaternion.identity, frames[forgeData.indexFrame].GetComponent<RectTransform>());
                    generationObjectForge.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
                    break;
                }
            }
            
        }
    }




    public void GenerateRandomItem()
    {
        int r;
        for (int i = 0; i < forgeItems.Count; i++)
        {
            r = Random.Range(0, forgeItems.Count);
            Generation(forgeItems[r], frames[i], i);
        }
        //if (r < generateItems.Count)
        //{
        //    Generation(generateItems[r], placeForItemCollider.bounds.center);
        //}
        //else
        //{
        //    Generation(bag4x4, placeForItemCollider.bounds.center);
        //}
    }



    void Start()
    {
        GameObject.FindGameObjectWithTag("Forge").GetComponent<Forge>().LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "forgeData.json"));
        if (listForgeData != null && listForgeData.Count != 0)
        {
            GenerationFromFile();
        }
        else
            GenerateRandomItem();
    }

}
