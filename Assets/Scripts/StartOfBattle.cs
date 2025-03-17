using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class StartOfBattle : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;

    private float timerStart = 0.5f;
    private bool timer_locked_outStart = true;

    private GameObject placeForLogDescription;

    //debuffs
    public GameObject LogPoisonStackCharacter, LogPoisonStackEnemy;
    public GameObject LogFrostStackCharacter, LogFrostStackEnemy;
    public GameObject LogBlindStackCharacter, LogBlindStackEnemy;
    public GameObject LogBleedStackCharacter, LogBleedStackEnemy;

    //buffs
    public GameObject LogBaseCritStackCharacter, LogBaseCritStackEnemy;
    public GameObject LogFireStackCharacter, LogFireStackEnemy;
    public GameObject LogChanceCritStackCharacter, LogChanceCritStackEnemy;
    public GameObject LogEvasionStackCharacter, LogEvasionStackEnemy;
    public GameObject LogManaStackCharacter, LogManaStackEnemy;
    public GameObject LogPowerStackCharacter, LogPowerStackEnemy;
    public GameObject LogRegenHpStackCharacter, LogRegenHpStackEnemy;
    public GameObject LogResistanceStackCharacter, LogResistanceStackEnemy;
    public GameObject LogVampireStackCharacter, LogVampireStackEnemy;

    //attack, armor, timer, stamina
    public GameObject LogAttackStackCharacter, LogAttackStackEnemy;
    public GameObject LogArmorStackCharacter, LogArmorStackEnemy;
    public GameObject LogTimerStackCharacter, LogTimerStackEnemy;
    public GameObject LogStaminaStackCharacter, LogStaminaStackEnemy;

    [HideInInspector]public float addStamina = 0;

    private void Awake()
    {
        placeForLogDescription = GameObject.FindGameObjectWithTag("BattleLogContent");
    }
    
    private void CreateLogMessage(GameObject log, string message)
    {
        var obj = Instantiate(log, placeForLogDescription.GetComponent<RectTransform>().transform);
        obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
        //obj.GetComponent<LogMessage>().nestedObject = gameObject;
    }

    private void StartBattle()
    {
        PlayerBackpackBattle playerBattle = player.GetComponent<PlayerBackpackBattle>();
        PlayerBackpackBattle enemyBattle = enemy.GetComponent<PlayerBackpackBattle>();

        if (PlayerPrefs.HasKey("BottleOfWinePoison"))
        {
            playerBattle.menuFightIconData.AddDebuff(PlayerPrefs.GetInt("BottleOfWinePoison"), "IconPoison");
            CreateLogMessage(LogPoisonStackCharacter, "Bottle Of Wine give " + PlayerPrefs.GetInt("BottleOfWinePoison"));
            PlayerPrefs.DeleteKey("BottleOfWinePoison");
        }
        if (PlayerPrefs.HasKey("BottleOfWineCritChance"))
        {
            playerBattle.menuFightIconData.AddBuff(PlayerPrefs.GetInt("BottleOfWineCritChance"), "IconChanceCrit");
            CreateLogMessage(LogChanceCritStackCharacter, "Bottle Of Wine give " + PlayerPrefs.GetInt("BottleOfWineCritChance"));
            PlayerPrefs.DeleteKey("BottleOfWineCritChance");
        }



        if (PlayerPrefs.HasKey("BowOfSoupFire"))
        {
            playerBattle.menuFightIconData.AddBuff(PlayerPrefs.GetInt("BowOfSoupFire"), "IconBurn");
            CreateLogMessage(LogFireStackCharacter, "Bow Of SoupFire give " + PlayerPrefs.GetInt("BowOfSoupFire"));
            PlayerPrefs.DeleteKey("BowOfSoupFire");
        }




        if (PlayerPrefs.HasKey("CupOfTeaRegeneration"))
        {
            playerBattle.menuFightIconData.AddBuff(PlayerPrefs.GetInt("CupOfTeaRegeneration"), "IconRegenerate");
            CreateLogMessage(LogRegenHpStackCharacter, "Cup Of Tea give " + PlayerPrefs.GetInt("CupOfTeaRegeneration"));
            PlayerPrefs.DeleteKey("CupOfTeaRegeneration");
        }



        if (PlayerPrefs.HasKey("FriedMeatStamina"))
        {
            float staminaFriedMeat = PlayerPrefs.GetFloat("FriedMeatStamina");
            addStamina += staminaFriedMeat;
            PlayerPrefs.DeleteKey("FriedMeatStamina");
            CreateLogMessage(LogStaminaStackCharacter, "Fried Meat increased by " + staminaFriedMeat);
        }



        if (PlayerPrefs.HasKey("FriedPotatoStamina"))
        {
            float staminaFriedPotato = PlayerPrefs.GetFloat("FriedPotatoStamina");
            addStamina += staminaFriedPotato;
            PlayerPrefs.DeleteKey("FriedPotatoStamina");
            CreateLogMessage(LogStaminaStackCharacter, "Fried Potato increased by " + staminaFriedPotato);
        }



        if (PlayerPrefs.HasKey("JamAvoidance"))
        {
            playerBattle.menuFightIconData.AddBuff(PlayerPrefs.GetInt("JamAvoidance"), "IconEvasion");
            CreateLogMessage(LogEvasionStackCharacter, "Jam give " + PlayerPrefs.GetInt("JamAvoidance"));
            PlayerPrefs.DeleteKey("JamAvoidance");
        }



        if (PlayerPrefs.HasKey("MushroomPiePoison"))
        {
            enemyBattle.menuFightIconData.AddDebuff(PlayerPrefs.GetInt("MushroomPiePoison"), "IconPoison");
            CreateLogMessage(LogPoisonStackCharacter, "Mushroom Pie inflict " + PlayerPrefs.GetInt("MushroomPiePoison"));
            PlayerPrefs.DeleteKey("MushroomPiePoison");
        }



        if (PlayerPrefs.HasKey("StewStamina"))
        {
            float staminaFriedPotato = PlayerPrefs.GetFloat("StewStamina");
            addStamina += staminaFriedPotato;
            PlayerPrefs.DeleteKey("StewStamina");
            CreateLogMessage(LogStaminaStackCharacter, "Stew increased by " + staminaFriedPotato);
        }


        playerBattle.staminaMax += addStamina;
        playerBattle.stamina = playerBattle.staminaMax;
    }

    private void CoolDownStart()
    {
        if (timer_locked_outStart)
        {
            timerStart -= Time.deltaTime;

            if (timerStart <= 0)
            {
                timer_locked_outStart = false;
                StartBattle();
            }
        }
    }

    private void Update()
    {
        CoolDownStart();
    }
}
