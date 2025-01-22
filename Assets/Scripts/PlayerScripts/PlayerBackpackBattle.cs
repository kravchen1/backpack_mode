using System;
using System.Xml.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class PlayerBackpackBattle : MonoBehaviour
{

    public GameObject backpack;
    public GameObject armorBar;
    public GameObject hpBar;
    public GameObject staminaBar;
    public GameObject expBar;
    public GameObject menuFightIcon;


    public float armor = 0f;
    public float armorMax = 0f;

    public float hp = 74f;
    public float maxHP = 100f;

    public float stamina = 74f;
    public float staminaMax = 100f;
    public float staminaRegenerate = 1f;

    public int enemyExp;
    public int enemyCoins;

    public CharacterStats characterStats;



    private Canvas canvas;
    private Image[] hpBarImages;
    private Image[] staminaBarImages;
    private Image[] armorBarImages;

    private Text[] textBarHP;
    private Text[] textBarStamina;
    private Text[] textBarArmor;

    private string characterStatsDataFilePath;
    private CharacterStatsData characterStatsData;
    public FightMenuBuffAndDebuffs menuFightIconData;
    private EnemyStatData enemyStatData;
    void Awake()
    {
        InitializeData();
    }

    void InitializeData()
    {
        canvas = armorBar.GetComponentInParent<Canvas>();

        hpBarImages = hpBar.GetComponentsInChildren<Image>();
        staminaBarImages = staminaBar.GetComponentsInChildren<Image>();
        armorBarImages = armorBar.GetComponentsInChildren<Image>();

        textBarHP = hpBar.GetComponentsInChildren<Text>();
        textBarStamina = staminaBar.GetComponentsInChildren<Text>();
        textBarArmor = armorBar.GetComponentsInChildren<Text>();

        menuFightIconData = menuFightIcon.GetComponent<FightMenuBuffAndDebuffs>();

        switch (gameObject.name)
        {
            case "Character":
                characterStats = GetComponent<CharacterStats>();
                characterStats.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "characterStatsData.json"));
                characterStats.InitializeCharacterStats();
                hp = characterStats.playerHP;
                maxHP = characterStats.playerHP; //todo
                break;
            case "CharacterEnemy":
                characterStats = GetComponent<CharacterStats>();
                enemyStatData = new EnemyStatData("{\"playerHP\":20000.0,\"playerExp\":0.0,\"playerCoins\":919.0,\"requiredExp\":1000.0,\"playerLvl\":1.0}");
                characterStats.LoadDataEnemy(enemyStatData.jsonStat);//todo
                characterStats.InitializeCharacterStats();
                hp = characterStats.playerHP;
                maxHP = characterStats.playerHP; //todo
                break;
        }
        enemyExp = 100;
        enemyCoins = 10;
        
    }



    void changeBar(Image[] images, Text[] texts, float currentValue, float maxValue)
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i] != null)
            {
                if (images[i].tag == "Bar")
                {
                    images[i].fillAmount = currentValue / maxValue;
                }
            }
        }

        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] != null)
            {
                if (texts[i].tag == "TextBar")
                {
                        texts[i].text = currentValue + "/" + maxValue;
                }
            }
        }
    }

    public void ShowArmor(Image[] images, Text[] texts, float currentValue, float maxValue)
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (images[i] != null)
            {
                if (images[i].tag == "Bar")
                {
                    images[i].fillAmount = currentValue / maxValue;
                }
            }
        }


        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] != null)
            {
                if (texts[i].tag == "TextBar")
                {
                    if (armor > 0)
                    {
                        canvas.enabled = true;
                        texts[i].enabled = true;
                        texts[i].text = currentValue + "/" + maxValue;
                    }
                    else
                    {
                        canvas.enabled = false;
                        texts[i].enabled = false;
                    }

                }
            }
        }
    }

    public void staminaRegenerating()
    {
        if (stamina < staminaMax)
        {
            stamina += staminaRegenerate * Time.deltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        changeBar(hpBarImages, textBarHP, hp, maxHP);
        changeBar(staminaBarImages, textBarStamina, stamina, staminaMax);
        ShowArmor(armorBarImages, textBarArmor, armor, armorMax);

        if (gameObject.name == "Character")
        {
            characterStats.playerHP = Convert.ToInt32(hp);
            //characterStats.hpText.text = characterStats.playerHP.ToString();
        }

        staminaRegenerating();

    }






    public void MinusHP(int damage)
    {
        float armorBefore = armor;
        if (damage < armorBefore)
        {
            armor -= damage;
        }
        else
        {
            int dmgArmor = (int)armorBefore;
            armor = 0;
            hp -= (damage - dmgArmor);
            if (armorBefore == 0)
            {
                //CreateLogMessage(gameObject.name + " apply " + Math.Abs((Enemy.armor - damage)).ToString() + " damage");
            }
            else
            {
                //CreateLogMessage(gameObject.name + " destroy " + armorBefore.ToString() + " armor and apply " + Math.Abs((Enemy.armor - damage)).ToString() + " damage");
            }
        }
    }
}
