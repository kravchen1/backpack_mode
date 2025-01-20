using System;
using System.IO;
using System.Text;
using UnityEngine;

public class QuestData : MonoBehaviour
{
    public string questDataFilePath;
    public QDataList questData;

    public void SaveData(string filePath)
    {
        if (questData != null)
        {
            questDataFilePath = filePath;
            var saveData = JsonUtility.ToJson(questData);

            if (File.Exists(questDataFilePath))
            {
                File.Delete(questDataFilePath);
            }


            using (FileStream fileStream = new FileStream(questDataFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                fileStream.Seek(0, SeekOrigin.End);
                byte[] buffer = Encoding.Default.GetBytes(saveData);
                fileStream.Write(buffer, 0, buffer.Length);
            }
        }
    }
    public void LoadData(String fileName)
    {
        if (File.Exists(fileName))
        {
            questData = JsonUtility.FromJson<QDataList>(File.ReadAllText(fileName));
        }
    }
    private void Awake()
    {
        questDataFilePath = Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json");
    }
}