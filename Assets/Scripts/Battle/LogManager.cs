
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    private GameObject placeForLogDescription;
    private TimeSpeed ts;
    public ScrollRect scrollRect;


    public GameObject DescriptionLogCharacterAttack;//1
    public GameObject DescriptionLogEnemyAttack;//1

    public GameObject DescriptionLogCharacterArmor;
    public GameObject DescriptionLogEnemyArmor;


    public GameObject DescriptionLogCharacterBaseCrit;
    public GameObject DescriptionLogEnemyBaseCrit;

    public GameObject DescriptionLogCharacterBleed;
    public GameObject DescriptionLogEnemyBleed;

    public GameObject DescriptionLogCharacterBlind;
    public GameObject DescriptionLogEnemyBlind;

    public GameObject DescriptionLogCharacterChanceCrit;
    public GameObject DescriptionLogEnemyChanceCrit;

    public GameObject DescriptionLogCharacterEvasion;
    public GameObject DescriptionLogEnemyEvasion;

    public GameObject DescriptionLogCharacterFire;
    public GameObject DescriptionLogEnemyFire;

    public GameObject DescriptionLogCharacterFrost;
    public GameObject DescriptionLogEnemyFrost;

    public GameObject DescriptionLogCharacterMana;
    public GameObject DescriptionLogEnemyMana;

    public GameObject DescriptionLogCharacterPoison;
    public GameObject DescriptionLogEnemyPoison;

    public GameObject DescriptionLogCharacterPower;
    public GameObject DescriptionLogEnemyPower;

    public GameObject DescriptionLogCharacterRegenerate;
    public GameObject DescriptionLogEnemyRegenerate;

    public GameObject DescriptionLogCharacterResistance;
    public GameObject DescriptionLogEnemyResistance;

    public GameObject DescriptionLogCharacterStamina;
    public GameObject DescriptionLogEnemyStamina;

    public GameObject DescriptionLogCharacterTimer;
    public GameObject DescriptionLogEnemyTimer;

    public GameObject DescriptionLogCharacterVampire;
    public GameObject DescriptionLogEnemyVampire;



    private string settingLanguage = "en";
    private GridLayoutGroup gridLayoutGroup;
    private Vector2 scrollPosition;
    
    private void Start()
    {
        placeForLogDescription = GameObject.FindGameObjectWithTag("BattleLogContent");
        gridLayoutGroup = placeForLogDescription.GetComponent<GridLayoutGroup>();
        ts = GameObject.FindGameObjectWithTag("SliderTime").GetComponent<TimeSpeed>();
        settingLanguage = PlayerPrefs.GetString("LanguageSettings");
    }


    private GameObject chooseLog(string tag, bool Player)
    {
        switch(tag.ToLower())
        {
            case "attack":
                return Player ? DescriptionLogCharacterAttack : DescriptionLogEnemyAttack;
            case "armor":
                return Player ? DescriptionLogCharacterArmor : DescriptionLogEnemyArmor;
            case "basecrit":
                return Player ? DescriptionLogCharacterBaseCrit : DescriptionLogEnemyBaseCrit;
            case "bleed":
                return Player ? DescriptionLogCharacterBleed : DescriptionLogEnemyBleed;
            case "blind":
                return Player ? DescriptionLogCharacterBlind : DescriptionLogEnemyBlind;
            case "chancecrit":
                return Player ? DescriptionLogCharacterChanceCrit : DescriptionLogEnemyChanceCrit;
            case "evasion":
                return Player ? DescriptionLogCharacterEvasion : DescriptionLogEnemyEvasion;
            case "fire":
                return Player ? DescriptionLogCharacterFire : DescriptionLogEnemyFire;
            case "frost":
                return Player ? DescriptionLogCharacterFrost : DescriptionLogEnemyFrost;
            case "mana":
                return Player ? DescriptionLogCharacterMana : DescriptionLogEnemyMana;
            case "poison":
                return Player ? DescriptionLogCharacterPoison : DescriptionLogEnemyPoison;
            case "power":
                return Player ? DescriptionLogCharacterPower : DescriptionLogEnemyPower;
            case "regenerate":
                return Player ? DescriptionLogCharacterRegenerate : DescriptionLogEnemyRegenerate;
            case "stamina":
                return Player ? DescriptionLogCharacterStamina : DescriptionLogCharacterStamina;
            case "timer":
                return Player ? DescriptionLogCharacterTimer : DescriptionLogEnemyTimer;
            case "vampire":
                return Player ? DescriptionLogCharacterVampire : DescriptionLogEnemyVampire;
            case "resist":
                return Player ? DescriptionLogCharacterResistance : DescriptionLogEnemyResistance;
            default:
                return Player ? DescriptionLogCharacterAttack : DescriptionLogEnemyAttack;

        }
    }

    private void SaveScroll()
    {
        // 1. Сохраняем позицию скролла
        scrollPosition = scrollRect.normalizedPosition;

        // 2. Отключаем всё, что может влиять на расчёт
        gridLayoutGroup.enabled = false;
        if (TryGetComponent(out ContentSizeFitter fitter))
            fitter.enabled = false;
    }

    private void LoadScroll()
    {
        // 4. Включаем обратно и обновляем
        gridLayoutGroup.enabled = true;

        if (TryGetComponent(out ContentSizeFitter fitter))
            fitter.enabled = true;

        LayoutRebuilder.ForceRebuildLayoutImmediate(gridLayoutGroup.GetComponent<RectTransform>());

        // 5. Восстанавливаем скролл
        Canvas.ForceUpdateCanvases();
        scrollRect.normalizedPosition = scrollPosition;
    }

    public void CreateLogMessageAttackOnArmor(string nameItem, int damage, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        SaveScroll();
        obj = Instantiate(chooseLog("attack", Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        LoadScroll();
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Destroy") + " " + damage.ToString() + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Armor") + ".";

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageAttackOnHalfArmor(string nameItem, int damage, int armor, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        
        SaveScroll();
        obj = Instantiate(chooseLog("attack", Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Destroy") + " " + damage.ToString() + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Armor") + ".";
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
        LoadScroll();

        SaveScroll();
        obj = Instantiate(chooseLog("attack", Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Deal") + " " + damage.ToString() + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Damage") + ".";
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
        LoadScroll();


    }

    public void CreateLogMessageAttackWithoutArmor(string nameItem, int damage, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";

        SaveScroll();
        obj = Instantiate(chooseLog("attack", Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Deal") + " " + damage.ToString() + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Damage") + ".";
        LoadScroll();

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }


    public void CreateLogMessageMiss(string nameItem, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";

        SaveScroll();
        obj = Instantiate(chooseLog("stamina", Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Miss") + ".";
        LoadScroll();

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageNoHaveStamina(string nameItem, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";

        SaveScroll();
        obj = Instantiate(chooseLog("attack", Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "NoHaveStamina") + ".";
        LoadScroll();

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }






    public void CreateLogMessageGive(string nameItem, string tagIcon, int count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";

        SaveScroll();
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Give") + " " + count.ToString() + ".";
        LoadScroll();

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageGive(string nameItem, string tagIcon, float count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";

        SaveScroll();
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Give") + " " + count.ToString() + ".";
        LoadScroll();

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageInflict(string nameItem, string tagIcon, int count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";

        SaveScroll();
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Inflict") + " " + count.ToString() + ".";
        LoadScroll();

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }



    public void CreateLogMessageUse(string nameItem, string tagIcon, int count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";

        SaveScroll();
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Used") + " " + count.ToString() + ".";
        LoadScroll();

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageUseFromEnemy(string nameItem, string tagIcon, int count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";

        SaveScroll();
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Used") + " " + count.ToString() + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "FromEnemy") + ".";
        LoadScroll();

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageSteal(string nameItem, string tagIcon, int count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";

        SaveScroll();
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Steal") + " " + count.ToString() + ".";
        LoadScroll();
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageBlock(string nameItem, string tagIcon, int count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";

        SaveScroll();
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Block") + " " + count.ToString() + ".";
        LoadScroll();

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageDecreaseStamina(string nameItem, string tagIcon, float count, string decreaseItem, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";

        SaveScroll();
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "DecreaseStamina") + " " + decreaseItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "On") + " " + count.ToString() + ".";
        LoadScroll();

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageRemove(string nameItem, string tagIcon, int count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";

        SaveScroll();
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "Remove") + " " + count.ToString() + ".";
        LoadScroll();
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageReduced(string nameItem, string tagIcon, double count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";

        SaveScroll();
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "CooldownReducded") + " " + count.ToString() + "%.";
        LoadScroll();

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageReducedForItem(string nameItem, string tagIcon, double count, string reducedItem, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";

        SaveScroll();
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "CooldownReducdedFor") + " " + reducedItem + " " + LocalizationManager.Instance.GetTextBattleLog(settingLanguage, "By") + " " + count.ToString() + "%.";
        LoadScroll();

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }



}