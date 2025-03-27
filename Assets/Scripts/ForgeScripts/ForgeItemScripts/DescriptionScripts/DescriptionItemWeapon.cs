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

public class DescriptionItemWeapon : DescriptionItem
{

    public TextMeshPro weaponStat;

    public TextMeshPro Stats;


    public int damageMin = 1, damageMax = 2;
    public float staminaCost = 1;
    public int accuracyPercent = 95;
    public int chanceCrit = 5;
    public int critDamage = 130;
    public float cooldown = 1.1f;



    private new void Start()
    {
        SetTextBody();
        SetTextStat();
    }



    
    public void SetWeaponStat()
    {
        if(weaponStat != null)
        {
            weaponStat.text = LocalizationManager.Instance.GetTextWeaponStat(settingLanguage);
        }
    }

    public void SetTextStat()
    {
        string text = "<u>" + damageMin.ToString() + "</u>-<u>" + damageMax.ToString() + "</u> (" + Mathf.Round(((damageMin + damageMax) / 2 / cooldown)).ToString() + ")/sec"
            + "\r\n<u>" + chanceCrit.ToString() +
            "</u>%\r\n<u>" + critDamage.ToString() +
            "</u>%\r\n<u>" + staminaCost.ToString() +
            "</u>\r\n<u>" + accuracyPercent.ToString() +
            "</u>%\r\n<u>" + cooldown.ToString() + "</u> sec";

        Stats.text = text;
    }

    public override void SetTextBody()
    {
        SetWeaponStat();
        SetWeight();
        SetIconDescriptions();
        SetFont();
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
}
