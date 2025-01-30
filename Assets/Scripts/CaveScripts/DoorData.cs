using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class DoorData : MonoBehaviour
{
    string doorDataFilePath;
    [HideInInspector] public DoorDataClass DoorDataClass;

    public void SaveData()
    {
        var saveData = JsonUtility.ToJson(DoorDataClass);
        //saveData += "]";

        if (File.Exists(doorDataFilePath))
        {
            File.Delete(doorDataFilePath);
        }


        using (FileStream fileStream = new FileStream(doorDataFilePath, FileMode.Create, FileAccess.ReadWrite))
        {
            fileStream.Seek(0, SeekOrigin.End);
            byte[] buffer = Encoding.Default.GetBytes(saveData);
            fileStream.Write(buffer, 0, buffer.Length);
        }
    }

    public void LoadData()
    {
        if (File.Exists(doorDataFilePath))
        {
            DoorDataClass = JsonUtility.FromJson<DoorDataClass>(File.ReadAllText(doorDataFilePath));
        }
        //else
        //    Debug.LogError("There is no save data!");
    }

    public void Awake()
    {
        doorDataFilePath = Path.Combine(PlayerPrefs.GetString("savePath"), "doorData.json");
    }
}