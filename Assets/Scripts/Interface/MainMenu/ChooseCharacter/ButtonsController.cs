
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
    // Список ключей, которые нужно сохранить
    private List<string> keysToKeep = new List<string> { "ScreenMode", "MusicVolume", "SoundVolume", "WindowedResoultionWidth", "WindowedResoultionHeight", "EducationGlobalMap", "CaveEducation", "StartEducation", "EducationCaveIn" };

    // Метод для удаления всех ключей, кроме указанных
    public void DeleteAllExcept(List<string> keysToKeep)
    {
        // Шаг 1: Сохраняем значения ключей, которые нужно оставить
        Dictionary<string, object> savedValues = new Dictionary<string, object>();
        foreach (string key in keysToKeep)
        {
            if (PlayerPrefs.HasKey(key))
            {
                // Определяем тип данных и сохраняем значение
                if (PlayerPrefs.GetFloat(key, float.MinValue) != float.MinValue) // Проверяем, является ли значение float
                {
                    savedValues[key] = PlayerPrefs.GetFloat(key);
                }
                else if (PlayerPrefs.GetInt(key, int.MinValue) != int.MinValue) // Проверяем, является ли значение int
                {
                    savedValues[key] = PlayerPrefs.GetInt(key);
                }
                else if (PlayerPrefs.GetString(key) != null) // Проверяем, является ли значение строкой
                {
                    savedValues[key] = PlayerPrefs.GetString(key);
                }
            }
        }

        // Шаг 2: Удаляем все ключи
        PlayerPrefs.DeleteAll();

        // Шаг 3: Восстанавливаем сохраненные ключи
        foreach (var kvp in savedValues)
        {
            string key = kvp.Key;
            object value = kvp.Value;

            if (value is string)
            {
                PlayerPrefs.SetString(key, (string)value);
            }
            else if (value is int)
            {
                PlayerPrefs.SetInt(key, (int)value);
            }
            else if (value is float)
            {
                PlayerPrefs.SetFloat(key, (float)value);
            }
        }

        // Сохраняем изменения
        PlayerPrefs.Save();
    }

public void ToogleMainChoice()
   {
        mainCanvas.SetActive(!mainCanvas.activeSelf);
        chooseCharCanvas.SetActive(!chooseCharCanvas.activeSelf);
    }

    public void Choose()
    {
        PlayerPrefs.SetString("savePath", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games\\Backpack Seeker's"));
        PlayerPrefs.SetString("savePathTestBackpack", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games\\Backpack Seeker's backpacks"));
        if (!Directory.Exists(PlayerPrefs.GetString("savePath")))
        {
            Directory.CreateDirectory(PlayerPrefs.GetString("savePath"));
        }
        if (!Directory.Exists(PlayerPrefs.GetString("savePathTestBackpack")))
        {
            Directory.CreateDirectory(PlayerPrefs.GetString("savePathTestBackpack"));
        }
        DeleteAllData();
        StartBackPack();
        StartQeust();
        PlayerPrefs.SetInt("NeedSpawnEnemys", 1);
    }

    public void ChooseEarth()
    {
        DeleteAllExcept(keysToKeep);
        PlayerPrefs.SetString("characterClass", "Player_Earth");
        Choose();
        StartStats(85, 135, 1, 150, 100, 1, 11);

        mainCanvas.SetActive(false);
        chooseCharCanvas.SetActive(false);
        SceneLoader.Instance.LoadScene("GenerateMapInternumFortress1");
        //SceneManager.LoadScene("GenerateMapInternumFortress1");
    }

    public void ChooseIce()
    {
        DeleteAllExcept(keysToKeep);
        //PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString("characterClass", "Player_Ice");
        Choose();

        StartStats(50, 100, 1, 150, 100, 1, 9);

        mainCanvas.SetActive(false);
        chooseCharCanvas.SetActive(false);
        SceneLoader.Instance.LoadScene("GenerateMapInternumFortress1");
        //SceneManager.LoadScene("GenerateMapInternumFortress1");
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