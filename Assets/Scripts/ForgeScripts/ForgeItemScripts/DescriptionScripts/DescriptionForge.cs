using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionForge : MonoBehaviour
{
    public int needCountItem1;
    public string needItemName1;
    public string craftedItem;
    public GameObject textMesh1, textMesh1_need;

    
    public int haveCountItem1 = 0;
    private GameObject backPackAndStorage;
    private int backPackCountItem1, storageCountItem1;
    private BackPackAndStorageData backPackAndStorageData;


    void SearchHaveCountItem()
    {
        backPackAndStorage = GameObject.FindGameObjectWithTag("BackPackAndStorageData");
        // var z = idiNaHui.GetComponent<BackPackAndStorageData>().backPackData.itemData.items.Where(e => e.name == needItemName1).Count();
        // var z2 = idiNaHui.GetComponent<BackPackAndStorageData>().storageData.itemData.items.Where(e => e.name == needItemName1).Count();
        backPackAndStorageData = backPackAndStorage.GetComponent<BackPackAndStorageData>();

        backPackCountItem1 = backPackAndStorageData.backPackData.itemData.items.Where(e => e.name == needItemName1).Count();
        storageCountItem1 = backPackAndStorageData.storageData.itemData.items.Where(e => e.name == needItemName1).Count();

        haveCountItem1 = backPackCountItem1 + storageCountItem1;


        textMesh1.GetComponent<TextMeshPro>().text = haveCountItem1.ToString();
        textMesh1_need.GetComponent<TextMeshPro>().text = "/" + needCountItem1.ToString();
    }

    public void CreateAndDeleteItemsFromBackPackAndStorage()
    {
        int countDeletedItems = 0;
        //foreach(var z in backPackAndStorageData.storageData.itemData.Where(e => e.name == needItemName1))
        var listStorage = backPackAndStorageData.storageData.itemData.items.Where(e => e.name == needItemName1).ToList();
        var listBackPack = backPackAndStorageData.backPackData.itemData.items.Where(e => e.name == needItemName1).ToList();
        while (countDeletedItems < needCountItem1)
        {
            for (int i = 0; i < listStorage.Count() && countDeletedItems < needCountItem1; i++)
            {
                backPackAndStorageData.storageData.itemData.items.Remove(listStorage[i]);
                countDeletedItems++;
            }
            for (int i = 0; i < listBackPack.Count() && countDeletedItems < needCountItem1; i++)
            {
                backPackAndStorageData.backPackData.itemData.items.Remove(listBackPack[i]);
                countDeletedItems++;
            }
        }

        backPackAndStorageData.storageData.itemData.items.Add(new Data(craftedItem, new Vector2(0,0)));
        backPackAndStorageData.storageData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json"));
        backPackAndStorageData.backPackData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "backpackData.json"));

    }

    private void Awake()
    {
        SearchHaveCountItem();
    }

    void Update()
    {
        
    }
}
