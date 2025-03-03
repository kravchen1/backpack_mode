using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsController : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject chooseCharCanvas;

    public void ToogleMainChoice()
   {
        mainCanvas.SetActive(!mainCanvas.activeSelf);
        chooseCharCanvas.SetActive(!chooseCharCanvas.activeSelf);
    }

    public void Choose()
    {
        PlayerPrefs.SetString("savePath", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games\\Backpack Seeker's"));
        if (!Directory.Exists(PlayerPrefs.GetString("savePath")))
        {
            Directory.CreateDirectory(PlayerPrefs.GetString("savePath"));
        }
        DeleteAllData();
        StartBackPack();
        StartQeust();
        PlayerPrefs.SetInt("NeedSpawnEnemys", 1);
    }

    public void ChooseEarth()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString("characterClass", "Player_Earth");
        Choose();
        StartStats(85, 135, 1, 200, 100, 1, 11);
        SceneManager.LoadScene("GenerateMapInternumFortress1");
    }

    public void ChooseIce()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString("characterClass", "Player_Ice");
        Choose();

        StartStats(50, 100, 1, 200, 100, 1, 9);
        SceneManager.LoadScene("GenerateMapInternumFortress1");
    }



    void DeleteAllData()
    {
        var saveDirectory = PlayerPrefs.GetString("savePath");
        foreach (var file in Directory.GetFiles(saveDirectory))
        {
            File.Delete(file);
        }
    }
    private void StartBackPack()
    {
        BackpackData backpackData = new BackpackData();
        backpackData.itemData = new ItemData();
        Data data = null;
        string character = PlayerPrefs.GetString("characterClass");

        int rX = UnityEngine.Random.Range(0, 5);
        int rY = UnityEngine.Random.Range(0, 5);
        float x = 84.49f;//шаг
        float y = 80.34f;//шаг

        if (character.Contains("Fire"))
        {
            data = new Data("bagStartFire", new Vector3(-260.41583251953127f + (x * rX), -164.7316436767578f + (y * rY), -1f));
        }
        else if (character.Contains("Earth"))
        {
            data = new Data("bagStartEarth", new Vector3(-260.41583251953127f + (x * rX), -164.7316436767578f + (y * rY), -1f));
        }
        else if (character.Contains("Ice"))
        {
            data = new Data("bagStartIce", new Vector3(-260.41583251953127f + (x * rX), -164.7316436767578f + (y * rY), -1f));
        }
        backpackData.itemData.items.Add(data);
        backpackData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "backpackData.json"));
    }

    private void StartQeust()
    {
        QuestData questData = new QuestData();
        questData.questData = new QDataList();

        Quest quest = new Quest("Еhe beginning of time", "talk to the king", -1, 1);

        questData.questData.quests.Add(quest);
        questData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json"));
    }

    private void StartStats(int playerHP, int playerMaxHp, int playerExp, int playerCoins, int requiredExp, int playerLvl, float playerMaxStamina)
    {
        string characterStatsDataFilePath;
        CharacterStatsData characterStatsData;

        characterStatsDataFilePath = Path.Combine(PlayerPrefs.GetString("savePath"), "characterStatsData.json");
        characterStatsData = new CharacterStatsData(playerHP, playerMaxHp, playerExp, playerCoins, requiredExp, playerLvl, playerMaxStamina);

        var saveData = JsonUtility.ToJson(characterStatsData);

        if (File.Exists(characterStatsDataFilePath))
        {
            File.Delete(characterStatsDataFilePath);
        }


        using (FileStream fileStream = new FileStream(characterStatsDataFilePath, FileMode.Create, FileAccess.ReadWrite))
        {
            fileStream.Seek(0, SeekOrigin.End);
            byte[] buffer = Encoding.Default.GetBytes(saveData);
            fileStream.Write(buffer, 0, buffer.Length);
        }
    }
}