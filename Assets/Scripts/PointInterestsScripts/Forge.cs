using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Forge : MonoBehaviour
{
    private BackpackData storageData;
    private List<GameObject> forgeItems;
    private GameObject[] forgeFrames;
    private GameObject[] prefabs;

    private bool move;
    private float timer_cooldown = 0.2f;
    private float timer = 0f;
    private bool timer_locked_out = false;
    private int index = 0;
    public bool foolItemChestList = false;

    private int winnerIndex = -1;

    void Awake()
    {
        storageData = new BackpackData();
        forgeItems = new List<GameObject>();
        forgeFrames = GameObject.FindGameObjectsWithTag("ForgeFrame");
    }


    public void InitializatedForgeItems()
    {
        int i = 0;
        foreach (var forgeFrame in forgeFrames)
        {
            if (forgeFrame.transform.childCount == 0)
            {
                var generationObjectForge = Instantiate(forgeItems[i], new Vector3(0, 0, 0), Quaternion.identity, forgeFrame.GetComponent<RectTransform>());
                generationObjectForge.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
                i++;
            }
        }
    }

    public void LoadForgeItems(string tagName)
    {
        if (prefabs == null)
        {
            prefabs = Resources.LoadAll<GameObject>("");
        }
        forgeItems.AddRange(prefabs.Where(e => e.tag == tagName).ToList());
    }

    public void AddNewItemInStorage()
    {
        storageData.LoadData("Assets/Saves/storageData.json");
        storageData.itemData.items.Add(new Data(forgeItems[winnerIndex].name, new Vector2(0,0)));
        storageData.SaveDataFromChest("Assets/Saves/storageData.json");
    }

    private void Update()
    {    
    }
}

