using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class BattlesSpawnerData : MonoBehaviour
{
    [HideInInspector] public BattlesSpawnerDataClass battlesSpawnerDataClass;

    public void SaveData(string battleSpawnerDataFilePath)
    {
        var saveData = JsonUtility.ToJson(battlesSpawnerDataClass);
        //saveData += "]";

        if (File.Exists(battleSpawnerDataFilePath))
        {
            File.Delete(battleSpawnerDataFilePath);
        }


        using (FileStream fileStream = new FileStream(battleSpawnerDataFilePath, FileMode.Create, FileAccess.ReadWrite))
        {
            fileStream.Seek(0, SeekOrigin.End);
            byte[] buffer = Encoding.Default.GetBytes(saveData);
            fileStream.Write(buffer, 0, buffer.Length);
        }
    }

    public void LoadData(string battleSpawnerDataFilePath)
    {
        if (File.Exists(battleSpawnerDataFilePath))
        {
            battlesSpawnerDataClass = JsonUtility.FromJson<BattlesSpawnerDataClass>(File.ReadAllText(battleSpawnerDataFilePath));
        }
        //else
        //    Debug.LogError("There is no save data!");
    }

}