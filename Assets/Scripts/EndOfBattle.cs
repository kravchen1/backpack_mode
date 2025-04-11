
using System.Collections;

using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

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



    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip loseClip;

    [SerializeField] private AudioSource backgroundBattleMusic;

    private bool awardsReceived = false;
    private bool win = true;

    public float fillExpSpeed = 0.00001f;// Скорость заполнения полосы - чем меньше, тем быстрее

    private int hpAddCountForLvl = 35;


    private void Awake()
    {
    }
    void Start()
    {
        playerBackpackBattle = player.GetComponent<PlayerBackpackBattle>();
        enemyBackpackBattle = enemy.GetComponent<PlayerBackpackBattle>();
    }


    private int winExp = 0;
    public void EndScene()
    {
        if (enemyBackpackBattle.hp <= 0 && !awardsReceived) //win
        {
            gameObject.AddComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
            gameObject.GetComponent<AudioSource>().PlayOneShot(winClip);
            StopFight();
            PlayerPrefs.SetInt("WinLose", 1);
            if (PlayerPrefs.GetInt("VampireAmulet") == 1)
            {
                PlayerPrefs.SetInt("VampireAmulet", 0);
                giveItem("AngryFluff");
            }
            var enemyName = PlayerPrefs.GetString("enemyName");
            if (enemyName == "Dragon(Clone)")
            {
                if (QuestComplete(6))
                {
                    NewQuest(7, "NPC_King", 6);
                    //NewQuestId7();
                }

            }
            else if (enemyName == "Podpol")
            {
                if (QuestComplete(8))
                {
                    NewQuest(9, "NPC_King", 8);
                    //NewQuestId9();
                }
                PlayerPrefs.SetInt("isEnemyPodpolDefeat", 1);
            }
            else if (enemyName == "General")
            {
                if (QuestComplete(10))
                {
                    NewQuest(11, "NPC_King", 10);
                    //NewQuestId11();
                }
                PlayerPrefs.SetInt("isEnemyGeneralDefeat",1);
            }
            else
            {
                if (!QuestCompleteProgress(4))
                {
                    NewQuest(5, "NPC_King", 4);
                    //NewQuestId5();
                }
            }

            DieEnemy();

            if (playerBackpackBattle.characterStats.playerLvl < 15)
            {
                int winExp = PlayerPrefs.GetInt("enemyLvl") * 100;
                winExp += Random.Range(0, 10);
                this.winExp = winExp;
                WinExp(winExp);
            }

            int winGold = PlayerPrefs.GetInt("enemyLvl") * 10;
            winGold += Random.Range(0, 10);
            WinGold(winGold);

            PlayerPrefs.DeleteKey("isEnemyAlive");
        }
        else if (playerBackpackBattle.hp <= 0 && !awardsReceived) //lose
        {
            gameObject.AddComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
            gameObject.GetComponent<AudioSource>().PlayOneShot(loseClip);
            StopFight();
            Lose();
            PlayerPrefs.DeleteKey("isEnemyAlive");
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
        StartCoroutine(AddExperienceCoroutineUI(amount));
    }

    private int countLvlUp = 0, showCountLvlUp = 0;

    



    private QuestManager qm;
    public bool QuestCompleteProgress(int questID)
    {
        qm = new QuestManager();

        if (File.Exists(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json")))
        {
            qm.questData = new QuestData();
            qm.questData.questData = new QDataList();
            qm.questData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json"));
            qm.AddCurrentProgressQuestWithoutUI(questID);
        }
        return qm.CheckQuestComplete(questID);
    }

    public bool QuestComplete(int questID)
    {
        qm = new QuestManager();

        if (File.Exists(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json")))
        {
            qm.questData = new QuestData();
            qm.questData.questData = new QDataList();
            qm.questData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json"));
            return qm.CompleteQuest(questID, false);
        }
        else
            return false;
    }

    public void NewQuest(int idQuest, string NPCName, int ppDialogueID)
    {
        string settingLanguage = "en";
        settingLanguage = PlayerPrefs.GetString("LanguageSettings");

        string questName = QuestManagerJSON.Instance.GetNameQuest(settingLanguage, idQuest);
        string questText = QuestManagerJSON.Instance.GetTextQuest(settingLanguage, idQuest);
        int questProgress = QuestManagerJSON.Instance.GetProgressQuest(settingLanguage, idQuest);

        if (qm == null)
        {
            qm = new QuestManager();

            if (File.Exists(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json")))
            {
                qm.questData = new QuestData();
                qm.questData.questData = new QDataList();
                qm.questData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json"));

                

                var questDat = qm.questData.questData.quests.Where(e => e.id == idQuest).ToList();
                if (questDat.Count == 0)
                {
                    Quest quest = new Quest(questName, questText, questProgress, idQuest);

                    qm.questData.questData.quests.Add(quest);
                    qm.questData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json"));
                    PlayerPrefs.SetInt(NPCName, ppDialogueID);
                }
            }
        }
        else
        {
            qm.questData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json"));
            var questDat = qm.questData.questData.quests.Where(e => e.id == idQuest).ToList();
            if (questDat.Count == 0)
            {
                Quest quest = new Quest(questName, questText, questProgress, idQuest);
                qm.questData.questData.quests.Add(quest);
                qm.questData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json"));
                PlayerPrefs.SetInt(NPCName, ppDialogueID);
            }
        }
    }




    public void DieEnemy()
    {
        if (PlayerPrefs.HasKey("isEnemyAlive"))
            PlayerPrefs.SetInt("isEnemyAlive", 0);
        else
        {
            BattlesSpawnerData battlesSpawnerData = new BattlesSpawnerData();

            battlesSpawnerData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "battlesIn1.json"));

            foreach (var battleData in battlesSpawnerData.battlesSpawnerDataClass.battleData.Where(e => e.id == PlayerPrefs.GetInt("enemyIdSpawner")).ToList())
            {
                battleData.die = true;
            }
            battlesSpawnerData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "battlesIn1.json"));
        }

        

    }

    void StopFight()
    {
        Time.timeScale = 0f;
        timeSpeed.value = 0f;
        timeSpeed.interactable = false;

        endOfBattleCanvas.SetActive(true);
        animations.SetActive(false);
        backgroundBattleMusic.Stop();


        awardsReceived = true;
        playerBackpackBattle.characterStats.playerHP = playerBackpackBattle.hp;

    }

    void Lose()
    {
        winMenu.SetActive(false);
        loseMenu.SetActive(true);
        PlayerPrefs.SetInt("NeedSpawnEnemys", 1);
        win = false;
        PlayerPrefs.SetInt("WinLose", 2);

        DoorData doorData = new DoorData();
        doorData.InitializeFilePath();
        doorData.DeleteData();
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
            if (playerBackpackBattle.characterStats.playerLvl < 15)
            {
                AddExperience(winExp);
            }
            if(playerBackpackBattle.characterStats.playerHP < 1)
            {
                playerBackpackBattle.characterStats.playerHP = 1;
            }
            playerBackpackBattle.characterStats.SaveData();
            //SceneManager.LoadScene(PlayerPrefs.GetString("currentLocation"));
            
            SceneLoader.Instance.LoadScene(PlayerPrefs.GetString("currentLocation"));
        }
        else
        {
            playerBackpackBattle.characterStats.playerHP = 1;
            if(playerBackpackBattle.characterStats.playerCoins < 50)
            {
                playerBackpackBattle.characterStats.playerCoins = 50;
            }

            playerBackpackBattle.characterStats.SaveData();

            PlayerPrefs.DeleteKey("PostionMapX");
            //SceneManager.LoadScene("GenerateMapFortress1");
            SceneLoader.Instance.LoadScene("GenerateMapFortress1");
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

        backPackAndStorageData.storageData.itemData.items.Add(new Data(itemName, new Vector3(0, 0, -2)));
        backPackAndStorageData.storageData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json"));
    }


    public void LevelUpAddUI(int countLvlUp)
    {
        var hpAddCount = (countLvlUp * hpAddCountForLvl);
        winHPAddCount.text = "+" + hpAddCount.ToString();

        var staminaAddCount = (countLvlUp * 0.2f);
        winStaminaAddCount.text = "+" + staminaAddCount.ToString();

        winHPAddAwards.gameObject.SetActive(true);
        winStaminaAddAwards.gameObject.SetActive(true);
    }

    private void LevelUpUI()
    {
        showPlayerLvl++;
        winLevelText.text = showPlayerLvl.ToString();
        showPlayerExp = 0;
        showRequiredExp += 250 * showPlayerLvl; // Увеличиваем необходимый опыт для следующего уровня
        winExpBar.fillAmount = 0;


        showCountLvlUp++;
        LevelUpAddUI(showCountLvlUp);
    }


    private int showPlayerExp = -1, showRequiredExp = -1, showPlayerLvl = -1;
    private IEnumerator AddExperienceCoroutineUI(int amount)
    {
        if (showPlayerExp == -1)
        {
            showPlayerExp = playerBackpackBattle.characterStats.playerExp;
            showRequiredExp = playerBackpackBattle.characterStats.requiredExp;
            showPlayerLvl = playerBackpackBattle.characterStats.playerLvl;
        }

        int targetExperience = showPlayerExp + amount;
        int startStartExp = showPlayerExp;

        while (startStartExp < targetExperience)
        {
            startStartExp++;
            showPlayerExp++;
            winExpBarText.text = showPlayerExp + "/" + showRequiredExp;
            winExpBar.fillAmount = (float)showPlayerExp / showRequiredExp;

            if (showPlayerExp >= showRequiredExp)
            {
                LevelUpUI();
            }

            yield return new WaitForSecondsRealtime(fillExpSpeed);
        }
    }

    public void LevelUpAdd(int countLvlUp)
    {
        var hpAddCount = (countLvlUp * hpAddCountForLvl);
        playerBackpackBattle.characterStats.playerMaxHp += hpAddCount;
        playerBackpackBattle.characterStats.playerHP = playerBackpackBattle.characterStats.playerMaxHp;

        var staminaAddCount = (countLvlUp * 0.2f);
        playerBackpackBattle.characterStats.playerMaxStamina += staminaAddCount;
    }

    private void LevelUp()
    {
        playerBackpackBattle.characterStats.playerLvl++;
        playerBackpackBattle.characterStats.playerExp = 0;
        playerBackpackBattle.characterStats.requiredExp += 250 * playerBackpackBattle.characterStats.playerLvl; // Увеличиваем необходимый опыт для следующего уровня


        countLvlUp++;
        LevelUpAdd(countLvlUp);
    }

    private void AddExperience(int amount)
    {
        int targetExperience = playerBackpackBattle.characterStats.playerExp + amount;
        int startStartExp = playerBackpackBattle.characterStats.playerExp;
        while (startStartExp < targetExperience)
        {
            startStartExp++;
            playerBackpackBattle.characterStats.playerExp++;

            if (playerBackpackBattle.characterStats.playerExp >= playerBackpackBattle.characterStats.requiredExp)
            {
                LevelUp();
            }
        }
    }
}
