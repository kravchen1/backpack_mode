using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

   // [Header("Battle Settings")]
    
    //public float autoAttackInterval = 2f;

    [Header("Escape System")]
    public float escapeTime = 10f;
    public bool canEscape = false;
    public TextMeshProUGUI escapeTimerText;
    public GameObject escapeButton;

    [Header("UI References")]
    //public Transform playerTeamPanel;
    //public Transform enemyTeamPanel;
    //public Transform friendsPanel;
    public List<GameObject> playerTeamIcons;
    public List<GameObject> enemyTeamIcons;
    public GameObject playerIcon;
    //public GameObject characterIconPrefab;
    public GameObject battleUICanvas;

    [Header("Friend System")]
    public bool enableFriendSystem = true;

    private List<NPCDataManager> playerTeam = new List<NPCDataManager>();
    private List<NPCDataManager> enemyTeam = new List<NPCDataManager>();
    private NPCDataManager selectedTarget;
    private bool isBattleActive = false;
    private float currentEscapeTimer;
    private Coroutine escapeTimerCoroutine;

    public ButtonsController buttonsController;
    public GameObject canvasShop;

    const int maxPlayerTeamSize = 4;
    const int maxEnemyTeamSize = 5;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Основной метод начала боя
    public void StartBattle(List<NPCDataManager> players, List<NPCDataManager> enemies)
    {
        if (isBattleActive)
        {
            Debug.LogWarning("Бой уже активен!");
            return;
        }
        //if (buttonsController != null)
        //{
        //    buttonsController = GameObject.Find("ButtonsController").GetComponent<ButtonsController>();
        //}
        //if (canvasShop)
        //{
        //    canvasShop = GameObject.Find("CanvasShop").transform.GetChild(0).gameObject;
        //}

        
        buttonsController.CloseInventory();
        canvasShop.SetActive(false);

        playerTeam = players.Take(maxPlayerTeamSize).ToList();
        enemyTeam = enemies.Take(maxEnemyTeamSize).ToList(); // Ограничиваем врагов

        

        CreateBattleUI();
        StartBattleLogic();
    }

    private void CreateBattleUI()
    {
        // Показываем UI боя
        if (battleUICanvas != null)
            battleUICanvas.SetActive(true);

        foreach (var payerTeamIcon in playerTeamIcons)
        {
            payerTeamIcon.SetActive(false);
        }

        foreach (var enemyTeamIcon in enemyTeamIcons)
        {
            enemyTeamIcon.SetActive(false);
        }




        // Создание иконок
        playerIcon.GetComponent<PlayerCharacterIcon>().Initialize();
        foreach (var character in playerTeam)
        {
            CreateCharacterIcon(character, false);
        }

        foreach (var character in enemyTeam)
        {
            CreateCharacterIcon(character, true);
        }
    }


    private void CreateCharacterIcon(NPCDataManager character, bool isEnemy)
    {
        if (isEnemy)
        {
            foreach (var enemyTeamIcon in enemyTeamIcons)
            {
                if (!enemyTeamIcon.activeSelf)
                {
                    enemyTeamIcon.GetComponent<CharacterIcon>().Initialize(character, isEnemy);

                    //var button = enemyTeamIcon.GetComponent<UnityEngine.UI.Button>();
                    //if (button != null)
                    //{
                    //    button.onClick.AddListener(() => OnTargetSelected(character));
                    //}

                    return;
                }
            }
        }
        else
        {
            foreach (var playerTeamIcon in playerTeamIcons)
            {
                if (!playerTeamIcon.activeSelf)
                {
                    playerTeamIcon.GetComponent<CharacterIcon>().Initialize(character, isEnemy);

                    //var button = playerTeamIcon.GetComponent<UnityEngine.UI.Button>();
                    //if (button != null)
                    //{
                    //    button.onClick.AddListener(() => OnFriendSelected(character));
                    //}

                    return;
                }
            }
           
        }

        //GameObject iconObj = Instantiate(characterIconPrefab, parent);
        //

        //if (icon != null)
        //{
        //    icon.Initialize(character, isEnemy);

        //    if (isEnemy)
        //    {
        //        var button = iconObj.GetComponent<UnityEngine.UI.Button>();
        //        if (button != null)
        //        {
        //            button.onClick.AddListener(() => OnTargetSelected(character));
        //        }
        //    }
        //}
    }

    private void StartBattleLogic()
    {
        isBattleActive = true;
        selectedTarget = enemyTeam.FirstOrDefault();

        StartCoroutine(AutoAttackRoutinePlayer(PlayerDataManager.Instance));
        // Запуск автоматических атак
        foreach (var player in playerTeam)
            StartCoroutine(AutoAttackRoutine(player));

        // Запуск систем
        StartEscapeTimer();
        StartFriendSystem();

        Debug.Log($"Battle started! {playerTeam.Count} vs {enemyTeam.Count}");
    }

    // СИСТЕМА ВЫБОРА ЦЕЛИ
    public void OnTargetSelected(NPCDataManager target)
    {
        if (!isBattleActive || target == null || !enemyTeam.Contains(target))
            return;

        selectedTarget = target;
        UpdateTargetVisuals();
    }

    public void PlayerTarget()
    {
        if (!isBattleActive)
            return;
        Debug.Log("Player target self");
        //selectedTarget = target;
        UpdateTargetVisuals();
    }

    // СИСТЕМА ВЫБОРА Союзника
    public void OnFriendSelected(NPCDataManager target)
    {
        if (!isBattleActive || target == null || !playerTeam.Contains(target))
            return;

        //selectedTarget = target;
        UpdateTargetVisuals();
        Debug.Log("To do обновление инвентаря");
    }

    private void UpdateTargetVisuals()
    {
        //foreach (Transform iconTransform in enemyTeamPanel)
        //{
        //    var icon = iconTransform.GetComponent<CharacterIcon>();
        //    if (icon != null)
        //    {
        //        icon.SetSelected(icon.BattleCharacter == selectedTarget);
        //    }
        //}
    }

    // СИСТЕМА АТАК
    private System.Collections.IEnumerator AutoAttackRoutine(NPCDataManager attacker)
    {
        while (isBattleActive && attacker.IsAlive)
        {
            yield return new WaitForSeconds(0.1f);

            if (selectedTarget != null && selectedTarget.IsAlive)
            {
                Attack(attacker, selectedTarget);
            }
            else
            {
                selectedTarget = enemyTeam.FirstOrDefault(e => e.IsAlive);
                if (selectedTarget == null)
                {
                    EndBattle(true);
                    yield break;
                }
            }
            CheckBattleEnd();
        }
    }

    private System.Collections.IEnumerator AutoAttackRoutinePlayer(PlayerDataManager attacker)
    {
        while (isBattleActive && attacker.IsAlive)
        {
            yield return new WaitForSeconds(0.1f);

            if (selectedTarget != null && selectedTarget.IsAlive)
            {
                Attack(attacker, selectedTarget);
            }
            else
            {
                selectedTarget = enemyTeam.FirstOrDefault(e => e.IsAlive);
                if (selectedTarget == null)
                {
                    EndBattle(true);
                    yield break;
                }
            }
            CheckBattleEnd();
        }
    }

    public void Attack(NPCDataManager attacker, NPCDataManager target)
    {
        if (!isBattleActive || !attacker.IsAlive || !target.IsAlive)
            return;
        Debug.Log(attacker + " атакует " + target);
        //int damage = CalculateDamage(attacker, target);
        //target.TakeDamage(damage);
        UpdateCharacterUI(target);
    }

    public void Attack(PlayerDataManager attacker, NPCDataManager target)
    {
        if (!isBattleActive || !attacker.IsAlive || !target.IsAlive)
            return;
        Debug.Log(attacker + " атакует " + target);
        //int damage = CalculateDamage(attacker, target);
        //target.TakeDamage(damage);
        UpdateCharacterUI(target);
    }

    private int CalculateDamage(NPCDataManager attacker, NPCDataManager target)
    {
        //int baseDamage = 10;

        //// ИСПРАВЛЕНО: правильный доступ к атрибутам через PlayerDataManager
        //int strengthBonus = 0;
        //if (attacker.PlayerDataManager != null && attacker.PlayerDataManager.Attributes != null)
        //{
        //    strengthBonus = Mathf.FloorToInt(attacker.PlayerDataManager.Attributes.strength.GetValue() * 2);
        //}

        //return Mathf.Max(1, baseDamage + strengthBonus);
        return 10;
    }

    // СИСТЕМА ПОБЕГА
    private void StartEscapeTimer()
    {
        canEscape = false;
        currentEscapeTimer = escapeTime;

        if (escapeTimerCoroutine != null)
            StopCoroutine(escapeTimerCoroutine);

        escapeTimerCoroutine = StartCoroutine(EscapeTimerRoutine());
    }

    private IEnumerator EscapeTimerRoutine()
    {
        while (currentEscapeTimer > 0 && isBattleActive)
        {
            currentEscapeTimer -= Time.deltaTime;
            UpdateEscapeTimerUI();
            yield return null;
        }

        if (isBattleActive && currentEscapeTimer <= 0)
            EnableEscape();
    }

    private void UpdateEscapeTimerUI()
    {
        if (escapeTimerText != null)
        {
            escapeTimerText.text = currentEscapeTimer > 0 ?
                $"Бежать через: {Mathf.Ceil(currentEscapeTimer)}с" : "Можно бежать!";
            escapeTimerText.color = currentEscapeTimer > 0 ? Color.yellow : Color.green;
        }
    }

    private void EnableEscape()
    {
        canEscape = true;
        Debug.Log("Escape is now available!");

        if (escapeButton != null)
            escapeButton.SetActive(canEscape);
    }

    public void AttemptEscape()
    {
        if (!canEscape || !isBattleActive) return;

        float escapeChance = CalculateEscapeChance();
        bool escapeSuccessful = Random.Range(0f, 1f) <= escapeChance;

        if (escapeSuccessful) EscapeSuccess();
        else EscapeFailed();
    }

    private float CalculateEscapeChance()
    {
        //float baseChance = 0.7f;

        //// ИСПРАВЛЕНО: правильный доступ к атрибутам
        //if (playerTeam.Count > 0 && playerTeam[0].PlayerDataManager != null && playerTeam[0].PlayerDataManager.Attributes != null)
        //{
        //    var attributes = playerTeam[0].PlayerDataManager.Attributes;
        //    baseChance += attributes.agility.GetValue() * 0.02f;
        //    baseChance += attributes.luck.GetValue() * 0.03f;
        //}

        //float enemyRatioPenalty = (enemyTeam.Count - playerTeam.Count) * 0.1f;
        //return Mathf.Clamp(baseChance - enemyRatioPenalty, 0.1f, 0.95f);
        return 1f;
    }

    private void EscapeSuccess()
    {
        Debug.Log("Escape successful!");
        EndBattleWithEscape(true);
        DistributeEscapeRewards();
    }

    private void EscapeFailed()
    {
        Debug.Log("Escape failed! Enemies get a free attack.");

        foreach (var enemy in enemyTeam.Where(e => e.IsAlive))
        {
            var randomPlayer = playerTeam.Where(p => p.IsAlive).OrderBy(x => Random.value).FirstOrDefault();
            if (randomPlayer != null)
            {
                int damage = CalculateDamage(enemy, randomPlayer) / 2;
                randomPlayer.TakeDamage(damage);
            }
        }
        StartEscapeTimer();
        CheckBattleEnd();
    }

    // СИСТЕМА ДРУЗЕЙ
    private void StartFriendSystem()
    {
        if (enableFriendSystem && FriendSystem.Instance != null)
            FriendSystem.Instance.StartFriendJoinProcess();
    }

    // ЗАВЕРШЕНИЕ БОЯ
    private void CheckBattleEnd()
    {
        if (enemyTeam.All(e => !e.IsAlive))
            EndBattle(true);
        else if (!PlayerDataManager.Instance.IsAlive)
            EndBattle(false);
    }

    private void EndBattle(bool playerWon)
    {
        isBattleActive = false;
        canEscape = false;
        StopAllCoroutines();

        // Останавливаем систему друзей
        if (FriendSystem.Instance != null)
            FriendSystem.Instance.StopFriendSystem();

        // Скрываем UI
        if (battleUICanvas != null)
            battleUICanvas.SetActive(false);

        if (playerWon)
        {
            Debug.Log("Battle won! Victory!");
            DistributeRewards();
        }
        else
        {
            Debug.Log("Battle lost! Defeat!");
        }

        OnBattleEnded?.Invoke(playerWon);
    }

    private void EndBattleWithEscape(bool escaped)
    {
        isBattleActive = false;
        canEscape = false;
        StopAllCoroutines();

        if (FriendSystem.Instance != null)
            FriendSystem.Instance.StopFriendSystem();

        if (battleUICanvas != null)
            battleUICanvas.SetActive(false);

        OnBattleEnded?.Invoke(false);
        OnBattleEscaped?.Invoke(escaped);
    }

    // НАГРАДЫ
    private void DistributeRewards()
    {
        int expReward = enemyTeam.Count * 50;
        float moneyReward = enemyTeam.Count * 25f;

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.AddExperience(expReward);
            PlayerDataManager.Instance.AddMoney(moneyReward);
        }
    }

    private void DistributeEscapeRewards()
    {
        int expReward = Mathf.RoundToInt(enemyTeam.Count * 10);
        float moneyReward = Mathf.RoundToInt(enemyTeam.Count * 5);

        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.AddExperience(expReward);
            PlayerDataManager.Instance.AddMoney(moneyReward);
        }
    }

    // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
    private void UpdateCharacterUI(NPCDataManager character)
    {
        playerIcon.GetComponent<PlayerCharacterIcon>().UpdateBars();
        foreach (var playerTeamIcon in playerTeamIcons)
        {
            playerTeamIcon.GetComponent<CharacterIcon>().UpdateBars();
        }
        foreach (var enemyTeamIcon in enemyTeamIcons)
        {
            enemyTeamIcon.GetComponent<CharacterIcon>().UpdateBars();
        }
    }

    // API МЕТОДЫ
    public void AddEnemyToBattle(NPCDataManager newEnemy)
    {
        if (!isBattleActive || enemyTeam.Count >= maxEnemyTeamSize) return;

        enemyTeam.Add(newEnemy);
        //CreateCharacterIcon(newEnemy, enemyTeamPanel, true);
        //newEnemy.InitializeCharacter();

        if (selectedTarget == null || !selectedTarget.IsAlive)
        {
            selectedTarget = newEnemy;
            UpdateTargetVisuals();
        }
    }

    public void AddFriendToBattle(NPCDataManager friend)
    {
        if (!isBattleActive) return;

        playerTeam.Add(friend);
        //CreateCharacterIcon(friend, friendsPanel, false);
        StartCoroutine(AutoAttackRoutine(friend));
    }

    public void ForceAttack(NPCDataManager attacker, NPCDataManager target)
    {
        Attack(attacker, target);
    }

    public void HealCharacter(NPCDataManager character, int healAmount)
    {
        if (character.IsAlive)
        {
            character.Heal(healAmount);
            UpdateCharacterUI(character);
        }
    }

    public List<NPCDataManager> GetPlayerTeam() => new List<NPCDataManager>(playerTeam);
    public List<NPCDataManager> GetEnemyTeam() => new List<NPCDataManager>(enemyTeam);
    public NPCDataManager GetSelectedTarget() => selectedTarget;
    public float GetEscapeTimer() => currentEscapeTimer;
    public bool CanEscape() => canEscape;

    // СОБЫТИЯ
    public System.Action<bool> OnBattleEnded;
    public System.Action<bool> OnBattleEscaped;
}