using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEditor.Progress;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Forge : MonoBehaviour
{
    public ListForgeData listForgeData;
    public GameObject forgeItems;
    public void SaveData(string forgeDataFilePath)
    {
        //var listForgeData = GameObject.FindforgeItems.GetComponent<GenerateForgeItems>().listForgeData;
        listForgeData = new ListForgeData();
        listForgeData.listForgeData = GameObject.FindGameObjectWithTag("ForgeItems").GetComponent<GenerateForgeItems>().listForgeData;
        var saveData = JsonUtility.ToJson(listForgeData);


        if (File.Exists(forgeDataFilePath))
        {
            File.Delete(forgeDataFilePath);
        }

        using (FileStream fileStream = new FileStream(forgeDataFilePath, FileMode.Create, FileAccess.ReadWrite))
        {
            fileStream.Seek(0, SeekOrigin.End);
            byte[] buffer = Encoding.Default.GetBytes(saveData);
            fileStream.Write(buffer, 0, buffer.Length);
        }
    }
    public void LoadData(string forgeDataFilePath)
    {
        listForgeData = new ListForgeData();
        if (File.Exists(forgeDataFilePath))
        {
            var r = JsonUtility.FromJson<ListForgeData>(File.ReadAllText(forgeDataFilePath));
            GameObject.FindGameObjectWithTag("ForgeItems").GetComponent<GenerateForgeItems>().listForgeData = JsonUtility.FromJson<ListForgeData>(File.ReadAllText(forgeDataFilePath)).listForgeData;
        }
        else
            Debug.LogError("There is no save data!");
    }

    public void DeleteData(string forgeDataFilePath)
    {
        if (File.Exists(forgeDataFilePath))
        {
            File.Delete(forgeDataFilePath);
        }
    }

    private void Awake()
    {
        if (!File.Exists("Assets/Saves/forgeData.json"))
        {
            LoadData("Assets/Saves/forgeData.json");
        }
    }
}

