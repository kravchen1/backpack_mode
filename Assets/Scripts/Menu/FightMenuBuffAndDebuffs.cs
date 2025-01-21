using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using System;
using TMPro;
public class FightMenuBuffAndDebuffs : MonoBehaviour
{
    public List<GameObject> generateIcons;
    private GameObject[] prefabs;
    public List<Icon> icons = new List<Icon>();

    public GameObject placeGenerationIcons;
    public GameObject OwnerStat;
    public GameObject DescriptionLog;
    private int rowIconsCount = 4;

    private float firstElementX = -20;
    private float firstElementY = 30;

    private float stepSizeX = 65f;
    private float stepSizeY = -50f;



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
    public void LoadPrefabs(string tagName)//а можно сделать функцией?
    {
        if (prefabs == null)
        {
            prefabs = Resources.LoadAll<GameObject>("");
        }
        generateIcons.AddRange(prefabs.Where(e => e.tag.ToUpper() == tagName).ToList());
    }


    private void Start()
    {
        timerCDFouting = timerFatigueStart;
        placeForLogDescription = GameObject.FindGameObjectWithTag("BattleLogContent");
        LoadPrefabs("ICON");
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
            var sceneGameObjectIcon = Instantiate(Resources.Load<GameObject>("Icons/"+ buffName), new Vector2(0, 0), Quaternion.identity, placeGenerationIcons.transform);
            var icon = sceneGameObjectIcon.GetComponent<Icon>();
            icon.countStack = count;
            icon.sceneGameObjectIcon = sceneGameObjectIcon;
            sceneGameObjectIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(firstElementX + stepSizeX * (icons.Count() % rowIconsCount), firstElementY + stepSizeY * ((int)(icons.Count() / rowIconsCount)));
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
            var sceneGameObjectIcon = Instantiate(Resources.Load<GameObject>("Icons/" + debuffName), new Vector2(0, 0), Quaternion.identity, placeGenerationIcons.transform);
            var icon = sceneGameObjectIcon.GetComponent<Icon>();
            icon.countStack = count;
            sceneGameObjectIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(firstElementX + stepSizeX * icons.Count() % rowIconsCount, firstElementY + stepSizeY * icons.Count() / rowIconsCount);
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

        foreach (var icon in icons)
        {
            Destroy(icon.sceneGameObjectIcon);
        }

        icons.Remove(iconToRemove);
        List<Icon> oldIcons = new List<Icon>(icons);
        icons.Clear();
        foreach (var icon in oldIcons)
        {
            AddBuff(icon.countStack, icon.sceneGameObjectIcon.name.Replace("(Clone)", ""));
        }
    }

    public void DeleteDebuff(int count, string debuffName)
    {
        Icon iconToRemove = null;
        foreach (var icon in icons)
        {
            if (icon.sceneGameObjectIcon.name.ToUpper().Contains(debuffName.ToUpper()))
            {
                iconToRemove = icon;
            }
            Destroy(icon.sceneGameObjectIcon);
            
        }
        if(iconToRemove != null)
            icons.Remove(iconToRemove);

        var oldIcons = icons;
        icons.Clear();
        foreach (var icon in oldIcons)
        {
            AddBuff(icon.countStack, icon.sceneGameObjectIcon.name.Replace("(Clone)", ""));
        }

    }
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
        
        heal -= heal / 100 * countPoison;
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
        countHeal = damage / 100 * countVampire;
        if (countHeal > 0)
            return CalculateHeal(countHeal);
        else return 0;
    }


    public void CreateLogMessage(string message)
    {
        var obj = Instantiate(DescriptionLog, placeForLogDescription.GetComponent<RectTransform>().transform);
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        obj.GetComponent<LogMessage>().nestedObject = gameObject;
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
    private void BleedingAndFatigue()
    {
        int countBleed = 0;
        foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBLEED")))
        {
            if (icon.countStack > 0)
            {
                countBleed = icon.countStack;
            }
        }

        if (countBleed > 0 && countFatigue > 0)
        {
            OwnerStat.GetComponent<PlayerBackpackBattle>().MinusHP((countBleed * countDamageBleed) + countFatigue);
            CreateLogMessage(transform.parent.parent.gameObject.name + " bleeding for " + Math.Abs(countBleed * countDamageBleed).ToString());
            CreateLogMessage(transform.parent.parent.gameObject.name + " fatiguing for " + Math.Abs(countFatigue).ToString());
            if (OwnerStat.name == "Character")//мы кровоточим и устали
            {
                GameObject goDebuffAnimation = GameObject.FindGameObjectWithTag("DebuffAnimationPlayer");
                goDebuffAnimation.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>().text = "-" + (countBleed * countDamageBleed).ToString();
                goDebuffAnimation.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>().fontSize = 750 + (countBleed * countDamageBleed);
                goDebuffAnimation.transform.GetChild(1).GetChild(1).GetComponent<TextMeshPro>().text = "-" + (countFatigue).ToString();
                goDebuffAnimation.transform.GetChild(1).GetChild(1).GetComponent<TextMeshPro>().fontSize = 750 + (countFatigue);
                goDebuffAnimation.GetComponent<Animator>().Play("BleedingAndFatiguingPlayer");
                Invoke("StopDebuffAnimationPlayer", 0.2f);
            }
            else//враг кровоточит и устал
            {
                GameObject goDebuffAnimation = GameObject.FindGameObjectWithTag("DebuffAnimationEnemy");
                goDebuffAnimation.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>().text = "-" + (countBleed * countDamageBleed).ToString();
                goDebuffAnimation.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>().fontSize = 750 + (countBleed * countDamageBleed);
                goDebuffAnimation.transform.GetChild(1).GetChild(1).GetComponent<TextMeshPro>().text = "-" + (countFatigue).ToString();
                goDebuffAnimation.transform.GetChild(1).GetChild(1).GetComponent<TextMeshPro>().fontSize = 750 + (countFatigue);
                goDebuffAnimation.GetComponent<Animator>().Play("BleedingAndFatiguingEnemy");
                Invoke("StopDebuffAnimationEnemy", 0.2f);
            }
        }
        else if (countBleed > 0)
        {
            OwnerStat.GetComponent<PlayerBackpackBattle>().MinusHP(countBleed * countDamageBleed);
            CreateLogMessage(transform.parent.parent.gameObject.name + " bleeding for " + Math.Abs(countBleed * countDamageBleed).ToString());
            if (OwnerStat.name == "Character")//мы кровоточим
            {
                GameObject goDebuffAnimation = GameObject.FindGameObjectWithTag("DebuffAnimationPlayer");
                goDebuffAnimation.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>().text = "-" + (countBleed * countDamageBleed).ToString();
                goDebuffAnimation.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>().fontSize = 750 + (countBleed * countDamageBleed);
                goDebuffAnimation.GetComponent<Animator>().Play("BleedingPlayer");
                Invoke("StopDebuffAnimationPlayer", 0.2f);
            }
            else//враг кровоточит
            {
                GameObject goDebuffAnimation = GameObject.FindGameObjectWithTag("DebuffAnimationEnemy");
                goDebuffAnimation.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>().text = "-" + (countBleed * countDamageBleed).ToString();
                goDebuffAnimation.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>().fontSize = 750 + (countBleed * countDamageBleed);
                goDebuffAnimation.GetComponent<Animator>().Play("BleedingEnemy");
                Invoke("StopDebuffAnimationEnemy", 0.2f);
            }
        }
        else if (countFatigue > 0)
        {
            OwnerStat.GetComponent<PlayerBackpackBattle>().MinusHP(countFatigue);
            CreateLogMessage(transform.parent.parent.gameObject.name + " fatiguing for " + Math.Abs(countFatigue).ToString());
            if (OwnerStat.name == "Character")//мы устали
            {
                GameObject goDebuffAnimation = GameObject.FindGameObjectWithTag("DebuffAnimationPlayer");
                goDebuffAnimation.transform.GetChild(1).GetChild(1).GetComponent<TextMeshPro>().text = "-" + (countFatigue).ToString();
                goDebuffAnimation.transform.GetChild(1).GetChild(1).GetComponent<TextMeshPro>().fontSize = 750 + (countFatigue);
                goDebuffAnimation.GetComponent<Animator>().Play("FatiguingPlayer");
                Invoke("StopDebuffAnimationPlayer", 0.2f);
            }
            else//враг устал
            {
                GameObject goDebuffAnimation = GameObject.FindGameObjectWithTag("DebuffAnimationEnemy");
                goDebuffAnimation.transform.GetChild(1).GetChild(1).GetComponent<TextMeshPro>().text = "-" + (countFatigue).ToString();
                goDebuffAnimation.transform.GetChild(1).GetChild(1).GetComponent<TextMeshPro>().fontSize = 750 + (countFatigue);
                goDebuffAnimation.GetComponent<Animator>().Play("FatiguingEnemy");
                Invoke("StopDebuffAnimationEnemy", 0.2f);
            }
        }


    }


    

    private bool timer_locked_outFlouting = true;
    private float timerCDBleed = 1f;
    private void CoolDownDebuffs()
    {
        timerCDBleed -= Time.deltaTime;
        if (timerCDBleed <= 0)
        {
            timerCDBleed = timerBleedAndFatigue;
            BleedingAndFatigue();
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
        CoolDownDebuffs();
        CoolDownFatigue();
    }

}