//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
////using UnityEditor.Overlays;
//using UnityEngine;

//public class BackpackData : MonoBehaviour
//{
//    private string backpackDataFilePath;
//    public ItemData itemData;
//    //public Tile tile;

//    //public List<string> tileName;
//    //public List<Vector2> tilePosition;

//    public void SaveData(string filePath)
//    {
//        if (itemData != null)
//        {
//            backpackDataFilePath = filePath;
//            var saveData = JsonUtility.ToJson(itemData);
//            //saveData += "]";

//            if (File.Exists(backpackDataFilePath))
//            {
//                File.Delete(backpackDataFilePath);
//            }


//            using (FileStream fileStream = new FileStream(backpackDataFilePath, FileMode.Create, FileAccess.ReadWrite))
//            {
//                fileStream.Seek(0, SeekOrigin.End);
//                byte[] buffer = Encoding.Default.GetBytes(saveData);
//                fileStream.Write(buffer, 0, buffer.Length);
//            }
//        }
//    }
//    public void SaveData()
//    {

//        List<Data> data = new List<Data>();
//        for (int i = 0; i < gameObject.transform.childCount; i++)
//        {
//            var childGO = gameObject.transform.GetChild(i).gameObject;
//            if (childGO.layer != 7 && childGO.layer != 13)
//            {
//                data.Add(new Data(childGO.GetComponent<Item>().prefabOriginalName, childGO.transform.localPosition, childGO.transform.rotation));
//            }
//        }

//        itemData.items = data;
//        var saveData = JsonUtility.ToJson(itemData);

//        if (File.Exists(backpackDataFilePath))
//        {
//            File.Delete(backpackDataFilePath);
//        }

//        using (FileStream fileStream = new FileStream(backpackDataFilePath, FileMode.Create, FileAccess.ReadWrite))
//        {
//            fileStream.Seek(0, SeekOrigin.End);
//            byte[] buffer = Encoding.Default.GetBytes(saveData);
//            fileStream.Write(buffer, 0, buffer.Length);
//        }
//    }

//    public void SaveNewData(string filePath)
//    {

//        List<Data> data = new List<Data>();
//        for (int i = 0; i < gameObject.transform.childCount; i++)
//        {
//            var childGO = gameObject.transform.GetChild(i).gameObject;
//            if (childGO.layer != 7 && childGO.layer != 13)
//            {
//                data.Add(new Data(childGO.GetComponent<Item>().prefabOriginalName, childGO.transform.localPosition, childGO.transform.rotation));
//            }
//        }

//        itemData.items = data;
//        var saveData = JsonUtility.ToJson(itemData);

//        if (File.Exists(filePath))
//        {
//            File.Delete(filePath);
//        }

//        using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
//        {
//            fileStream.Seek(0, SeekOrigin.End);
//            byte[] buffer = Encoding.Default.GetBytes(saveData);
//            fileStream.Write(buffer, 0, buffer.Length);
//        }
//    }
//    public void LoadData(String fileName)
//    {
//        if (File.Exists(fileName))
//        {
//            //foreach (var line in File.ReadLines(mapDataFilePath))
//            //{
//            //    if (line != "[" && line != "]")
//            //        mapData.tiles.Add(JsonUtility.FromJson<Tile>(line.Substring(0, line.Length - 1)));
//            //}
//            itemData = JsonUtility.FromJson<ItemData>(File.ReadAllText(fileName));
//        }
//        //else
//        //    Debug.LogError("There is no save data!");
//    }

//    public void LoadDataEnemy(String jsonData)
//    {
//        itemData = JsonUtility.FromJson<ItemData>(jsonData);
//    }
//    private void Awake()
//    {
//        switch (gameObject.name)
//        {
//            case "backpack":
//                backpackDataFilePath = Path.Combine(PlayerPrefs.GetString("savePath"), "backpackData.json");
//                break;
//            case "Storage":
//                backpackDataFilePath = Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json");
//                break;
//        }

//    }
//}