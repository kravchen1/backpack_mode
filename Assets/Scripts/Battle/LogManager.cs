
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogManager : MonoBehaviour
{
    private GameObject placeForLogDescription;
    private TimeSpeed ts;


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

    private GameObject needGameobject;

    private void Start()
    {
        placeForLogDescription = GameObject.FindGameObjectWithTag("BattleLogContent");
        ts = GameObject.FindGameObjectWithTag("SliderTime").GetComponent<TimeSpeed>();
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

    public void CreateLogMessageAttackOnArmor(string nameItem, int damage, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        if (Player)
        {
            obj = Instantiate(DescriptionLogCharacterAttack, placeForLogDescription.GetComponent<RectTransform>().transform);
            text = nameItem + " destroy " + damage.ToString() + " armor.";
        }
        else
        {
            obj = Instantiate(DescriptionLogEnemyAttack, placeForLogDescription.GetComponent<RectTransform>().transform);
            text = nameItem + " destroy " + damage.ToString() + " armor.";
        }
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageAttackOnHalfArmor(string nameItem, int damage, int armor, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        if (Player)
        {
            obj = Instantiate(DescriptionLogCharacterAttack, placeForLogDescription.GetComponent<RectTransform>().transform);
            text = nameItem + " destroy " + damage.ToString() + " armor.";
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;

            obj = Instantiate(DescriptionLogCharacterAttack, placeForLogDescription.GetComponent<RectTransform>().transform);
            text = nameItem + " deal " + damage.ToString() + " damage.";
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
        }
        else
        {
            obj = Instantiate(DescriptionLogCharacterAttack, placeForLogDescription.GetComponent<RectTransform>().transform);
            text = nameItem + " destroy " + damage.ToString() + " armor.";
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;

            obj = Instantiate(DescriptionLogCharacterAttack, placeForLogDescription.GetComponent<RectTransform>().transform);
            text = nameItem + " deal " + damage.ToString() + " damage.";
            obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
            obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
        } 
    }

    public void CreateLogMessageAttackWithoutArmor(string nameItem, int damage, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        if (Player)
        {
            obj = Instantiate(DescriptionLogCharacterAttack, placeForLogDescription.GetComponent<RectTransform>().transform);
            text = nameItem + " deal " + damage.ToString() + " damage.";
        }
        else
        {
            obj = Instantiate(DescriptionLogEnemyAttack, placeForLogDescription.GetComponent<RectTransform>().transform);
            text = nameItem + " deal " + damage.ToString() + " damage.";
        }
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }


    public void CreateLogMessageMiss(string nameItem, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        obj = Instantiate(chooseLog("stamina", Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " miss.";

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageNoHaveStamina(string nameItem, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        obj = Instantiate(chooseLog("attack", Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " no have stamina.";

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }






    public void CreateLogMessageGive(string nameItem, string tagIcon, int count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " give " + count.ToString() + ".";

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageGive(string nameItem, string tagIcon, float count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " give " + count.ToString() + ".";

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageInflict(string nameItem, string tagIcon, int count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " inflict " + count.ToString() + ".";

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }



    public void CreateLogMessageUse(string nameItem, string tagIcon, int count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " used " + count.ToString() + ".";

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageUseFromEnemy(string nameItem, string tagIcon, int count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " used " + count.ToString() + " from enemy.";

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageReset(string nameItem, string tagIcon, int count, string resetItem, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " reset on " + count.ToString() + resetItem + ".";

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageSteal(string nameItem, string tagIcon, int count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " steal " + count.ToString() + ".";

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageBlock(string nameItem, string tagIcon, int count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " block " + count.ToString() + ".";

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageDecreaseStamina(string nameItem, string tagIcon, float count, string decreaseItem, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " decrease "+ decreaseItem + "stamina for " + count.ToString() + ".";


        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageRemove(string nameItem, string tagIcon, int count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " removed " + count.ToString() + ".";

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageReduced(string nameItem, string tagIcon, double count, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " cooldown reduced by " + count.ToString() + "%.";

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }

    public void CreateLogMessageReducedForItem(string nameItem, string tagIcon, double count, string reducedItem, bool Player)
    {
        GameObject obj;
        string text = "";
        string textTime = ts.nowTime.ToString() + "s";
        obj = Instantiate(chooseLog(tagIcon, Player), placeForLogDescription.GetComponent<RectTransform>().transform);
        text = nameItem + " cooldown reduced for " + reducedItem + " by " + count.ToString() + "%.";

        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = textTime;
    }



}