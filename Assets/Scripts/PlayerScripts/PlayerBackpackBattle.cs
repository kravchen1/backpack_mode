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
    [HideInInspector] public GameObject expBar;


    [HideInInspector] public float armor = 0f;
    [HideInInspector] public float armorMax = 0f;

    [HideInInspector] public float hp = 74f;
    [HideInInspector] public float maxHP = 100f;

    [HideInInspector] public float stamina = 74f;
    [HideInInspector] public float staminaMax = 100f;
    [HideInInspector] public float staminaRegenerate = 1f;


    [HideInInspector] public CharacterStats characterStats;



    private Canvas canvas;
    public Image hpBarImage;
    public Image staminaBarImage;
    public Image armorBarImage;

    public TextMeshProUGUI textBarHP;
    public TextMeshProUGUI textBarStamina;
    public TextMeshProUGUI textBarArmor;

    private string characterStatsDataFilePath;
    private CharacterStatsData characterStatsData;
    public GameObject menuFightIcon;
    [HideInInspector] public FightMenuBuffAndDebuffs menuFightIconData;
    private EnemyStatData enemyStatData;
    void Awake()
    {
        InitializeData();
    }

    void InitializeData()
    {
        switch (gameObject.name)
        {
            case "Character":
                characterStats = GetComponent<CharacterStats>();
                characterStats.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "characterStatsData.json"));
                characterStats.InitializeCharacterStats();
                hp = characterStats.playerHP;
                maxHP = characterStats.playerHP; 
                staminaMax = characterStats.playerMaxStamina;
                stamina = characterStats.playerMaxStamina;
                break;
            case "CharacterEnemy":
                characterStats = GetComponent<CharacterStats>();
                enemyStatData = new EnemyStatData("{\"playerHP\":150.0,\"playerMaxHp\":175.0,\"playerExp\":1.0,\"playerCoins\":50.0,\"requiredExp\":1000.0,\"playerLvl\":1.0,\"playerMaxStamina\":20.0}");
                characterStats.LoadDataEnemy(enemyStatData.jsonStat);
                characterStats.InitializeCharacterStats();
                hp = characterStats.playerHP;
                maxHP = characterStats.playerHP;
                staminaMax = characterStats.playerMaxStamina;
                stamina = characterStats.playerMaxStamina;
                break;
        }
        menuFightIconData = menuFightIcon.GetComponent<FightMenuBuffAndDebuffs>();

    }



    void changeBar(Image image, TextMeshProUGUI text, float currentValue, float maxValue)
    {

        image.fillAmount = currentValue / maxValue;
        text.text = Math.Round(currentValue,2) + "/" + Math.Round(maxValue,2);

    }

    public void ShowArmor(Image image, TextMeshProUGUI text, float currentValue, float maxValue)
    {
        image.fillAmount = currentValue / maxValue;

        if (armor > 0)
        {
            image.enabled = true;
            text.enabled = true;
            text.text = currentValue + "/" + maxValue;
        }
        else
        {
            image.enabled = false;
            text.enabled = false;
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
        changeBar(hpBarImage, textBarHP, hp, maxHP);
        changeBar(staminaBarImage, textBarStamina, stamina, staminaMax);
        ShowArmor(armorBarImage, textBarArmor, armor, armorMax);

        //if (gameObject.name == "Character")
        //{
        //    characterStats.playerHP = Convert.ToInt32(hp);
        //    //characterStats.hpText.text = characterStats.playerHP.ToString();
        //}

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
