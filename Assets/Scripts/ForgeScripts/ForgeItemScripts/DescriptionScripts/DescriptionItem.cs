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

    

    

    public string settingLanguage = "ru";
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
    public void SetFont()
    {
        //if (settingLanguage == "zh-CN")
        //{
        //    var font = Resources.Load<TMP_FontAsset>("Fonts/china main SDF");
        //    weaponStat.font = font;
        //    Stats.font = font;
        //}
    }
    public virtual void SetTextBody()
    {
        SetWeight();
        SetIconDescriptions();
        SetFont();
        ItemsText itemText = LocalizationManager.Instance.GetTextItem(settingLanguage, originalName);
        SetTypeAndRarity(itemText);



        // ��������� Sprite Asset
        TMP_SpriteAsset spriteAsset = Resources.Load<TMP_SpriteAsset>("Icons/iconsAtlas");

        if (spriteAsset == null)
        {
            Debug.LogError($"SpriteAsset not found at path: \"Icons/iconsAtlas\"");
            return;
        }

        textBody.spriteAsset = spriteAsset;



        string patternFigure = @"\{\d+\}";
        Regex regexFigrue = new Regex(patternFigure);

        // ����� ���� ����������
        MatchCollection matchesFigure = regexFigrue.Matches(itemText.description);

        // ���������� ���������
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

        // �������������� ���������� (�� ������ ������)
        textBody.ForceMeshUpdate();

    }

    public object GetField(int index)
    {
        var childType = this.GetType();

        // �������� ��� ���� ��������� ������
        FieldInfo[] fields = childType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);


        return fields[index].GetValue(this);
    }


    public void Start()
    {
        SetTextBody();
    }
}
