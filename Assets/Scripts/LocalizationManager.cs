using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



[System.Serializable]
public class IconsEducation
{
    public string name;
    public string description;
}
[System.Serializable]
public class ItemsText
{
    public string name;
    public string description;
    public string type;
    public string rarity;
}
[System.Serializable]
public class uiTexts
{
    public string name;
    public string text;
}

[System.Serializable]
public class dialoguesTexts
{
    public string name;
    public string text;
}

[System.Serializable]
public class LocalizedData
{
    public List<uiTexts> ui;
    public List<dialoguesTexts> dialogs;
    public List<IconsEducation> iconsEducation;
    public List<ItemsText> items;
    public string weaponStat;
}

[System.Serializable]
public class LocalizationFile
{
    public LocalizedData en;
    public LocalizedData ru;
    public LocalizedData zhCN;
}


public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    private LocalizationFile _localizationData;

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

        LoadLocalizedText("LozalizationLanguage"); // Загружаем язык по умолчанию
    }

    public void LoadLocalizedText(string languageCode)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Localization/" + languageCode);
        if (jsonFile == null)
        {
            Debug.LogError("Localization file not found!");
            return;
        }

        _localizationData = JsonUtility.FromJson<LocalizationFile>(jsonFile.text);
    }

    public ItemsText GetTextItem(string lang, string itemName)
    {
        ItemsText text = null;
        if (_localizationData != null)
        {
            switch (lang)
            {
                case "en":
                    text = _localizationData.en.items.Where(e => e.name == itemName).ToList()[0];
                    break;
                case "ru":
                    text = _localizationData.ru.items.Where(e => e.name == itemName).ToList()[0];
                    break;
                default:
                    text = _localizationData.en.items.Where(e => e.name == itemName).ToList()[0];
                    break;
            }
        }

        return text;
    }

    public IconsEducation GetTextIconDescriptionEducation(string lang, string itemName)
    {
        IconsEducation text = null;
        if (_localizationData != null)
        {
            switch (lang)
            {
                case "en":
                    text = _localizationData.en.iconsEducation.Where(e => e.name == itemName).ToList()[0];
                    break;
                case "ru":
                    text = _localizationData.ru.iconsEducation.Where(e => e.name == itemName).ToList()[0];
                    break;
                default:
                    text = _localizationData.en.iconsEducation.Where(e => e.name == itemName).ToList()[0];
                    break;
            }
        }

        return text;
    }

    public string GetTextWeaponStat(string lang)
    {
        string text = "";
        if (_localizationData != null)
        {
            switch (lang)
            {
                case "en":
                    text = _localizationData.en.weaponStat;
                    break;
                case "ru":
                    text = _localizationData.ru.weaponStat;
                    break;
                case "zh-CN":
                    text = _localizationData.zhCN.weaponStat;
                    break;
                default:
                    text = _localizationData.en.weaponStat;
                    break;
            }
        }

        return text;
    }

}