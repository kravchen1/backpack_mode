// PlayerDataManager.cs
using UnityEngine;
using System;

public class NPCDataManager : MonoBehaviour
{
    [Header("Character Info")]
    public string CharacterName;
    public bool IsPlayerTeam;
    public PlayerAttributes Attributes { get; private set; }
    public PlayerStats Stats { get; private set; }

    private string _saveKey;// = "УтуьнStatsAndAttributes"; // Ключ для PlayerPrefs

    public bool IsAlive => Stats.CurrentHealth > 0;
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

        // Загружаем данные
        LoadData();
    }

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

        Debug.Log("Data reset to default.");
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

        Debug.Log("Game Saved: " + jsonData);
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

            Debug.Log("Game Loaded: " + jsonData);
        }
        else
        {
            Debug.Log("No save data found. Initializing with default values.");
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

    //для тестов
    public void TakeDamage(int damage)
    {

        Stats.CurrentHealth -= damage;
        Debug.Log($"Нанесено урона: {damage}. Здоровье: {Stats.CurrentHealth}");

        SaveData();
    }

    public void Heal(int countPoint)
    {

        Stats.CurrentHealth += countPoint;
        Debug.Log($"Вылечено: {countPoint}. Здоровье: {Stats.CurrentHealth}");

        SaveData();
    }

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

    // Метод для тестирования - добавление очков улучшений
    [ContextMenu("Add 5 Skill Points")]
    public void AddTestSkillPoints()
    {


        Stats.AddSkillPoints(5);
        Debug.Log($"Added 5 skill points. Total: {Stats.UnspentSkillPoints}");

        SaveData();
    }

    [ContextMenu("Add 100 Experience")]
    public void AddTestExperience()
    {
        AddExperience(100);
    }
}
