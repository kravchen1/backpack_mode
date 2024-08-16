using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class BackpackData : MonoBehaviour
{
    //public Tiles tiles = new Tiles();
    public List<ItemData> items;
    public string backpackDataFilePath;
    //public Tile tile;

    //public List<string> tileName;
    //public List<Vector2> tilePosition;

    public BackpackData(List<ItemData> items)
    {
        this.items = items;
    }

    public void SaveData()
    {
        backpackDataFilePath = "Assets/Saves/backpackData.json";
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            var childGO = gameObject.transform.GetChild(i).gameObject;
            if (childGO.layer != 7)
            {
                items.Add(new ItemData(childGO.name, childGO.transform.localPosition));
            }
        }
        //var backpackData = new BackpackData();

        //var saveData = "[";
        var saveData = JsonUtility.ToJson(backpackDataFilePath);
        //saveData += "]";

        if (File.Exists(backpackDataFilePath))
        {
            File.Delete(backpackDataFilePath);
        }


        using (FileStream fileStream = new FileStream(backpackDataFilePath, FileMode.Create, FileAccess.ReadWrite))
        {
            fileStream.Seek(0, SeekOrigin.End);
            byte[] buffer = Encoding.Default.GetBytes(saveData);
            fileStream.Write(buffer, 0, buffer.Length);
        }
    }
    public void LoadData()
    {
        //    mapData = new MapData(tiles, new Vector2(0, 0));
        //    if (File.Exists(mapDataFilePath))
        //    {
        //        //foreach (var line in File.ReadLines(mapDataFilePath))
        //        //{
        //        //    if (line != "[" && line != "]")
        //        //        mapData.tiles.Add(JsonUtility.FromJson<Tile>(line.Substring(0, line.Length - 1)));
        //        //}
        //        mapData = JsonUtility.FromJson<MapData>(File.ReadAllText(mapDataFilePath));
        //    }
        //    else
        //        Debug.LogError("There is no save data!");
        //}
    }
}
