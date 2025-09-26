// PlayerDataManager.cs
using UnityEngine;
using System;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance; // Синглтон для простого доступа

    public PlayerAttributes Attributes { get; private set; }
    public PlayerStats Stats { get; private set; }

    private string _saveKey = "PlayerStatsAndAttributs"; // Ключ для PlayerPrefs

    private void Awake()
    {
        // Простая реализация синглтона
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeData()
    {
        // Создаем экземпляры классов
        Attributes = new PlayerAttributes();
        Stats = new PlayerStats();

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

        Stats.Money = 25;
        Stats.CurrentHealth = Stats.MaxHealth; // Полное здоровье
        Stats.CurrentStamina = Stats.MaxStamina;

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
        // ... сохраняем другие необходимые поля из Stats

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

    //для тестов
    public void ApplyDamage(int damage)
    {
        if (PlayerDataManager.Instance == null) return;

        PlayerDataManager.Instance.Stats.CurrentHealth -= damage;
        Debug.Log($"Нанесено урона: {damage}. Здоровье: {PlayerDataManager.Instance.Stats.CurrentHealth}");

        // Автосохранение при получении урона (опционально)
        PlayerDataManager.Instance.SaveData();
    }

    public void Heal(int countPoint)
    {
        if (PlayerDataManager.Instance == null) return;

        PlayerDataManager.Instance.Stats.CurrentHealth += countPoint;
        Debug.Log($"Вылечено: {countPoint}. Здоровье: {PlayerDataManager.Instance.Stats.CurrentHealth}");

        // Автосохранение при получении урона (опционально)
        PlayerDataManager.Instance.SaveData();
    }
}

// Вспомогательный класс, который будет сериализоваться в JSON
[System.Serializable]
public class PlayerSaveData
{
    public PlayerAttributes attributes;
    public int currentHealth;
    public float currentStamina;
    public float money;
    public float currentWeight;
}