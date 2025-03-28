using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using Unity.Burst.CompilerServices;
using UnityEditor.VersionControl;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class DescriptionItem : MonoBehaviour
{
    public string originalName = "hiddenDagger";

    public TextMeshPro textBody;

    public TextMeshPro type;
    public TextMeshPro rarity;
    public TextMeshPro weightText;

    public TextMeshPro iconPoisonDescription;
    public TextMeshPro iconBleedingDescription;
    public TextMeshPro iconBlindDescription;
    public TextMeshPro iconBurnDescription;
    public TextMeshPro iconIceDescription;
    public TextMeshPro iconResistanceDescription;
    public TextMeshPro iconChanceCritDescription;
    public TextMeshPro iconRegenerateDescription;
    public TextMeshPro iconEvasionDescription;
    public TextMeshPro iconManaDescription;
    public TextMeshPro iconPowerDescription;
    public TextMeshPro iconVampireDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;


    public float weight = 0.5f;

    

    

    protected string settingLanguage = "en";
    public void SetIconDescriptions()
    {
        if(iconPoisonDescription != null)
        {
            iconPoisonDescription.text = LocalizationManager.Instance.GetTextIconDescriptionEducation(settingLanguage, "Poison").description;
        }
        if (iconBleedingDescription != null)
        {
            iconBleedingDescription.text = LocalizationManager.Instance.GetTextIconDescriptionEducation(settingLanguage, "Bleed").description;
        }
        if (iconBlindDescription != null)
        {
            iconBlindDescription.text = LocalizationManager.Instance.GetTextIconDescriptionEducation(settingLanguage, "Blind").description;
        }
        if (iconBurnDescription != null)
        {
            iconBurnDescription.text = LocalizationManager.Instance.GetTextIconDescriptionEducation(settingLanguage, "Burn").description;
        }
        if (iconIceDescription != null)
        {
            iconIceDescription.text = LocalizationManager.Instance.GetTextIconDescriptionEducation(settingLanguage, "Frost").description;
        }
        if (iconResistanceDescription != null)
        {
            iconResistanceDescription.text = LocalizationManager.Instance.GetTextIconDescriptionEducation(settingLanguage, "Resistance").description;
        }
        if (iconChanceCritDescription != null)
        {
            iconChanceCritDescription.text = LocalizationManager.Instance.GetTextIconDescriptionEducation(settingLanguage, "ChanceCrit").description;
        }
        if (iconRegenerateDescription != null)
        {
            iconRegenerateDescription.text = LocalizationManager.Instance.GetTextIconDescriptionEducation(settingLanguage, "Regenerate").description;
        }
        if (iconEvasionDescription != null)
        {
            iconEvasionDescription.text = LocalizationManager.Instance.GetTextIconDescriptionEducation(settingLanguage, "Evasion").description;
        }
        if (iconManaDescription != null)
        {
            iconManaDescription.text = LocalizationManager.Instance.GetTextIconDescriptionEducation(settingLanguage, "Mana").description;
        }
        if (iconPowerDescription != null)
        {
            iconPowerDescription.text = LocalizationManager.Instance.GetTextIconDescriptionEducation(settingLanguage, "Power").description;
        }
        if (iconVampireDescription != null)
        {
            iconVampireDescription.text = LocalizationManager.Instance.GetTextIconDescriptionEducation(settingLanguage, "Vampire").description;
        }
        

    }
    
    public void SetWeight()
    {
        if (weightText != null)
        {
            weightText.text = string.Format(LocalizationManager.Instance.GetTextWeight(settingLanguage), weight);
        }
    }
    public void SetTypeAndRarity(ItemsText itemText)
    {
        type.text = itemText.type;
        rarity.text = itemText.rarity;
    }
    public virtual void SetFont()
    {
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts/NotoSansSC-Black SDF");
        switch (settingLanguage)
        {
            case "zh":
                textBody.font = font;
                type.font = font;
                rarity.font = font;
                weightText.font = font;

                textBody.lineSpacing = -30;
                type.lineSpacing = -30;
                rarity.lineSpacing = -30;
                weightText.lineSpacing = -30;

                if (iconPoisonDescription != null)
                    iconPoisonDescription.font = font;
                if (iconBleedingDescription != null)
                    iconBleedingDescription.font = font;
                if (iconBlindDescription != null)
                    iconBlindDescription.font = font;
                if (iconBurnDescription != null)
                    iconBurnDescription.font = font;
                if (iconIceDescription != null)
                    iconIceDescription.font = font;
                if (iconResistanceDescription != null)
                    iconResistanceDescription.font = font;
                if (iconChanceCritDescription != null)
                    iconChanceCritDescription.font = font;
                if (iconRegenerateDescription != null)
                    iconRegenerateDescription.font = font;
                if (iconEvasionDescription != null)
                    iconEvasionDescription.font = font;
                if (iconPowerDescription != null)
                    iconPowerDescription.font = font;
                if (iconVampireDescription != null)
                    iconVampireDescription.font = font;
                break;
            case "zh_tw":
                textBody.font = font;
                type.font = font;
                rarity.font = font;
                weightText.font = font;

                textBody.lineSpacing = -30;
                type.lineSpacing = -30;
                rarity.lineSpacing = -30;
                weightText.lineSpacing = -30;

                if (iconPoisonDescription != null)
                    iconPoisonDescription.font = font;
                if (iconBleedingDescription != null)
                    iconBleedingDescription.font = font;
                if (iconBlindDescription != null)
                    iconBlindDescription.font = font;
                if (iconBurnDescription != null)
                    iconBurnDescription.font = font;
                if (iconIceDescription != null)
                    iconIceDescription.font = font;
                if (iconResistanceDescription != null)
                    iconResistanceDescription.font = font;
                if (iconChanceCritDescription != null)
                    iconChanceCritDescription.font = font;
                if (iconRegenerateDescription != null)
                    iconRegenerateDescription.font = font;
                if (iconEvasionDescription != null)
                    iconEvasionDescription.font = font;
                if (iconPowerDescription != null)
                    iconPowerDescription.font = font;
                if (iconVampireDescription != null)
                    iconVampireDescription.font = font;
                break;
            default:
                textBody.font = font;
                type.font = font;
                rarity.font = font;
                weightText.font = font;

                textBody.lineSpacing = -30;
                type.lineSpacing = -30;
                rarity.lineSpacing = -30;
                weightText.lineSpacing = -30;

                if (iconPoisonDescription != null)
                    iconPoisonDescription.font = font;
                if (iconBleedingDescription != null)
                    iconBleedingDescription.font = font;
                if (iconBlindDescription != null)
                    iconBlindDescription.font = font;
                if (iconBurnDescription != null)
                    iconBurnDescription.font = font;
                if (iconIceDescription != null)
                    iconIceDescription.font = font;
                if (iconResistanceDescription != null)
                    iconResistanceDescription.font = font;
                if (iconChanceCritDescription != null)
                    iconChanceCritDescription.font = font;
                if (iconRegenerateDescription != null)
                    iconRegenerateDescription.font = font;
                if (iconEvasionDescription != null)
                    iconEvasionDescription.font = font;
                if (iconPowerDescription != null)
                    iconPowerDescription.font = font;
                if (iconVampireDescription != null)
                    iconVampireDescription.font = font;
                break;
        }
    }
    public virtual void SetTextBody()
    {
        //en ru zh zh_tw
        settingLanguage = PlayerPrefs.GetString("LanguageSettings");
        SetFont();
        SetWeight();
        SetIconDescriptions();
        ItemsText itemText = LocalizationManager.Instance.GetTextItem(settingLanguage, originalName);
        SetTypeAndRarity(itemText);



        // Загружаем Sprite Asset
        TMP_SpriteAsset spriteAsset = Resources.Load<TMP_SpriteAsset>("Icons/iconsAtlas");

        if (spriteAsset == null)
        {
            Debug.LogError($"SpriteAsset not found at path: \"Icons/iconsAtlas\"");
            return;
        }

        textBody.spriteAsset = spriteAsset;



        string patternFigure = @"\{\d+\}";
        Regex regexFigrue = new Regex(patternFigure);

        // Поиск всех совпадений
        MatchCollection matchesFigure = regexFigrue.Matches(itemText.description);

        // Количество вхождений
        int count = matchesFigure.Count;

        switch (count)
        {
            case 0:
                textBody.text = itemText.description;
                break;
            case 1:
                textBody.text = string.Format(itemText.description, GetField(0));
                break;
            case 2:
                textBody.text = string.Format(itemText.description, GetField(0), GetField(1));
                break;
            case 3:
                textBody.text = string.Format(itemText.description, GetField(0), GetField(1), GetField(2));
                break;
            case 4:
                textBody.text = string.Format(itemText.description, GetField(0), GetField(1), GetField(2), GetField(3));
                break;
            case 5:
                textBody.text = string.Format(itemText.description, GetField(0), GetField(1), GetField(2), GetField(3), GetField(4));
                break;
        }

        // Принудительное обновление (на всякий случай)
        textBody.ForceMeshUpdate();

    }

    public object GetField(int index)
    {
        var childType = this.GetType();

        // Получаем все поля дочернего класса
        FieldInfo[] fields = childType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);


        return fields[index].GetValue(this);
    }

    public void Start()
    {
        
        SetTextBody();
    }
}
