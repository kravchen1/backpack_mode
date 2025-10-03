// PlayerDataManager.cs
using UnityEngine;
using System;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance; // �������� ��� �������� �������

    public PlayerAttributes Attributes { get; private set; }
    public PlayerStats Stats { get; private set; }

    private string _saveKey = "PlayerStatsAndAttributes"; // ���� ��� PlayerPrefs

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
        Stats = GetComponent<PlayerStats>() ?? gameObject.AddComponent<PlayerStats>();

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

        Stats.SetLevel(1);
        Stats.CurrentExp = 0;
        Stats.Money = 0.001f;
        Stats.CurrentHealth = Stats.MaxHealth; // ������ ��������
        Stats.CurrentStamina = Stats.MaxStamina;
        Stats.CurrentWeight = 0;

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
        saveData.level = Stats.Level;
        saveData.currentExp = Stats.CurrentExp;
        saveData.unspentSkillPoints = Stats.UnspentSkillPoints;

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

    // ��������� ���� ����� ��� ������ �� ���� ��� � ����������� ������
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

    // ������ ��� ������ � ������ � �������
    public void AddExperience(int expAmount)
    {
        if (Instance == null) return;

        Stats.AddExp(expAmount);
        Debug.Log($"Added {expAmount} exp. Current: {Stats.CurrentExp}/{Stats.ExpToNextLevel}. Level: {Stats.Level}");

        SaveData();
    }

    public void LevelUp()
    {
        if (Instance == null) return;

        // ��������� ���������� ����� ��� ���������� ������
        int neededExp = Stats.ExpToNextLevel - Stats.CurrentExp;
        Stats.AddExp(neededExp);

        SaveData();
    }

    // ����� ��� ����� ����� ���������
    public bool SpendSkillPointOnAttribute(System.Func<bool> attributeUpgradeMethod)
    {
        if (Instance == null) return false;

        bool success = Stats.SpendSkillPointOnAttribute(attributeUpgradeMethod);
        if (success)
        {
            SaveData();
        }
        return success;
    }

    // ���������� ������ ��� ��������� ���������� ���������
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

    //��� ������
    public void ApplyDamage(int damage)
    {
        if (Instance == null) return;

        Stats.CurrentHealth -= damage;
        Debug.Log($"�������� �����: {damage}. ��������: {Stats.CurrentHealth}");

        SaveData();
    }

    public void Heal(int countPoint)
    {
        if (Instance == null) return;

        Stats.CurrentHealth += countPoint;
        Debug.Log($"��������: {countPoint}. ��������: {Stats.CurrentHealth}");

        SaveData();
    }

    // ������ ��� ���������� ��������
    public void AddMoney(float amount)
    {
        if (Instance == null) return;

        Stats.Money += amount;
        Debug.Log($"Added {amount} money. Total: {Stats.Money}");

        SaveData();
    }

    public bool SpendMoney(float amount)
    {
        if (Instance == null || Stats.Money < amount) return false;

        Stats.Money -= amount;
        Debug.Log($"Spent {amount} money. Remaining: {Stats.Money}");

        SaveData();
        return true;
    }

    // ����� ��� ������������ - ���������� ����� ���������
    [ContextMenu("Add 5 Skill Points")]
    public void AddTestSkillPoints()
    {
        if (Instance == null) return;

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

// ��������������� �����, ������� ����� ��������������� � JSON
[System.Serializable]
public class PlayerSaveData
{
    public PlayerAttributes attributes;
    public int currentHealth;
    public float currentStamina;
    public float money;
    public float currentWeight;
    public int level;
    public int currentExp;
    public int unspentSkillPoints;
}