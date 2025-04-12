using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




[System.Serializable]
public class QuestsData
{
    public int id;
    public int progress;
    public string name;
    public string text;
}

[System.Serializable]
public class Quests
{
    public List<QuestsData> quests;
}

[System.Serializable]
public class LocalizationFileQuests
{
    public Quests ru;
    public Quests en;
    public Quests zh;
    public Quests zh_tw;
}


public class QuestManagerJSON : MonoBehaviour
{
    public static QuestManagerJSON Instance;

    private LocalizationFileQuests _localizationData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadLocalizedText("Quests"); // Загружаем язык по умолчанию
    }

    public void Initialize()
    {
        LoadLocalizedText("Quests");
    }

    public void LoadLocalizedText(string languageCode)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("JSONs/" + languageCode);
        if (jsonFile == null)
        {
            Debug.LogError("Localization quests file not found!");
            return;
        }

        _localizationData = JsonUtility.FromJson<LocalizationFileQuests>(jsonFile.text);
    }

    public string GetNameQuest(string lang, int id)
    {
        string text = "";
        if (_localizationData != null)
        {
            switch (lang)
            {
                case "en":
                    text = _localizationData.en.quests.Where(e => e.id == id).ToList()[0].name;
                    break;
                case "ru":
                    text = _localizationData.ru.quests.Where(e => e.id == id).ToList()[0].name;
                    break;
                case "zh":
                    text = _localizationData.zh.quests.Where(e => e.id == id).ToList()[0].name;
                    break;
                case "zh_tw":
                    text = _localizationData.zh_tw.quests.Where(e => e.id == id).ToList()[0].name;
                    break;
                default:
                    text = _localizationData.en.quests.Where(e => e.id == id).ToList()[0].name;
                    break;
            }
        }
        return text;
    }

    public string GetTextQuest(string lang, int id)
    {
        string text = "";
        if (_localizationData != null)
        {
            switch (lang)
            {
                case "en":
                    text = _localizationData.en.quests.Where(e => e.id == id).ToList()[0].text;
                    break;
                case "ru":
                    text = _localizationData.ru.quests.Where(e => e.id == id).ToList()[0].text;
                    break;
                case "zh":
                    text = _localizationData.zh.quests.Where(e => e.id == id).ToList()[0].text;
                    break;
                case "zh_tw":
                    text = _localizationData.zh_tw.quests.Where(e => e.id == id).ToList()[0].text;
                    break;
                default:
                    text = _localizationData.en.quests.Where(e => e.id == id).ToList()[0].text;
                    break;
            }
        }
        return text;
    }

    public int GetProgressQuest(string lang, int id)
    {
        int result = -1;
        if (_localizationData != null)
        {
            switch (lang)
            {
                case "en":
                    result = _localizationData.en.quests.Where(e => e.id == id).ToList()[0].progress;
                    break;
                case "ru":
                    result = _localizationData.ru.quests.Where(e => e.id == id).ToList()[0].progress;
                    break;
                case "zh":
                    result = _localizationData.zh.quests.Where(e => e.id == id).ToList()[0].progress;
                    break;
                case "zh_tw":
                    result = _localizationData.zh_tw.quests.Where(e => e.id == id).ToList()[0].progress;
                    break;
                default:
                    result = _localizationData.en.quests.Where(e => e.id == id).ToList()[0].progress;
                    break;
            }
        }
        return result;
    }

}