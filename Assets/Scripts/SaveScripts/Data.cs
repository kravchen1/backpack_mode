using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor.Overlays;

public class Data : MonoBehaviour
{
    public string filePath;

    public MapData mapData;

    public Data(string filePath, MapData mapData)
    {
        this.filePath = filePath;
        this.mapData = mapData;
    }


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
        using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            fileStream.Seek(0, SeekOrigin.End);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fileStream, mapData);
            Debug.Log("Game data saved!");
        }
    }
    public void LoadData()
    {
        if (File.Exists(filePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream file =File.Open(filePath, FileMode.Open))
            {
                MapData data = (MapData)bf.Deserialize(file);
            }
                
        }
        else
            Debug.LogError("There is no save data!");
    }  
}
