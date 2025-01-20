using System.IO;
using System.Text;
using UnityEngine;

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
        if (!File.Exists(Path.Combine(PlayerPrefs.GetString("savePath"), "forgeData.json")))
        {
            LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "forgeData.json"));
        }
    }
}

