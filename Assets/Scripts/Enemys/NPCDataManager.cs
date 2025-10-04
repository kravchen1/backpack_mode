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

    private string _saveKey;// = "�����StatsAndAttributes"; // ���� ��� PlayerPrefs

    public bool IsAlive => Stats.CurrentHealth > 0;
    private void Awake()
    {
        InitializeData();
    }

    private void InitializeData()
    {
        _saveKey = gameObject.name;
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
        Stats.AddExp(expAmount);
        Debug.Log($"Added {expAmount} exp. Current: {Stats.CurrentExp}/{Stats.ExpToNextLevel}. Level: {Stats.Level}");

        SaveData();
    }

    public void LevelUp()
    {
        // ��������� ���������� ����� ��� ���������� ������
        int neededExp = Stats.ExpToNextLevel - Stats.CurrentExp;
        Stats.AddExp(neededExp);

        SaveData();
    }

    // ����� ��� ����� ����� ���������
    public bool SpendSkillPointOnAttribute(System.Func<bool> attributeUpgradeMethod)
    {

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
    public void TakeDamage(int damage)
    {

        Stats.CurrentHealth -= damage;
        Debug.Log($"�������� �����: {damage}. ��������: {Stats.CurrentHealth}");

        SaveData();
    }

    public void Heal(int countPoint)
    {

        Stats.CurrentHealth += countPoint;
        Debug.Log($"��������: {countPoint}. ��������: {Stats.CurrentHealth}");

        SaveData();
    }

    // ������ ��� ���������� ��������
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

    // ����� ��� ������������ - ���������� ����� ���������
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
