//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using TMPro;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.TextCore.Text;
//using UnityEngine.UI;
//using static UnityEngine.GraphicsBuffer;

//public class BattleManager : MonoBehaviour
//{
//    public static BattleManager Instance;

//   // [Header("Battle Settings")]
    
//    //public float autoAttackInterval = 2f;

//    [Header("Escape System")]
//    public float escapeTime = 10f;
//    public bool canEscape = false;
//    public TextMeshProUGUI escapeTimerText;
//    public GameObject escapeButton;

//    [Header("UI References")]
//    //public Transform playerTeamPanel;
//    //public Transform enemyTeamPanel;
//    //public Transform friendsPanel;
//    public List<GameObject> playerTeamIcons;
//    public List<GameObject> enemyTeamIcons;
//    public GameObject playerIcon;

//    public List<CellsData> playerTeamBackpacks;
//    public List<CellsData> enemyTeamBackpacks;
//    public CellsData playerBackpack;
//    //public GameObject characterIconPrefab;
//    public GameObject battleUICanvas;


//    private List<NPCDataManager> playerTeam = new List<NPCDataManager>();
//    private List<NPCDataManager> enemyTeam = new List<NPCDataManager>();
//    private NPCDataManager selectedTarget;
//    public bool isBattleActive = false;
//    private float currentEscapeTimer;
//    private Coroutine escapeTimerCoroutine;

//    public ButtonsController buttonsController;
//    public GameObject canvasShop;

//    const int maxPlayerTeamSize = 4;
//    const int maxEnemyTeamSize = 5;

//    public System.Action OnBattleStart;
//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    // Основной метод начала боя
//    public void StartBattle(List<NPCDataManager> players, List<NPCDataManager> enemies)
//    {
//        if (isBattleActive)
//        {
//            Debug.LogWarning("Бой уже активен!");
//            return;
//        }
//        isBattleActive = true;
//        DragManager.Instance.isDragActive = false;

//        buttonsController.CloseInventory();
//        canvasShop.SetActive(false);

//        playerTeam = players.Take(maxPlayerTeamSize).ToList();
//        enemyTeam = enemies.Take(maxEnemyTeamSize).ToList(); 

        

//        CreateBattleUI();
//        StartBattleLogic();

//        OnBattleStart?.Invoke();
//    }

//    private void CreateBattleUI()
//    {
//        // Показываем UI боя
//        if (battleUICanvas != null)
//            battleUICanvas.SetActive(true);

//        foreach (var payerTeamIcon in playerTeamIcons)
//        {
//            payerTeamIcon.SetActive(false);
//        }

//        foreach (var enemyTeamIcon in enemyTeamIcons)
//        {
//            enemyTeamIcon.SetActive(false);
//        }




//        // Создание иконок
//        playerIcon.GetComponent<PlayerCharacterIcon>().Initialize();
//        foreach (var character in playerTeam)
//        {
//            CreateCharacterIconAndBackpacks(character, false);
//        }

//        foreach (var character in enemyTeam)
//        {
//            CreateCharacterIconAndBackpacks(character, true);
//        }
//    }


//    private void CreateCharacterIconAndBackpacks(NPCDataManager character, bool isEnemy)
//    {
//        if (isEnemy)
//        {
//            for(int i = 0; i < enemyTeamIcons.Count; i++)
//            {
//                if (!enemyTeamIcons[i].activeSelf)
//                {
//                    enemyTeamIcons[i].GetComponent<CharacterIcon>().Initialize(character, isEnemy);


//                    enemyTeamBackpacks[i].settingsKey = character.GetComponent<NPC>().Config.settingKey;
//                    enemyTeamBackpacks[i].LoadData();
//                    character.cellsFight = enemyTeamBackpacks[i];
//                    return;
//                }
//            }
//        }
//        else
//        {
//            for (int i = 0; i < playerTeamIcons.Count; i++)
//            {
//                if (!playerTeamIcons[i].activeSelf)
//                {
//                    playerTeamIcons[i].GetComponent<CharacterIcon>().Initialize(character, isEnemy);

//                    playerTeamBackpacks[i].settingsKey = character.GetComponent<NPC>().Config.settingKey;
//                    playerTeamBackpacks[i].LoadData();
//                    character.cellsFight = playerTeamBackpacks[i];
//                    return;
//                }
//            }
           
//        }
//    }

//    private void StartBattleLogic()
//    {
//        selectedTarget = enemyTeam.FirstOrDefault();

//        //StartCoroutine(AutoAttackRoutinePlayer(PlayerDataManager.Instance));
//        // Запуск автоматических атак
//        for (int i = 0; i < playerTeam.Count; i++)
//        {
//            StartCoroutine(AutoAttackRoutine(playerTeam[i], i, false));
//        }

//        for (int i = 0; i < enemyTeam.Count; i++)
//        {
//            StartCoroutine(AutoAttackRoutine(enemyTeam[i], i, true));
//        }

//        StartCoroutine(AutoAttackRoutinePlayer(PlayerDataManager.Instance));

//        // Запуск систем
//        StartEscapeTimer();
//        //StartFriendSystem();

//        Debug.Log($"Battle started! {playerTeam.Count} vs {enemyTeam.Count}");
//    }

//    // СИСТЕМА ВЫБОРА ЦЕЛИ
//    public void OnTargetSelected(NPCDataManager target)
//    {
//        if (!isBattleActive || target == null || !enemyTeam.Contains(target))
//            return;

//        selectedTarget = target;
//    }


//    // СИСТЕМА АТАК
//    private System.Collections.IEnumerator AutoAttackRoutine(NPCDataManager attacker, int indexAttacker, bool isEnemy)
//    {
//        while (isBattleActive && attacker.IsAlive)
//        {
//            yield return null;
//            List<ItemActionController> itemActions = new List<ItemActionController>();
//            if(isEnemy)
//            {
//                foreach (var itemAction in enemyTeamBackpacks[indexAttacker].GetComponentsInChildren<ActivationItemActionController>())
//                {
//                    int r = Random.Range(0, playerTeam.Count + 1);
//                    if (r != 0)
//                    {
//                        while (!playerTeam[r - 1].IsAlive)
//                        {
//                            r -= 1;
//                            if (r == 0) break;
//                        }
//                    }

//                    if (r == 0)
//                    {
//                        itemAction.UpdateForBattle(attacker, PlayerDataManager.Instance);
//                    }
//                    else
//                    {
//                        itemAction.UpdateForBattle(attacker, playerTeam[r - 1]);
//                    }
                   
//                }
//            }
//            else
//            {
//                foreach (var itemAction in playerTeamBackpacks[indexAttacker].GetComponentsInChildren<ActivationItemActionController>())
//                {
//                    if (selectedTarget != null && selectedTarget.IsAlive)
//                    {
//                        itemAction.UpdateForBattle(attacker, selectedTarget);
//                    }
//                    else
//                    {
//                        selectedTarget = enemyTeam.FirstOrDefault(e => e.IsAlive);
//                        if (selectedTarget == null)
//                        {
//                            EndBattle(true);
//                            yield break;
//                        }
//                    }
//                }
//            }
//            CheckBattleEnd();
//        }
//    }

//    private System.Collections.IEnumerator AutoAttackRoutinePlayer(PlayerDataManager attacker)
//    {
//        while (isBattleActive && attacker.IsAlive)
//        {
//            yield return null;
//            List<ItemActionController> itemActions = new List<ItemActionController>();
//            foreach (var itemAction in playerBackpack.GetComponentsInChildren<ActivationItemActionController>())
//            {
//                if (selectedTarget != null && selectedTarget.IsAlive)
//                {
//                    itemAction.UpdateForBattle(attacker, selectedTarget);
//                }
//                else
//                {
//                    selectedTarget = enemyTeam.FirstOrDefault(e => e.IsAlive);
//                    if (selectedTarget == null)
//                    {
//                        EndBattle(true);
//                        yield break;
//                    }
//                }
//            }
//            CheckBattleEnd();
//        }
//    }



//    // СИСТЕМА ПОБЕГА
//    private void StartEscapeTimer()
//    {
//        canEscape = false;
//        currentEscapeTimer = escapeTime;

//        if (escapeTimerCoroutine != null)
//            StopCoroutine(escapeTimerCoroutine);

//        escapeTimerCoroutine = StartCoroutine(EscapeTimerRoutine());
//    }

//    private IEnumerator EscapeTimerRoutine()
//    {
//        while (currentEscapeTimer > 0 && isBattleActive)
//        {
//            currentEscapeTimer -= Time.deltaTime;
//            UpdateEscapeTimerUI();
//            yield return null;
//        }

//        if (isBattleActive && currentEscapeTimer <= 0)
//            EnableEscape();
//    }

//    private void UpdateEscapeTimerUI()
//    {
//        if (escapeTimerText != null)
//        {
//            escapeTimerText.text = currentEscapeTimer > 0 ?
//                $"Бежать через: {Mathf.Ceil(currentEscapeTimer)}с" : "Можно бежать!";
//            escapeTimerText.color = currentEscapeTimer > 0 ? Color.yellow : Color.green;
//        }
//    }

//    private void EnableEscape()
//    {
//        canEscape = true;
//        Debug.Log("Escape is now available!");

//        if (escapeButton != null)
//            escapeButton.SetActive(canEscape);
//    }



//    public void Escape()
//    {
//        Debug.Log("Escape successful!");
//        EndBattleWithEscape(true);
//    }

//    // ЗАВЕРШЕНИЕ БОЯ
//    private void CheckBattleEnd()
//    {
//        if (enemyTeam.All(e => !e.IsAlive))
//            EndBattle(true);
//        else if (!PlayerDataManager.Instance.IsAlive)
//            EndBattle(false);
//    }

//    private void EndBattle(bool playerWon)
//    {
//        isBattleActive = false;
//        DragManager.Instance.isDragActive = true;
//        canEscape = false;
//        StopAllCoroutines();
//        SaveBackpacks();

//        // Скрываем UI
//        if (battleUICanvas != null)
//        {
//            escapeButton.SetActive(false);
//            battleUICanvas.SetActive(false);
//        }

//        if (playerWon)
//        {
//            Debug.Log("Battle won! Victory!");
//            DistributeRewards();
//        }
//        else
//        {
//            Debug.Log("Battle lost! Defeat!");
//        }

//        OnBattleEnded?.Invoke(playerWon);
//        Time.timeScale = 1f;
//    }

//    private void EndBattleWithEscape(bool escaped)
//    {
//        isBattleActive = false;
//        DragManager.Instance.isDragActive = true;
//        canEscape = false;
//        StopAllCoroutines();
//        SaveBackpacks();

//        if (battleUICanvas != null)
//            battleUICanvas.SetActive(false);
         
//        OnBattleEnded?.Invoke(false);
//        OnBattleEscaped?.Invoke(escaped);


//        Time.timeScale = 1f;
//        foreach (var enemy in enemyTeam)
//        {
//            StartCoroutine(EndFight(10f, enemy));
//        }
//        foreach (var friend in playerTeam)
//        {
//            StartCoroutine(EndFight(0.5f, friend));
//        }
//    }

//    protected System.Collections.IEnumerator EndFight(float delay, NPCDataManager target)
//    {
//        yield return new WaitForSeconds(delay);
//        target.GetComponent<NPC>().InFightNow = false;
//        yield break;
//    }

//    private void SaveBackpacks()
//    {
//        playerBackpack.SaveData();
//        foreach (var backPack in enemyTeamBackpacks)
//        {
//            backPack.SaveData();
//        }
//        foreach (var backPack in playerTeamBackpacks)
//        {
//            backPack.SaveData();
//        }
//    }

//    // НАГРАДЫ
//    private void DistributeRewards()
//    {
//        int expReward = enemyTeam.Count * 50;
//        float moneyReward = enemyTeam.Count * 25f;

//        if (PlayerDataManager.Instance != null)
//        {
//            PlayerDataManager.Instance.AddExperience(expReward);
//            PlayerDataManager.Instance.AddMoney(moneyReward);
//        }
//    }



//    // API МЕТОДЫ
//    public void AddEnemyToBattle(NPCDataManager newEnemy)
//    {
//        if (!isBattleActive || enemyTeam.Count >= maxEnemyTeamSize) return;

//        enemyTeam.Add(newEnemy);
//        CreateCharacterIconAndBackpacks(newEnemy, true);
//        StartCoroutine(AutoAttackRoutine(newEnemy, enemyTeam.Count-1, true));
//    }

//    public void AddFriendToBattle(NPCDataManager friend)
//    {
//        if (!isBattleActive) return;

//        playerTeam.Add(friend);
//        CreateCharacterIconAndBackpacks(friend, false);
//        StartCoroutine(AutoAttackRoutine(friend, playerTeam.Count-1, true));
//    }

//    public List<NPCDataManager> GetPlayerTeam() => new List<NPCDataManager>(playerTeam);
//    public List<NPCDataManager> GetEnemyTeam() => new List<NPCDataManager>(enemyTeam);
//    public NPCDataManager GetSelectedTarget() => selectedTarget;
//    public float GetEscapeTimer() => currentEscapeTimer;
//    public bool CanEscape() => canEscape;

//    // СОБЫТИЯ
//    public System.Action<bool> OnBattleEnded;
//    public System.Action<bool> OnBattleEscaped;
//}