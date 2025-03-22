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

    public TextMeshPro weaponStat;

    public TextMeshPro textBody;
    public TextMeshPro Stats;

    public TextMeshPro type;
    public TextMeshPro rarity;


    public RectTransform bodyRect; // Родительский объект
    public RectTransform textRectForEndPosition;



    public TextMeshPro iconPoisonDescription;
    public TextMeshPro iconBleedingDescription;
    public TextMeshPro iconBlindDescription;
    public TextMeshPro iconBurnDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;
    //public TextMeshPro iconBaseCritDescription;




    public int damageMin = 1, damageMax = 2;
    public float staminaCost = 1;
    public int accuracyPercent = 95;
    public int chanceCrit = 5;
    public int critDamage = 130;
    public float cooldown = 1.1f;

    public int Armor = 0;

    

    

    private string settingLanguage = "ru";
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
    }
    public void SetWeaponStat()
    {
        if(weaponStat != null)
        {
            weaponStat.text = LocalizationManager.Instance.GetTextWeaponStat(settingLanguage);
        }
    }
    public void SetTypeAndRarity(ItemsText itemText)
    {
        type.text = itemText.type;
        rarity.text = itemText.rarity;
    }
    public void SetFont()
    {
        if(settingLanguage == "zh-CN")
        {
            var font = Resources.Load<TMP_FontAsset>("Fonts/china main SDF");
            weaponStat.font = font;
            Stats.font = font;
        }
    }
    public void SetTextBody()
    {
        SetWeaponStat();
        SetIconDescriptions();
        SetFont();

        ItemsText itemText = LocalizationManager.Instance.GetTextItem(settingLanguage, originalName);
        SetTypeAndRarity(itemText);
        string pattern = @"Icon(\w+)\(([-\d.]+)f,([-\d.]+)f\)";
        Regex regex = new Regex(pattern);

        // Список для хранения найденных данных
        var iconData = new List<(string Name, string Number1, string Number2)>();

        // Поиск всех совпадений
        MatchCollection matches = regex.Matches(itemText.description);
        foreach (Match match in matches)
        {
            string iconName = match.Groups[1].Value; // Имя иконки (например, Bleed)
            string number1 = match.Groups[2].Value;  // Первое число (например, 284)
            string number2 = match.Groups[3].Value;  // Второе число (например, -317)

            // Добавляем данные в список
            iconData.Add((iconName, number1, number2));
        }

        // Удаляем блоки с Icon из исходного текста
        string cleanedText = regex.Replace(itemText.description, "").Trim();

        





        string patternFigure = @"\{\d+\}";
        Regex regexFigrue = new Regex(patternFigure);

        // Поиск всех совпадений
        MatchCollection matchesFigure = regexFigrue.Matches(itemText.description);

        // Количество вхождений
        int count = matchesFigure.Count;



        switch (count)
        {
            case 0:
                textBody.text = cleanedText;
                break;
            case 1:
                textBody.text = string.Format(cleanedText, GetField(0));
                break;
            case 2:
                textBody.text = string.Format(cleanedText, GetField(0), GetField(1));
                break;
            case 3:
                textBody.text = string.Format(cleanedText, GetField(0), GetField(1), GetField(2));
                break;
            case 4:
                textBody.text = string.Format(cleanedText, GetField(0), GetField(1), GetField(2), GetField(3));
                break;
            case 5:
                textBody.text = string.Format(cleanedText, GetField(0), GetField(1), GetField(2), GetField(3), GetField(4));
                break;
        }

        foreach (var data in iconData)
        {
            //Console.WriteLine($"Icon: {data.Name}, Number1: {data.Number1}, Number2: {data.Number2}");
            //
            //Vector2 endPosition = CalculateTextEndPosition(bodyRect, textRectForEndPosition);
            var icon = Instantiate(Resources.Load<GameObject>("Icons/Icon" + data.Name), gameObject.transform.GetChild(1).transform);
            icon.GetComponent<RectTransform>().anchoredPosition = new Vector3(float.Parse(data.Number1), float.Parse(data.Number2), 0);
            //icon.GetComponent<RectTransform>().anchoredPosition = new Vector3(endPosition.x, endPosition.y, 0);
            
        }
        
        //Debug.Log(endPosition);
    }

    public int GetField(int index)
    {
        var childType = this.GetType();

        // Получаем все поля дочернего класса
        FieldInfo[] fields = childType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);


        return (int)fields[index].GetValue(this);
    }


    Vector2 CalculateTextEndPosition(RectTransform parentRectTransform, RectTransform textRectTransform)
    {
        // Размер родительского объекта
        Vector2 parentSize = parentRectTransform.rect.size;

        // Якоря текста
        Vector2 anchorMin = textRectTransform.anchorMin; // MinX, MinY
        Vector2 anchorMax = textRectTransform.anchorMax; // MaxX, MaxY

        // Размер текста
        Vector2 textSize = textRectTransform.sizeDelta;

        // anchoredPosition текста
        Vector2 anchoredPosition = textRectTransform.anchoredPosition;

        // Вычисляем абсолютные координаты начала текста
        Vector2 textStartPosition = new Vector2(
            parentSize.x * anchorMin.x + anchoredPosition.x,
            parentSize.y * anchorMin.y + anchoredPosition.y
        );

        // Вычисляем конечную точку текста
        Vector2 textEndPosition = textStartPosition + textSize;

        return textEndPosition;
    }

    private void Awake()
    {
    }

    void Update()
    {
        
    }
}
