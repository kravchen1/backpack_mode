using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class EndOfBattle : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;
    //[SerializeField] private GameObject okButton;
    //[SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject endOfBattleCanvas;
    [SerializeField] private GameObject animations;
    [SerializeField] private Slider timeSpeed;
    private PlayerBackpackBattle playerBackpackBattle;
    private PlayerBackpackBattle enemyBackpackBattle;



    
    [SerializeField] private TextMeshProUGUI winLevelText;
    [SerializeField] private GameObject winMenuAwards;
    [SerializeField] private Image winExpBar;
    [SerializeField] private TextMeshProUGUI winExpBarText;
    [SerializeField] private TextMeshProUGUI winExpCount;
    [SerializeField] private TextMeshProUGUI winGoldCount;
    [SerializeField] private GameObject winHPAddAwards;
    [SerializeField] private TextMeshProUGUI winHPAddCount;
    [SerializeField] private GameObject winStaminaAddAwards;
    [SerializeField] private TextMeshProUGUI winStaminaAddCount;


    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject loseMenu;



    private bool awardsReceived = false;
    private bool win = true;

    public float fillExpSpeed = 0.0005f;// Скорость заполнения полосы - чем меньше, тем быстрее

    private void Awake()
    {
    }
    void Start()
    {
        playerBackpackBattle = player.GetComponent<PlayerBackpackBattle>();
        enemyBackpackBattle = enemy.GetComponent<PlayerBackpackBattle>();
    }

    public void EndScene()
    {
        if (enemyBackpackBattle.hp <= 0 && !awardsReceived) //win
        {
            StopFight();
            if(PlayerPrefs.GetInt("VampireAmulet") == 1)
            {
                PlayerPrefs.SetInt("VampireAmulet", 0);

                giveItem("AngryFluff");
            }
            var enemyName = PlayerPrefs.GetString("enemyName");
            if (enemyName == "Fanatik(Clone)"
                || enemyName == "Fallen Knight(Clone)"
                || enemyName == "Goblin(Clone)"
                )
            {
                QuestComplete(4);
                DieEnemy();

                int winExp = PlayerPrefs.GetInt("enemyLvl") * 100;
                winExp += Random.Range(0, 10);
                WinExp(winExp);

                int winGold = PlayerPrefs.GetInt("enemyLvl") * 10;
                winGold += Random.Range(0, 10);
                WinGold(winGold);
            }
        }
        else if (playerBackpackBattle.hp <= 0 && !awardsReceived) //lose
        {
            StopFight();
            Lose();
        }
    }

    public void WinGold(int winGold)
    {
        winGoldCount.text = "+" + winGold;
        playerBackpackBattle.characterStats.playerCoins += winGold;
    }

    public void WinExp(int amount)
    {
        winExpCount.text = amount.ToString();
        winLevelText.text = playerBackpackBattle.characterStats.playerLvl.ToString();
        StartCoroutine(AddExperienceCoroutine(amount));
    }

    private int countLvlUp = 0;
    public void LevelUpAdd(int countLvlUp)
    {
        var hpAddCount = (countLvlUp * 10);
        winHPAddCount.text = "+" + hpAddCount.ToString();
        playerBackpackBattle.characterStats.playerMaxHp += hpAddCount;
        playerBackpackBattle.characterStats.playerHP = playerBackpackBattle.characterStats.playerMaxHp;

        var staminaAddCount = (countLvlUp * 0.2f);
        winStaminaAddCount.text = "+" + staminaAddCount.ToString();
        playerBackpackBattle.characterStats.playerMaxStamina += staminaAddCount;

        winHPAddAwards.gameObject.SetActive(true);
        winStaminaAddAwards.gameObject.SetActive(true);
    }

    private void LevelUp()
    {
        playerBackpackBattle.characterStats.playerLvl++;
        winLevelText.text = playerBackpackBattle.characterStats.playerLvl.ToString();
        playerBackpackBattle.characterStats.playerExp = 0;
        playerBackpackBattle.characterStats.requiredExp += 500 * playerBackpackBattle.characterStats.playerLvl; // Увеличиваем необходимый опыт для следующего уровня
        winExpBar.fillAmount = 0;


        countLvlUp++;
        LevelUpAdd(countLvlUp);
    }


    public void QuestComplete(int questID)
    {
        QuestManager qm = new QuestManager();

        if (File.Exists(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json")))
        {
            qm.questData = new QuestData();
            qm.questData.questData = new QDataList();
            qm.questData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json"));
            qm.AddCurrentProgressQuestWithoutUI(questID);
        }
    }

    public void DieEnemy()
    {
        BattlesSpawnerData battlesSpawnerData = new BattlesSpawnerData();

        battlesSpawnerData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "battlesIn1.json"));

        foreach(var battleData in battlesSpawnerData.battlesSpawnerDataClass.battleData.Where(e => e.id == PlayerPrefs.GetInt("enemyIdSpawner")).ToList())
        {
            battleData.die = true;
        }
        battlesSpawnerData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "battlesIn1.json"));

    }

    void StopFight()
    {
        Time.timeScale = 0f;
        timeSpeed.value = 0f;
        timeSpeed.interactable = false;
        float addStaminaStartBattle = GetComponent<StartOfBattle>().addStamina;
        playerBackpackBattle.characterStats.playerMaxStamina -= addStaminaStartBattle;
        endOfBattleCanvas.SetActive(true);
        animations.SetActive(false);

        awardsReceived = true;
        playerBackpackBattle.characterStats.playerHP = playerBackpackBattle.hp;
    }

    void Lose()
    {
        winMenu.SetActive(false);
        loseMenu.SetActive(true);
        PlayerPrefs.SetInt("NeedSpawnEnemys", 1);
        win = false;
    }
    // Update is called once per frame
    void Update()
    {
        EndScene();
    }



    //buttons
    public void buttonEndFightOk()
    {
        if(win)
        {
            //todo
            playerBackpackBattle.characterStats.SaveData();
            SceneManager.LoadScene(PlayerPrefs.GetString("currentLocation"));
        }
        else
        {
            playerBackpackBattle.characterStats.playerHP = 1;
            playerBackpackBattle.characterStats.SaveData();
            SceneManager.LoadScene("GenerateMapFortress1");
        }
    }

    private void giveItem(string itemName)
    {
        BackPackAndStorageData backPackAndStorageData = new BackPackAndStorageData();
        backPackAndStorageData.storageData = new BackpackData();
        backPackAndStorageData.storageData.itemData = new ItemData();
        if (File.Exists(Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json")))
        {
            backPackAndStorageData.storageData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json"));
        }

        backPackAndStorageData.storageData.itemData.items.Add(new Data(itemName, new Vector2(0, 0)));
        backPackAndStorageData.storageData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json"));
    }


    private IEnumerator AddExperienceCoroutine(int amount)
    {
        int targetExperience = playerBackpackBattle.characterStats.playerExp + amount;
        int startStartExp = playerBackpackBattle.characterStats.playerExp;
        while (startStartExp < targetExperience)
        {
            startStartExp++;
            playerBackpackBattle.characterStats.playerExp++;
            winExpBarText.text = playerBackpackBattle.characterStats.playerExp + "/" + playerBackpackBattle.characterStats.requiredExp;
            winExpBar.fillAmount = (float)playerBackpackBattle.characterStats.playerExp / playerBackpackBattle.characterStats.requiredExp;

            if (playerBackpackBattle.characterStats.playerExp >= playerBackpackBattle.characterStats.requiredExp)
            {
                LevelUp();
            }

            yield return new WaitForSecondsRealtime(fillExpSpeed);
        }
    }
}
