using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using static generateMapScript;
using Newtonsoft.Json;
using System.IO.Pipes;
using Newtonsoft.Json.Linq;
using System;

public class Map : MonoBehaviour
{
    [HideInInspector] public List<Vector2> tilePositions = new List<Vector2>();
    [HideInInspector] public List<Vector2> pointInterestPoisitions = new List<Vector2>();
    [HideInInspector] public List<InterestPointStructure> pointInterestStructure = new List<InterestPointStructure>();

    [HideInInspector] public GameObject bossTile;
    [HideInInspector] public GameObject startTile;
    [HideInInspector] public Vector3 startTilePosition;
    [HideInInspector] public Vector2 startPlayerPosition;
    [HideInInspector] public List<Tile> tiles = new List<Tile>();

    [HideInInspector] public MapData mapData;//= ScriptableObject.CreateInstance<MapData>();
    [HideInInspector] public string mapDataFilePath;

    //[HideInInspector] public MapData mapData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SaveData()
    {
        mapDataFilePath = "Assets/Saves/mapData.json";
        mapData = new MapData(tiles);
        var saveData = "[\n";
        foreach (var tile in mapData.tiles)
        {
            saveData += JsonUtility.ToJson(tile) + ",\n";
        }
        saveData += "]";
        using (FileStream fileStream = new FileStream(mapDataFilePath, FileMode.Create, FileAccess.ReadWrite))
        {
            fileStream.Seek(0, SeekOrigin.End);
            byte[] buffer = Encoding.Default.GetBytes(saveData);
            fileStream.Write(buffer, 0, buffer.Length);
        }
    }
    public void LoadData()
    {
        mapData = new MapData(tiles);
        if (File.Exists(mapDataFilePath))
        {
            foreach (var line in File.ReadLines(mapDataFilePath))
            {
                if(line!="[" &&  line!="]")
                    mapData.tiles.Add(JsonUtility.FromJson<Tile>(line.Substring(0, line.Length-1)));
            }
        }
        else
            Debug.LogError("There is no save data!");
    }
}
