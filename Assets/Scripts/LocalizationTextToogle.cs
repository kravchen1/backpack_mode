using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;





public class LocalizationTextToogle : MonoBehaviour
{
    public string objectSwitch;
    private string settingLanguage = "en";
    private void Start()
    {
        updateText();
    }

    public void updateText()
    {
        settingLanguage = PlayerPrefs.GetString("LanguageSettings");
        string itemText = "";

        TMP_Text text;
        if (gameObject.GetComponent<TextMeshPro>() != null)
            text = gameObject.GetComponent<TextMeshPro>();
        else
            text = gameObject.GetComponent<TextMeshProUGUI>();

        switch (objectSwitch)
        {
            case "mv":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "Music_volume");
                text.text = itemText;
                break;
            case "sv":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "Sound_volume");
                text.text = itemText;
                break;
            case "resolution":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "Resolution_text");
                text.text = itemText;
                break;
            case "wm":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "Window_mode");
                text.text = itemText;
                break;
            case "lang":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "Language_text");
                text.text = itemText;
                break;
            case "sq":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "StartEquip_text");
                text.text = itemText;
                break;
            case "gd_earth":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "GameDifficulty_text_earth");
                text.text = itemText;
                break;
            case "gd_ice":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "GameDifficulty_text_ice");
                text.text = itemText;
                break;
            case "ltd_earth":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "Description_earth");
                text.text = itemText;
                break;
            case "ltd_ice":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "Description_ice");
                text.text = itemText;
                break;
            case "quests":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "quests_text");
                text.text = itemText;
                break;
            case "Exit_button":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "Exit_button");
                text.text = itemText;
                break;
            case "EndOfBattleOk":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "EndOfBattleOk");
                text.text = itemText;
                break;
            case "BattleLog_Button":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "BattleLog_Button");
                text.text = itemText;
                break;






        }
        
    }




}