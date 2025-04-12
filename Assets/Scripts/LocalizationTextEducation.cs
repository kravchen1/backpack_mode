using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;





public class LocalizationTextEducation : MonoBehaviour
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
            
            case "BuffsDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BuffsDescription");
                text.text = itemText;
                break;
            case "ManaDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "ManaDescription");
                text.text = itemText;
                break;
            case "ChanceCritDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "ChanceCritDescription");
                text.text = itemText;
                break;
            case "BaseCritDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BaseCritDescription");
                text.text = itemText;
                break;
            case "BurnDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BurnDescription");
                text.text = itemText;
                break;
            case "RegenerationDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "RegenerationDescription");
                text.text = itemText;
                break;
            case "gd_ice":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "GameDifficulty_text_ice");
                text.text = itemText;
                break;
            case "EvasionDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "EvasionDescription");
                text.text = itemText;
                break;
            case "PowerDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "PowerDescription");
                text.text = itemText;
                break;
            case "VampireDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "VampireDescription");
                text.text = itemText;
                break;
            case "ResistanceDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "ResistanceDescription");
                text.text = itemText;
                break;
            case "ArmorDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "ArmorDescription");
                text.text = itemText;
                break;
            case "DebuffsDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "DebuffsDescription");
                text.text = itemText;
                break;
            case "BleedDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BleedDescription");
                text.text = itemText;
                break;
            case "FrostDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "FrostDescription");
                text.text = itemText;
                break;
            case "PoisonDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "PoisonDescription");
                text.text = itemText;
                break;
            case "BlindDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BlindDescription");
                text.text = itemText;
                break;
            case "BasicConceptsDescription":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BasicConceptsDescription");
                text.text = itemText;
                break;
            case "DamageDescriptionBase":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "DamageDescriptionBase");
                text.text = itemText;
                break;
            case "CooldownDescriptionBase":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "CooldownDescriptionBase");
                text.text = itemText;
                break;
            case "ChanceCritDescriptionBase":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "ChanceCritDescriptionBase");
                text.text = itemText;
                break;
            case "CritDamageDescriptionBase":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "CritDamageDescriptionBase");
                text.text = itemText;
                break;
            case "StaminaCostDescriptionBase":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "StaminaCostDescriptionBase");
                text.text = itemText;
                break;
            case "AccuracyDescriptionBase":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "AccuracyDescriptionBase");
                text.text = itemText;
                break;



            case "InCave1":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "InCave1");
                text.text = itemText;
                break;



            case "BackpackShopItemPage1":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopItemPage1");
                text.text = itemText;
                break;
            case "BackpackShopItemPage1_2":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopItemPage1_2");
                text.text = itemText;
                break;
            case "BackpackShopItemPage2":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopItemPage2");
                text.text = itemText;
                break;
            case "BackpackShopItemPage3":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopItemPage3");
                text.text = itemText;
                break;
            case "BackpackShopItemPage3_1":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopItemPage3_1");
                text.text = itemText;
                break;
            case "BackpackShopItemPage3_2":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopItemPage3_2");
                text.text = itemText;
                break;
            case "BackpackShopItemPage4":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopItemPage4");
                text.text = itemText;
                break;
            case "BackpackShopItemPage5":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopItemPage5");
                text.text = itemText;
                break;
            case "BackpackShopItemPage5_0":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopItemPage5_0");
                text.text = itemText;
                break;
            case "BackpackShopItemPage5_1":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopItemPage5_1");
                text.text = itemText;
                break;
            case "BackpackShopItemPage5_2":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopItemPage5_2");
                text.text = itemText;
                break;
            case "BackpackShopItemPage5_3":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopItemPage5_3");
                text.text = itemText;
                break;
            case "BackpackShopItemPage5_4":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopItemPage5_4");
                text.text = itemText;
                break;
            case "BackpackShopItemPage6":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopItemPage6");
                text.text = itemText;
                break;
            case "BackpackShopItemPage7":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopItemPage7");
                text.text = itemText;
                break;
            case "BackpackShopItemPage8":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopItemPage8");
                text.text = itemText;
                break;



            case "BackpackShopEatPage1":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopEatPage1");
                text.text = itemText;
                break;
            case "BackpackShopEatPage2":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "BackpackShopEatPage2");
                text.text = itemText;
                break;



            case "GenerateMapPage1":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "GenerateMapPage1");
                text.text = itemText;
                break;
            case "GenerateMapPage2":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "GenerateMapPage2");
                text.text = itemText;
                break;


            case "CavePage1":
                itemText = LocalizationManager.Instance.GetTextEducation(settingLanguage, "CavePage1");
                text.text = itemText;
                break;
        }
        
    }




}