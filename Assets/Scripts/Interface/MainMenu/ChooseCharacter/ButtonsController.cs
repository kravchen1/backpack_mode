using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
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

        SceneManager.LoadScene("GenerateMapInternumFortress1");
    }

    public void ChooseEarth()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString("characterClass", "Player_Earth");
        Choose();
    }

    public void ChooseIce()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString("characterClass", "Player_Ice");
        Choose();
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
            data = new Data("bagStartFire", new Vector3(-260.41583251953127f + (x * rX), -164.7316436767578f + (y * rY), 0f));
        }
        else if (character.Contains("Earth"))
        {
            data = new Data("bagStartEarth", new Vector3(-260.41583251953127f + (x * rX), -164.7316436767578f + (y * rY), 0f));
        }
        else if (character.Contains("Ice"))
        {
            data = new Data("bagStartIce", new Vector3(-260.41583251953127f + (x * rX), -164.7316436767578f + (y * rY), 0f));
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
}