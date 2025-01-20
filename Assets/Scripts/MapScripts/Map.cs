using System.Text;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static generateMapScript;

public class Map : MonoBehaviour
{
    [HideInInspector] public List<Vector2> tilePositions = new List<Vector2>();
    [HideInInspector] public List<Vector2> pointInterestPoisitions = new List<Vector2>();
    [HideInInspector] public List<InterestPointStructure> pointInterestStructure = new List<InterestPointStructure>();

    [HideInInspector] public GameObject endPointTile;
    [HideInInspector] public GameObject startTile;
    [HideInInspector] public GameObject player;
    [HideInInspector] public GameObject portalTile;
    [HideInInspector] public Vector3 startTilePosition;
    [HideInInspector] public Vector2 startPlayerPosition;
    [HideInInspector] public int mapLevel;
    [HideInInspector] public List<Tile> tiles;// = new List<Tile>();

    [HideInInspector] public MapData mapData;//= ScriptableObject.CreateInstance<MapData>();
    //[HideInInspector] public string mapDataFilePath;

    //[HideInInspector] public MapData mapData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SaveData(string mapDataFilePath, MapData mapData2 = null)
    {
        if(mapData2 == null)
            mapData = new MapData(tiles, startPlayerPosition);
        else 
            mapData = mapData2;
        //var saveData = "[";
        var saveData = JsonUtility.ToJson(mapData);
        //saveData += "]";

        if (File.Exists(mapDataFilePath))
        {
            File.Delete(mapDataFilePath);
        }


        using (FileStream fileStream = new FileStream(mapDataFilePath, FileMode.Create, FileAccess.ReadWrite))
        {
            fileStream.Seek(0, SeekOrigin.End);
            byte[] buffer = Encoding.Default.GetBytes(saveData);
            fileStream.Write(buffer, 0, buffer.Length);
        }
    }
    public void LoadData(string mapDataFilePath)
    {
        mapData = new MapData(tiles, new Vector2(0, 0));
        if (File.Exists(mapDataFilePath))
        {
            //foreach (var line in File.ReadLines(mapDataFilePath))
            //{
            //    if (line != "[" && line != "]")
            //        mapData.tiles.Add(JsonUtility.FromJson<Tile>(line.Substring(0, line.Length - 1)));
            //}
            mapData = JsonUtility.FromJson<MapData>(File.ReadAllText(mapDataFilePath));
        }
        else
            Debug.LogError("There is no save data!");
    }
    public void DeleteData(string mapDataFilePath)
    {
        if (File.Exists(mapDataFilePath))
        {
            File.Delete(mapDataFilePath);
        }
    }
    public void ChangeMapRedTimeZone()
    {
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemy.GetComponent<SpriteRenderer>().enabled = false;
        }
        foreach (var enemy in GameObject.FindGameObjectsWithTag("EnemyDemon"))
        {
            enemy.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
