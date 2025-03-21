using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



[System.Serializable]
public class ItemsText
{
    public string name;
    public string description;
    public int countIcons;
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
    public List<ItemsText> items;
    public List<ItemsText> itemsEducation;
}

[System.Serializable]
public class LocalizationFile
{
    public LocalizedData en;
    public LocalizedData ru;
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
                case "eng":
                    text = _localizationData.en.items.Where(e => e.name == itemName).ToList()[0];
                    break;
                case "ru":
                    text = _localizationData.ru.items.Where(e => e.name == itemName).ToList()[0];
                    break;

            }
        }

        return text;
    }

}