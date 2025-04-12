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
        }
        
    }




}