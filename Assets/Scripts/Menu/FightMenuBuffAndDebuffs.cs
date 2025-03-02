using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using System;
using TMPro;
public class FightMenuBuffAndDebuffs : MonoBehaviour
{
    public bool isPlayer = true;
    public List<GameObject> generateIcons;
    private GameObject[] prefabs;
    public List<Icon> icons = new List<Icon>();

    public GameObject bleedAnimation;
    public GameObject fatigueAnimation;
    public GameObject regenerateAnimation;

    public GameObject placeGenerationIconsBuffs;
    public GameObject placeGenerationIconsDebuffs;
    public GameObject OwnerStat;
    public GameObject DescriptionLog;
    private int rowIconsCount = 4;


    public float countPercentFire = 0.01f;
    public float countPercentFrost = 0.01f;

    public int countPercentEvasion = 2;
    public int countPercentChanceCrit = 2;
    public int countPercentBaseCrit = 5;
    public int countPercentPoisonDegenHP = 5;
    public int countPercentVampireRegenHP = 5;

    public int countPercentBlind = 5;
    public int countDamagePower = 1;
    public int countDamageBleed = 1;
    public int timerBleedAndFatigue = 1;
    public int countAddFatigue = 1;

    public int timerFatigueStart = 25;

    private int countFatigue = 0;
    
    private float timerCDFouting;

    private GameObject placeForLogDescription;

    public GameObject LogVampireStackCharacter, LogVampireStackEnemy;
    public GameObject LogPoisonStackCharacter, LogPoisonStackEnemy;
    private void Start()
    {
        timerCDFouting = timerFatigueStart;
        placeForLogDescription = GameObject.FindGameObjectWithTag("BattleLogContent");
        //LoadPrefabs("ICON");
        //AddBuff(10, "ICONBLEED");
    }



    public void AddBuff(int count, string buffName)
    {
        if (icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains(buffName.ToUpper())))
        {
            foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains(buffName.ToUpper())))
            {
                icon.countStack += count;
            }

        }
        else
        {
            var sceneGameObjectIcon = Instantiate(Resources.Load<GameObject>("Icons/"+ buffName), new Vector2(0, 0), Quaternion.identity, placeGenerationIconsBuffs.transform);
            var icon = sceneGameObjectIcon.GetComponent<Icon>();
            icon.sceneGameObjectIcon = sceneGameObjectIcon;
            icon.countStack = count;
            icon.Buff = true;
            icons.Add(icon);
        }
    }
    public void AddDebuff(int count, string debuffName)
    {
        if (icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains(debuffName.ToUpper())))
        {
            foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains(debuffName.ToUpper())))
            {
                icon.countStack += count;
            }

        }
        else
        {
            var sceneGameObjectIcon = Instantiate(Resources.Load<GameObject>("Icons/" + debuffName), new Vector2(0, 0), Quaternion.identity, placeGenerationIconsDebuffs.transform);
            var icon = sceneGameObjectIcon.GetComponent<Icon>();
            icon.sceneGameObjectIcon = sceneGameObjectIcon;
            icon.countStack = count;
            icon.Buff = false;
            icons.Add(icon);
        }
    }

    public void DeleteBuff(int count, string buffName)
    {
        Icon iconToRemove = null;

        foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains(buffName.ToUpper())))
        {
            if (icon.countStack - count <= 0)
            {
                icon.countStack = 0;
                iconToRemove = icon;
            }
            else
                icon.countStack -= count;
        }
        if (iconToRemove != null)
        {
            Destroy(iconToRemove.sceneGameObjectIcon);
            icons.Remove(iconToRemove);
        }
    }

    //public void DeleteDebuff(int count, string debuffName)
    //{
    //    Icon iconToRemove = null;
    //    foreach (var icon in icons)
    //    {
    //        if (icon.sceneGameObjectIcon.name.ToUpper().Contains(debuffName.ToUpper()))
    //        {
    //            iconToRemove = icon;
    //        }
    //        Destroy(icon.sceneGameObjectIcon);
            
    //    }
    //    if(iconToRemove != null)
    //        icons.Remove(iconToRemove);

    //    var oldIcons = icons;
    //    icons.Clear();
    //    foreach (var icon in oldIcons)
    //    {
    //        AddBuff(icon.countStack, icon.sceneGameObjectIcon.name.Replace("(Clone)", ""));
    //    }

    //}
    //public void addBleed(int count)
    //{
    //    foreach (var icon in Icons.Where(e => e.iconType.name.Contains("Bleed")))
    //    {
    //        icon.countStack++;
    //    }
    //}
    //public void addMana(int count)
    //{
    //    foreach (var icon in Icons.Where(e => e.iconType.name.Contains("Mana")))
    //    {
    //        icon.countStack++;
    //    }
    //}

    //создаём объект с позицией на сцене. Меняем позицию в зависимости от количества объектов в массиве?


    //public void ShowIcons()
    //{
    //    foreach(var icon in IconsPlayer)
    //    {
    //        if(icon.countStack > 0)
    //        {
    //            if(icon.sceneGameObjectIcon == null)
    //                icon.sceneGameObjectIcon = Instantiate(icon.iconType,);
    //        }
    //    }
    //}

    

    public void CalculateFireFrostStats()
    {
        List<Item> allItems = new List<Item>();
        GameObject backpack;
        if (gameObject.tag == "MenuFightPlayer")
        {
            backpack = GameObject.FindGameObjectWithTag("backpack");
            allItems = backpack.GetComponentsInChildren<Item>().ToList();
        }
        else
        {
            backpack = GameObject.FindGameObjectWithTag("backpackEnemy");
            allItems = backpack.GetComponentsInChildren<Item>().ToList();
        }

        if (icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN") || e.sceneGameObjectIcon.name.ToUpper().Contains("ICONFROST")))
        {
            int countBurn = 0, countFrost = 0;
            foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
            {
                countBurn = icon.countStack;
            }
            foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONFROST")))
            {
                countFrost = icon.countStack;
            }

            foreach (var item in allItems)
            {
                var changeCD = item.baseTimerCooldown * (countPercentFire * (countBurn - countFrost));
                if (item.baseTimerCooldown - changeCD > 0.1f)
                    item.timer_cooldown = item.baseTimerCooldown - changeCD;
                else
                    item.timer_cooldown = 0.1f;
            }
        }
    }



    public bool CalculateMissAccuracy(int accuracy)
    {
        if (icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBLIND")))
        {
            foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBLIND")))
            {
                accuracy -= (icon.countStack * countPercentBlind);
            }
        }

        int random = UnityEngine.Random.Range(1, 101);
        if (random <= accuracy)
            return true;
        else
            return false;
    }

    public int ReturnBlindAndAccuracy(int accuracy)
    {
        if (icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBLIND")))
        {
            foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBLIND")))
            {
                accuracy -= (icon.countStack * countPercentBlind);
            }
        }
        return accuracy;
    }

    public bool CalculateMissAvasion()
    {
        float evasion = 0;
        if (icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONEVASION")))
        {
            foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONEVASION")))
            {
                evasion = (icon.countStack * countPercentEvasion);
            }
        }

        int random = UnityEngine.Random.Range(1, 101);
        if (random <= evasion)
            return false; //false = увернулись
        else
            return true;
    }

    public int CalculateAddPower()
    {
        int result = 0;
        if (icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONPOWER")))
        {
            foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONPOWER")))
            {
                result = (icon.countStack * countDamagePower);
            }
        }
        return result;
    }


    public bool CalculateChanceCrit(int baseChanceCrit)
    {
        int countStackChanceCrit = 0;
        if (icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONCHANCECRIT")))
        {
            foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONCHANCECRIT")))
            {
                countStackChanceCrit = (icon.countStack * countPercentChanceCrit);
            }
        }

        int random = UnityEngine.Random.Range(1, 101);
        if (random <= (countStackChanceCrit + baseChanceCrit))
            return true; //крит!
        else
            return false;
    }

    public int CalculateChanceCrit()
    {
        int countStackChanceCrit = 0;
        if (icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONCHANCECRIT")))
        {
            foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONCHANCECRIT")))
            {
                countStackChanceCrit = (icon.countStack * countPercentChanceCrit);
            }
        }
        return countStackChanceCrit; //скок шанса крита?!

    }

    public float CalculateCritDamage(int baseCrit)
    {
        int countBaseCrit = 0;
        if (icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBASECRIT")))
        {
            foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBASECRIT")))
            {
                countBaseCrit = (icon.countStack * countPercentBaseCrit);
            }
        }
        countBaseCrit += baseCrit;

        return countBaseCrit / 100.0f;
    }

    public int CalculateHeal(int heal)
    {
        int countPoison = 0;
        if (icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONPOISON")))
        {
            foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONPOISON")))
            {
                countPoison = (icon.countStack * countPercentPoisonDegenHP);
            }
        }
        int reducedHP = (int)(heal / 100.0 * countPoison);
        heal -= reducedHP;

        if (reducedHP > 0)
        {
            if (isPlayer)
            {
                CreateLogMessage(LogPoisonStackCharacter, "head reduced by " + reducedHP.ToString());
            }
            else
            {
                CreateLogMessage(LogPoisonStackEnemy, "restored by " + reducedHP.ToString());
            }
        }

        return heal;
    }

    public int CalculateVampire(int damage)
    {
        int countVampire = 0;
        int countHeal = 0;
        if (icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONVAMPIRE")))
        {
            foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONVAMPIRE")))
            {
                countVampire = (icon.countStack * countPercentVampireRegenHP);
            }
        }
        countHeal = (int)(damage / 100.0 * countVampire);
        if (countHeal > 0)
        {
            if (isPlayer)
            {
                CreateLogMessage(LogVampireStackCharacter, "restored " + countHeal.ToString() + " hp");
            }
            else
            {
                CreateLogMessage(LogVampireStackEnemy, "restored " + countHeal.ToString() + " hp");
            }
        }
        if (countHeal > 0)
            return CalculateHeal(countHeal);
        else return 0;
    }


    public void CreateLogMessage(string message)
    {
        var obj = Instantiate(DescriptionLog, placeForLogDescription.GetComponent<RectTransform>().transform);
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        //obj.GetComponent<LogMessage>().nestedObject = gameObject;
    }

    public void CreateLogMessage(GameObject log, string message)
    {
        var obj = Instantiate(log, placeForLogDescription.GetComponent<RectTransform>().transform);
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        //obj.GetComponent<LogMessage>().nestedObject = gameObject;
    }


    private void StopDebuffAnimationPlayer()
    {
        GameObject goDebuffAnimation = GameObject.FindGameObjectWithTag("DebuffAnimationPlayer");
        goDebuffAnimation.GetComponent<Animator>().Play("New State");
    }
    private void StopDebuffAnimationEnemy()
    {
        GameObject goDebuffAnimation = GameObject.FindGameObjectWithTag("DebuffAnimationEnemy");
        goDebuffAnimation.GetComponent<Animator>().Play("New State");
    }
   
    private void Bleeding()
    {
        int countBleed = 0;
        foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBLEED")))
        {
            if (icon.countStack > 0)
            {
                countBleed = icon.countStack;
            }
        }
        if (countBleed > 0)
        {
            Debug.Log("bleed: " + countBleed.ToString());
            OwnerStat.GetComponent<PlayerBackpackBattle>().MinusHP((countBleed * countDamageBleed));
            //CreateLogMessage(transform.parent.parent.gameObject.name + " bleeding for " + Math.Abs(countBleed * countDamageBleed).ToString());
            bleedAnimation.transform.GetChild(1).GetComponent<TextMeshPro>().text = "-" + (countBleed * countDamageBleed).ToString();
            bleedAnimation.transform.GetChild(1).GetComponent<TextMeshPro>().fontSize = 750 + (countBleed * countDamageBleed);
            bleedAnimation.GetComponent<Animator>().Play("Activate", 0, 0f);
        }
    }


    private void Fatigue()
    {
        if (countFatigue > 0)
        {
            Debug.Log("fatigue: " + countFatigue.ToString());
            OwnerStat.GetComponent<PlayerBackpackBattle>().MinusHP(countFatigue);
            //CreateLogMessage(transform.parent.parent.gameObject.name + " fatiguing for " + Math.Abs(countFatigue).ToString());
            fatigueAnimation.transform.GetChild(1).GetComponent<TextMeshPro>().text = "-" + (countFatigue).ToString();
            fatigueAnimation.transform.GetChild(1).GetComponent<TextMeshPro>().fontSize = 750 + (countFatigue);
            fatigueAnimation.GetComponent<Animator>().Play("Activate", 0, 0f);
        }
    }

    private void Regenerate()
    {
        int countRegenerate = 0;
        foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONREGENERATE")))
        {
            if (icon.countStack > 0)
            {
                countRegenerate = icon.countStack;
            }
        }
        if(countRegenerate > 0)
        {
            Debug.Log("regenerate: " + countRegenerate.ToString());
            if(OwnerStat.GetComponent<PlayerBackpackBattle>().hp + countRegenerate >= OwnerStat.GetComponent<PlayerBackpackBattle>().maxHP)
            {
                OwnerStat.GetComponent<PlayerBackpackBattle>().hp = OwnerStat.GetComponent<PlayerBackpackBattle>().maxHP;
            }
            else
            {
                OwnerStat.GetComponent<PlayerBackpackBattle>().hp += countRegenerate;
            }

            //CreateLogMessage(transform.parent.parent.gameObject.name + " regenerating for " + Math.Abs(countRegenerate).ToString());
            regenerateAnimation.transform.GetChild(1).GetComponent<TextMeshPro>().text = "+" + (countRegenerate).ToString();
            regenerateAnimation.transform.GetChild(1).GetComponent<TextMeshPro>().fontSize = 750 + (countRegenerate);
            regenerateAnimation.GetComponent<Animator>().Play("Activate", 0, 0f);
        }
    }





    private bool timer_locked_outFlouting = true;
    private float timerCDBleed = 1f;
    private void CoolDownDebuffsBuffs()
    {
        timerCDBleed -= Time.deltaTime;
        if (timerCDBleed <= 0)
        {
            timerCDBleed = timerBleedAndFatigue;
            Bleeding();
            Fatigue();
            Regenerate();
        }
    }

    private void CoolDownFatigue()
    {
        timerCDFouting -= Time.deltaTime;
        if (timerCDFouting <= 0 && timer_locked_outFlouting)
        {
            timer_locked_outFlouting = false;
            timerCDFouting = timerBleedAndFatigue;
        }
        else if (timerCDFouting <= 0 && !timer_locked_outFlouting)
        {
            timerCDFouting = timerBleedAndFatigue;
            countFatigue += countAddFatigue;
        }
    }
    private void Update()
    {
        CoolDownDebuffsBuffs();
        CoolDownFatigue();
    }

}