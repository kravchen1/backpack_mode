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
            case "Settings_button":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "Settings_button");
                text.text = itemText;
                break;
            case "MainMenu_Button":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "MainMenu_Button");
                text.text = itemText;
                break;
            case "Surrend_Button":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "Surrend_Button");
                text.text = itemText;
                break;
            case "UnitsWeight":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "UnitsWeight");
                text.text = itemText;
                break;




            case "BagsText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "BagsText");
                text.text = itemText;
                break;
            case "FoodText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "FoodText");
                text.text = itemText;
                break;
            case "WeaponsText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "WeaponsText");
                text.text = itemText;
                break;
            case "ClothingText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "ClothingText");
                text.text = itemText;
                break;
            case "CrystalsText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "CrystalsText");
                text.text = itemText;
                break;
            case "KeyStonesText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "KeyStonesText");
                text.text = itemText;
                break;
            case "JunkText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "JunkText");
                text.text = itemText;
                break;
            case "MushroomsText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "MushroomsText");
                text.text = itemText;
                break;
            case "StuffText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "StuffText");
                text.text = itemText;
                break;
            case "DragonText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "DragonText");
                text.text = itemText;
                break;
            case "PetsText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "PetsText");
                text.text = itemText;
                break;
            case "WitchCraftText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "WitchCraftText");
                text.text = itemText;
                break;
            case "ManaText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "ManaText");
                text.text = itemText;
                break;
            case "FireText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "FireText");
                text.text = itemText;
                break;
            case "VampireText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "VampireText");
                text.text = itemText;
                break;



            case "WinFightText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "WinFightText");
                text.text = itemText;
                break;
            case "LoseFightText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "LoseFightText");
                text.text = itemText;
                break;
            case "PauseText":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "PauseText");
                text.text = itemText;
                break;
            case "ButtonStuck":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "ButtonStuck");
                text.text = itemText;
                break;



            case "CommonFilter":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "CommonFilter");
                text.text = itemText;
                break;
            case "RareFilter":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "RareFilter");
                text.text = itemText;
                break;
            case "EpicFilter":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "EpicFilter");
                text.text = itemText;
                break;
            case "LegendaryFilter":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "LegendaryFilter");
                text.text = itemText;
                break;
            case "StoneFilter":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "StoneFilter");
                text.text = itemText;
                break;
            case "BagsFilter":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "BagsFilter");
                text.text = itemText;
                break;
            case "EatFilter":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "EatFilter");
                text.text = itemText;
                break;
            case "AllFilter":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "AllFilter");
                text.text = itemText;
                break;
            case "ButtonSell":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "ButtonSell");
                text.text = itemText;
                break;
            case "ButtonDestroy":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "ButtonDestroy");
                text.text = itemText;
                break;
            case "PressEsc":
                itemText = LocalizationManager.Instance.GetTextUI(settingLanguage, "PressEsc");
                text.text = itemText;
                break;

        }
        
    }




}