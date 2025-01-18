using System;
using System.IO;
using System.Text;
using UnityEngine;

public class TalantTreeStats : MonoBehaviour
{
    private string talantTheeStatDataFilePath;
    public TalantTreeStatData talantTreeStatsData;



    private void Awake()
    {
        LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "talantTreeStatsData.json"));
    }

    public void SaveData()
    {
        talantTheeStatDataFilePath = Path.Combine(PlayerPrefs.GetString("savePath"), "talantTreeStatsData.json");

        //var saveData = "[";
        var saveData = JsonUtility.ToJson(talantTreeStatsData);
        //saveData += "]";

        if (File.Exists(talantTheeStatDataFilePath))
        {
            File.Delete(talantTheeStatDataFilePath);
        }


        using (FileStream fileStream = new FileStream(talantTheeStatDataFilePath, FileMode.Create, FileAccess.ReadWrite))
        {
            fileStream.Seek(0, SeekOrigin.End);
            byte[] buffer = Encoding.Default.GetBytes(saveData);
            fileStream.Write(buffer, 0, buffer.Length);
        }
    }
    public TalantTreeStatData LoadData(String filePath)
    {
        //characterStatsDataFilePath = "Assets/Saves/characterStatsData.json";
       // talantTreeStatsData = new CharacterStatsData(playerHP, playerExp, playerCoins, requiredExp, playerLvl, playerTime, activeTile, playerMaxHp);
        if (File.Exists(filePath))
        {
            //foreach (var line in File.ReadLines(mapDataFilePath))
            //{
            //    if (line != "[" && line != "]")
            //        mapData.tiles.Add(JsonUtility.FromJson<Tile>(line.Substring(0, line.Length - 1)));
            //}
            talantTreeStatsData = JsonUtility.FromJson<TalantTreeStatData>(File.ReadAllText(filePath));
        }
        //else
        //    Debug.LogError("There is no save data!");
        return talantTreeStatsData;
    }

}
