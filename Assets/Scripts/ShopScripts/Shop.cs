using System.Collections.Generic;
using System.IO;
using System.Text;  
using UnityEngine;

public class Shop : MonoBehaviour
{
    public ListShopSaveData listShopSaveData;

    public void SaveData(string shopDataFilePath)
    {
        var listShopData = GameObject.FindObjectsByType<GenerateShopItems>(FindObjectsSortMode.None);
        listShopSaveData.listShopSaveData = new List<ShopSaveData>();
        foreach (var data in listShopData)
        {
            if(data.shopData.slotName != "")
                listShopSaveData.listShopSaveData.Add(new ShopSaveData(data.shopData.slotName, data.shopData.prefabName, data.GetComponent<Price>().lockForItem.activeSelf));
        }   
        //var saveData = "[";
        var saveData = JsonUtility.ToJson(listShopSaveData);
        //var saveData = JsonUtility.ToJson(new ShopSaveData(listShopData[0].shopData.slotName, listShopData[0].shopData.prefabName));
        //saveData += "]";

        if (File.Exists(shopDataFilePath))
        {
            File.Delete(shopDataFilePath);
        }


        using (FileStream fileStream = new FileStream(shopDataFilePath, FileMode.Create, FileAccess.ReadWrite))
        {
            fileStream.Seek(0, SeekOrigin.End);
            byte[] buffer = Encoding.Default.GetBytes(saveData);
            fileStream.Write(buffer, 0, buffer.Length);
        }
    }
    public void LoadData(string shopDataFilePath)
    {
        listShopSaveData = new ListShopSaveData();
        if (File.Exists(shopDataFilePath))
        {
            //foreach (var line in File.ReadLines(mapDataFilePath))
            //{
            //    if (line != "[" && line != "]")
            //        mapData.tiles.Add(JsonUtility.FromJson<Tile>(line.Substring(0, line.Length - 1)));
            //}
            listShopSaveData = JsonUtility.FromJson<ListShopSaveData>(File.ReadAllText(shopDataFilePath));
        }
        //else
        //    Debug.LogError("There is no save data!");
    }

    public void DeleteData(string shopDataFilePath)
    {
        if (File.Exists(shopDataFilePath))
        {
            File.Delete(shopDataFilePath);
        }
    }

    private void Awake()
    {
        if (!File.Exists("Assets/Saves/shopData.json"))
        {
            LoadData("Assets/Saves/shopData.json");
        }
    }
}
