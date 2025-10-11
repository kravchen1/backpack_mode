// PlayerDataManager.cs
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NPCDataManager : MonoBehaviour
{
    [Header("Character Info")]
    public string CharacterName;
    public string backpackKey;
    public bool IsPlayerTeam;
    public PlayerAttributes Attributes { get; private set; }
    public PlayerStats Stats { get; private set; }
    public bool IsAlive => Stats.CurrentHealth > 0;


    [Header("Death Settings")]
    public GameObject deathChestPrefab; // Префаб сундука, который появится после смерти
    public int countDropChests = 1;
    public float deathAnimationDuration = 2f; // Длительность анимации смерти перед удалением
    public bool isMultiTilesDeathChestPrefab = false;
    public Vector2Int multiTilesDeathChestPrefabVector2 = new Vector2Int(2,2);

    private string _saveKey;

    public CellsData cellsFight;

    // Событие смерти
    public event Action<GameObject> OnDeath; // Передаем GameObject умершего NPC
    #region Initialize
    private void Awake()
    {
        InitializeData();
    }

    private void InitializeData()
    {
        _saveKey = gameObject.name;
        // Создаем экземпляры классов
        Attributes = new PlayerAttributes();
        Stats = GetComponent<PlayerStats>() ?? gameObject.AddComponent<PlayerStats>();

        // Инициализируем Stats, передавая ему Attributes
        Stats.Initialize(Attributes);

        // Подписываемся на событие смерти из Stats
        Stats.OnDeath += HandleDeath;

        // Загружаем данные
        LoadData();
    }
    #endregion

    #region SaveLoadMethods
    // Метод для быстрого сброса к дефолтным значениям (для тестирования)
    [ContextMenu("Reset Data")]
    public void ResetToDefault()
    {
        // Устанавливаем базовые значения
        Attributes.endurance.BaseValue = 1;
        Attributes.strength.BaseValue = 1;
        Attributes.agility.BaseValue = 1;
        Attributes.intellect.BaseValue = 1;
        Attributes.charisma.BaseValue = 1;
        Attributes.luck.BaseValue = 1;

        Stats.SetLevel(1);
        Stats.CurrentExp = 0;
        Stats.Money = 0.001f;
        Stats.CurrentHealth = Stats.MaxHealth; // Полное здоровье
        Stats.CurrentStamina = Stats.MaxStamina;
        Stats.CurrentWeight = 0;

        //Debug.Log("Data reset to default.");
    }

    [ContextMenu("Save Data")]
    public void SaveData()
    {
        // Сериализуем все данные в один класс для сохранения
        PlayerSaveData saveData = new PlayerSaveData();
        saveData.attributes = Attributes;
        saveData.currentHealth = Stats.CurrentHealth;
        saveData.currentStamina = Stats.CurrentStamina;
        saveData.money = Stats.Money;
        saveData.currentWeight = Stats.CurrentWeight;
        saveData.level = Stats.Level;
        saveData.currentExp = Stats.CurrentExp;
        saveData.unspentSkillPoints = Stats.UnspentSkillPoints;

        string jsonData = JsonUtility.ToJson(saveData, true); // true для красивого форматирования в отладке
        PlayerPrefs.SetString(_saveKey, jsonData);
        PlayerPrefs.Save(); // Важно вызывать Save()

        //Debug.Log("Game Saved: " + jsonData);
    }

    [ContextMenu("Load Data")]
    public void LoadData()
    {
        if (PlayerPrefs.HasKey(_saveKey))
        {
            string jsonData = PlayerPrefs.GetString(_saveKey);
            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(jsonData);

            // Восстанавливаем атрибуты
            Attributes = saveData.attributes ?? new PlayerAttributes();

            // Восстанавливаем Stats
            Stats.Initialize(Attributes); // Сначала инициализируем, чтобы подписаться на события и пересчитать статы
            Stats.SetLevel(saveData.level);
            Stats.CurrentExp = saveData.currentExp;
            Stats.UnspentSkillPoints = saveData.unspentSkillPoints;
            Stats.CurrentHealth = saveData.currentHealth;
            Stats.CurrentStamina = saveData.currentStamina;
            Stats.Money = saveData.money;
            Stats.CurrentWeight = saveData.currentWeight;

            //Debug.Log("Game Loaded: " + jsonData);
        }
        else
        {
            //Debug.Log("No save data found. Initializing with default values.");
            ResetToDefault();
        }
    }

    // Вызывайте этот метод при выходе из игры или в контрольных точках
    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveData();
        }
    }
    #endregion

    #region Death Handling
    private void HandleDeath()
    {
        Debug.Log($"{CharacterName} умер!");

        // Вызываем событие смерти
        OnDeath?.Invoke(gameObject);

        // Запускаем процесс смерти
        StartDeathSequence();
    }

    private void StartDeathSequence()
    {
        PlayerPrefs.SetInt(CharacterName + "Die", 1);
        // 1. Отключаем компоненты, которые не нужны мертвому NPC
        DisableNPCComponents();

        // 2. Запускаем анимацию смерти
        PlayDeathAnimation();

        // 3. Создаем сундук через заданное время
        Invoke(nameof(SpawnDeathChest), deathAnimationDuration);

        // 4. Удаляем NPC через заданное время
        Invoke(nameof(DestroyNPC), deathAnimationDuration + 0.1f);
    }

    private void DisableNPCComponents()
    {
        var npcNavigationAgent = GetComponent<NPCNavigationAgent>();
        if (npcNavigationAgent != null)
        {
            npcNavigationAgent.StopAllMovement();
            npcNavigationAgent.enabled = false;
        }

        // Отключаем NPCController если есть
        var npcController = GetComponent<NPCController>();
        if (npcController != null) npcController.enabled = false;

        // Отключаем NavMeshAgent если есть
        var navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null) navMeshAgent.enabled = false;

        // Отключаем коллайдеры
        var colliders = GetComponents<Collider2D>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private void PlayDeathAnimation()
    {
        var animationController = GetComponent<NPCAnimationController>();
        if (animationController != null && animationController.animator != null)
        {
            // Запускаем анимацию смерти
            animationController.animator.SetBool("IsDead", true);

            // Отключаем анимацию движения
            animationController.enabled = false;
        }
        else
        {
            // Если аниматора нет, просто делаем NPC полупрозрачным
            StartCoroutine(FadeOutCoroutine());
        }
    }

    private System.Collections.IEnumerator FadeOutCoroutine()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            float fadeTime = deathAnimationDuration;
            float elapsedTime = 0f;
            Color originalColor = spriteRenderer.color;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
        }
    }

    private void SpawnDeathChest()
    {
        if (deathChestPrefab != null)
        {
            for (int i = 0; i < countDropChests; i++)
            {
                GameObject chest;
                if (isMultiTilesDeathChestPrefab)
                {
                    chest = GridObjectManager.Instance.SpawnMultiCellObject(
                        deathChestPrefab,
                        transform.position,
                        multiTilesDeathChestPrefabVector2,
                        backpackKey
                    );
                }
                else
                {
                    chest = GridObjectManager.Instance.SpawnObject(
                        deathChestPrefab,
                        transform.position,
                        backpackKey
                    );
                }

                if (chest != null)
                {
                    ConfigureDeathChest(chest);
                }
            }
        }
    }

    private void ConfigureDeathChest(GameObject chest)
    {
        var chestTrigger = chest.GetComponent<ChestDeathTrigger>();
        if (chestTrigger != null)
        {
            // Настраиваем количество предметов в сундуке в зависимости от уровня NPC
            chestTrigger.settingsKey = backpackKey;

            // Можно добавить другие настройки сундука
            // Например, качество предметов в зависимости от уровня и т.д.
        }
    }

    private void DestroyNPC()
    {
        // Сохраняем данные перед удалением (если нужно)
        SaveData();

        // Уничтожаем объект
        Destroy(gameObject);
    }
    #endregion

    #region Expirience
    // Методы для работы с опытом и уровнем
    public void AddExperience(int expAmount)
    {
        Stats.AddExp(expAmount);
        Debug.Log($"Added {expAmount} exp. Current: {Stats.CurrentExp}/{Stats.ExpToNextLevel}. Level: {Stats.Level}");

        SaveData();
    }
    public void LevelUp()
    {
        // Добавляем достаточно опыта для следующего уровня
        int neededExp = Stats.ExpToNextLevel - Stats.CurrentExp;
        Stats.AddExp(neededExp);

        SaveData();
    }
    #endregion

    #region Attributes
    // Метод для траты очков улучшений
    public bool SpendSkillPointOnAttribute(System.Func<bool> attributeUpgradeMethod)
    {

        bool success = Stats.SpendSkillPointOnAttribute(attributeUpgradeMethod);
        if (success)
        {
            SaveData();
        }
        return success;
    }
    // Упрощенные методы для повышения конкретных атрибутов
    public bool UpgradeStrength()
    {
        return SpendSkillPointOnAttribute(() =>
        {
            Attributes.strength.BaseValue++;
            return true;
        });
    }
    public bool UpgradeEndurance()
    {
        return SpendSkillPointOnAttribute(() =>
        {
            Attributes.endurance.BaseValue++;
            return true;
        });
    }
    public bool UpgradeAgility()
    {
        return SpendSkillPointOnAttribute(() =>
        {
            Attributes.agility.BaseValue++;
            return true;
        });
    }
    public bool UpgradeIntellect()
    {
        return SpendSkillPointOnAttribute(() =>
        {
            Attributes.intellect.BaseValue++;
            return true;
        });
    }
    public bool UpgradeCharisma()
    {
        return SpendSkillPointOnAttribute(() =>
        {
            Attributes.charisma.BaseValue++;
            return true;
        });
    }
    public bool UpgradeLuck()
    {
        return SpendSkillPointOnAttribute(() =>
        {
            Attributes.luck.BaseValue++;
            return true;
        });
    }
    #endregion

    #region MoneyAPI
    // Методы для управления деньгами
    public void AddMoney(float amount)
    {

        Stats.Money += amount;
        Debug.Log($"Added {amount} money. Total: {Stats.Money}");

        SaveData();
    }

    public bool SpendMoney(float amount)
    {
        if (Stats.Money < amount) return false;

        Stats.Money -= amount;
        Debug.Log($"Spent {amount} money. Remaining: {Stats.Money}");

        SaveData();
        return true;
    }
    #endregion

    #region BattleAPI
    public void TakeDamage(int damage)
    {
        if (!IsAlive) return; // Не принимаем урон если уже мертв

        if (UnityEngine.Random.Range(0, 100) < 10)//обходим броню
        {
            Stats.CurrentHealth -= damage;
            Debug.Log($"Нанесено урона: {damage}. Здоровье: {Stats.CurrentHealth}");
        }
        else//ищем броню, которая впитает урон
        {
            Stats.CurrentHealth -= DestroyRandomArmorInFight(damage);
        }

        SaveData();
    }

    private int DestroyRandomArmorInFight(int damage)
    {
        //ItemArmorController foundArmor = new ItemArmorController();
        int remaining = damage;
        if (BattleManager.Instance.isBattleActive)
        {
            var Armors = cellsFight.GetComponentsInChildren<ItemArmorController>().Where(e => e.gameObject.GetComponent<ItemStats>().durability > 0 && e.gameObject.GetComponent<ItemStats>().isUseFight).ToList();
            int random = UnityEngine.Random.Range(0, Armors.Count);
            remaining = Armors[random].TakeDamage(damage);
        }
        //else if (cellsInventory.gameObject.activeSelf)
        //{
        //    var Armors = cellsInventory.GetComponentsInChildren<ItemArmorController>().Where(e => e.gameObject.GetComponent<ItemStats>().durability > 0 && e.gameObject.GetComponent<ItemStats>().isUseFight).ToList();
        //    int random = UnityEngine.Random.Range(0, Armors.Count);
        //    remaining = Armors[random].TakeDamage(damage);
        //}
        return remaining;
    }

    public void Heal(int countPoint)
    {
        if (!IsAlive) return; // Не лечим мертвых

        Stats.CurrentHealth += countPoint;
        Debug.Log($"Вылечено: {countPoint}. Здоровье: {Stats.CurrentHealth}");

        SaveData();
    }
    #endregion

    private void OnDestroy()
    {
        // Отписываемся от событий при уничтожении
        if (Stats != null)
        {
            Stats.OnDeath -= HandleDeath;
        }
    }


    private void FixedUpdate()
    {
        UpdateStaminaRegen();
    }
    private void UpdateStaminaRegen()
    {
        Stats.UpdateStaminaRegen(Time.deltaTime);
    }

}
