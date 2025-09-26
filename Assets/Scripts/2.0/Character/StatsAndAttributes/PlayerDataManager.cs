// PlayerDataManager.cs
using UnityEngine;
using System;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance; // �������� ��� �������� �������

    public PlayerAttributes Attributes { get; private set; }
    public PlayerStats Stats { get; private set; }

    private string _saveKey = "PlayerStatsAndAttributs"; // ���� ��� PlayerPrefs

    private void Awake()
    {
        // ������� ���������� ���������
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
        // ������� ���������� �������
        Attributes = new PlayerAttributes();
        Stats = new PlayerStats();

        // �������������� Stats, ��������� ��� Attributes
        Stats.Initialize(Attributes);

        // ��������� ������
        LoadData();
    }

    // ����� ��� �������� ������ � ��������� ��������� (��� ������������)
    [ContextMenu("Reset Data")]
    public void ResetToDefault()
    {
        // ������������� ������� ��������
        Attributes.endurance.BaseValue = 1;
        Attributes.strength.BaseValue = 1;
        Attributes.agility.BaseValue = 1;
        Attributes.intellect.BaseValue = 1;
        Attributes.charisma.BaseValue = 1;
        Attributes.luck.BaseValue = 1;

        Stats.Money = 25;
        Stats.CurrentHealth = Stats.MaxHealth; // ������ ��������
        Stats.CurrentStamina = Stats.MaxStamina;

        Debug.Log("Data reset to default.");
    }

    [ContextMenu("Save Data")]
    public void SaveData()
    {
        // ����������� ��� ������ � ���� ����� ��� ����������
        PlayerSaveData saveData = new PlayerSaveData();
        saveData.attributes = Attributes;
        saveData.currentHealth = Stats.CurrentHealth;
        saveData.currentStamina = Stats.CurrentStamina;
        saveData.money = Stats.Money;
        saveData.currentWeight = Stats.CurrentWeight;
        // ... ��������� ������ ����������� ���� �� Stats

        string jsonData = JsonUtility.ToJson(saveData, true); // true ��� ��������� �������������� � �������
        PlayerPrefs.SetString(_saveKey, jsonData);
        PlayerPrefs.Save(); // ����� �������� Save()

        Debug.Log("Game Saved: " + jsonData);
    }

    [ContextMenu("Load Data")]
    public void LoadData()
    {
        if (PlayerPrefs.HasKey(_saveKey))
        {
            string jsonData = PlayerPrefs.GetString(_saveKey);
            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(jsonData);

            // ��������������� ��������
            Attributes = saveData.attributes ?? new PlayerAttributes();

            // ��������������� Stats
            Stats.Initialize(Attributes); // ������� ��������������, ����� ����������� �� ������� � ����������� �����
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

    // ��������� ���� ����� ��� ������ �� ���� ��� � ����������� ������
    private void OnApplicationQuit()
    {
        SaveData();
    }

    //��� ������
    public void ApplyDamage(int damage)
    {
        if (PlayerDataManager.Instance == null) return;

        PlayerDataManager.Instance.Stats.CurrentHealth -= damage;
        Debug.Log($"�������� �����: {damage}. ��������: {PlayerDataManager.Instance.Stats.CurrentHealth}");

        // �������������� ��� ��������� ����� (�����������)
        PlayerDataManager.Instance.SaveData();
    }

    public void Heal(int countPoint)
    {
        if (PlayerDataManager.Instance == null) return;

        PlayerDataManager.Instance.Stats.CurrentHealth += countPoint;
        Debug.Log($"��������: {countPoint}. ��������: {PlayerDataManager.Instance.Stats.CurrentHealth}");

        // �������������� ��� ��������� ����� (�����������)
        PlayerDataManager.Instance.SaveData();
    }
}

// ��������������� �����, ������� ����� ��������������� � JSON
[System.Serializable]
public class PlayerSaveData
{
    public PlayerAttributes attributes;
    public int currentHealth;
    public float currentStamina;
    public float money;
    public float currentWeight;
}