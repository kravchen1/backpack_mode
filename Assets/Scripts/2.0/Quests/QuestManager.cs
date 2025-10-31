using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    #region Fields
    [Header("UI References")]
    [SerializeField] private GameObject questUIPrefab;
    [SerializeField] private Transform questsContainer;

    [Header("Quest Settings")]
    [SerializeField] private List<Quest> availableQuests = new List<Quest>();

    private const string QUEST_DATA_KEY = "PlayerQuests";
    private const string REPUTATION_DATA_KEY = "NPCReputations";
    private List<Quest> activeQuests = new List<Quest>();
    private QuestData questData = new QuestData();
    private Dictionary<string, int> npcReputations = new Dictionary<string, int>();

    #endregion

    #region Unity Methods
    private void Start()
    {
        LoadQuests();
        LoadReputations();
    }
    #endregion

    #region Public Methods - Quest Management
    public void AddQuest(Quest quest)
    {
        // Проверяем не только по ID, но и по статусу завершения
        var existingQuest = questData.quests.FirstOrDefault(q => q.id == quest.id);
        if (existingQuest != null && !existingQuest.isCompleted)
        {
            Debug.LogWarning($"Quest with ID {quest.id} already exists and is active!");
            return;
        }

        // Если квест завершен, создаем новый экземпляр
        Quest newQuest = new Quest(quest.questName, quest.description, quest.necessaryProgress,
                                 quest.id, quest.questType, quest.targetEnemy,
                                 quest.rewardGold, quest.rewardExp, quest.reputationReward, quest.giverNPCId);

        questData.quests.Add(newQuest);
        activeQuests.Add(newQuest);
        SaveQuests();

        Debug.Log($"Quest added: {quest.questName}");
    }

    public bool CompleteQuest(int questID)
    {
        Quest quest = questData.quests.FirstOrDefault(q => q.id == questID && !q.isCompleted);

        if (quest != null)
        {
            quest.isCompleted = true;
            GiveQuestReward(quest);

            // Увеличиваем репутацию у NPC
            if (!string.IsNullOrEmpty(quest.giverNPCId))
            {
                IncreaseReputation(quest.giverNPCId, quest.reputationReward);
            }

            activeQuests.Remove(quest);
            SaveQuests();
            //UpdateQuestUI();

            Debug.Log($"Quest completed: {quest.questName}");
            return true;
        }

        return false;
    }

    public void UpdateQuestProgress(QuestType type, string target, int amount = 1)
    {
        bool needsUpdate = false;

        foreach (Quest quest in activeQuests.Where(q => !q.isCompleted && q.questType == type))
        {
            bool shouldUpdate = false;

            switch (type)
            {
                case QuestType.Kill:
                    if (quest.targetEnemy == target)
                        shouldUpdate = true;
                    break;
                case QuestType.Collect:
                    if (quest.targetItem == target)
                        shouldUpdate = true;
                    break;
            }

            if (shouldUpdate)
            {
                quest.SetProgress(quest.currentProgress + amount);
                if (quest.isCompleted)
                {
                    CompleteQuest(quest.id);
                }
                needsUpdate = true;
            }
        }

        if (needsUpdate)
        {
            SaveQuests();
        }
    }
    #endregion

    #region Public Methods - Quest Checking
    public bool IsQuestCompleted(int questID)
    {
        return questData.quests.Any(q => q.id == questID && q.isCompleted);
    }

    public bool HasQuest(int questID)
    {
        // Проверяем есть ли активный (не завершенный) квест с таким ID
        return questData.quests.Any(q => q.id == questID && !q.isCompleted);
    }

    public List<Quest> GetAvailableQuests()
    {
        return availableQuests.Where(q => !HasQuest(q.id) && !IsQuestCompleted(q.id)).ToList();
    }
    #endregion

    #region Public Methods - Reputation System
    public void IncreaseReputation(string npcId, int amount)
    {
        if (!npcReputations.ContainsKey(npcId))
        {
            npcReputations[npcId] = 0;
        }

        npcReputations[npcId] += amount;
        SaveReputations();

        Debug.Log($"Reputation with {npcId} increased by {amount}. Current: {npcReputations[npcId]}");
    }

    public int GetReputation(string npcId)
    {
        return npcReputations.ContainsKey(npcId) ? npcReputations[npcId] : 0;
    }

    public void SetReputation(string npcId, int value)
    {
        npcReputations[npcId] = value;
        SaveReputations();
    }
    #endregion

    #region Private Methods - Data Management
    private void LoadQuests()
    {
        if (PlayerPrefs.HasKey(QUEST_DATA_KEY))
        {
            string json = PlayerPrefs.GetString(QUEST_DATA_KEY);
            questData = JsonUtility.FromJson<QuestData>(json);
            activeQuests = questData.quests.Where(q => !q.isCompleted).ToList();
        }
        else
        {
            questData = new QuestData();
        }
    }

    private void SaveQuests()
    {
        string json = JsonUtility.ToJson(questData);
        PlayerPrefs.SetString(QUEST_DATA_KEY, json);
        PlayerPrefs.Save();
    }

    private void LoadReputations()
    {
        if (PlayerPrefs.HasKey(REPUTATION_DATA_KEY))
        {
            string json = PlayerPrefs.GetString(REPUTATION_DATA_KEY);
            ReputationData data = JsonUtility.FromJson<ReputationData>(json);
            npcReputations = data.reputations ?? new Dictionary<string, int>();
        }
        else
        {
            npcReputations = new Dictionary<string, int>();
        }
    }

    private void SaveReputations()
    {
        ReputationData data = new ReputationData { reputations = npcReputations };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(REPUTATION_DATA_KEY, json);
        PlayerPrefs.Save();
    }
    #endregion


    #region Private Methods - Rewards
    private void GiveQuestReward(Quest quest)
    {
        if (quest.rewardGold > 0)
        {
            // Player.Instance.AddGold(quest.rewardGold);
            Debug.Log($"Received {quest.rewardGold} gold from quest!");
        }

        if (quest.rewardExp > 0)
        {
            // Player.Instance.AddExperience(quest.rewardExp);
            Debug.Log($"Received {quest.rewardExp} experience from quest!");
        }
    }
    #endregion

    #region Public Methods - UI Toggle
    public void ToggleQuestUI()
    {
        bool isActive = !transform.GetChild(0).gameObject.activeSelf;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(isActive);
        }

        PlayerPrefs.SetInt("QuestTableActive", isActive ? 1 : 0);
    }

    // Добавьте эти методы в класс QuestManager:

    public List<Quest> GetActiveQuests()
    {
        return activeQuests.Where(q => !q.isCompleted).ToList();
    }

    public List<Quest> GetCompletedQuests()
    {
        return questData.quests.Where(q => q.isCompleted).ToList();
    }

    public List<Quest> GetQuestsByGiver(string npcId)
    {
        return questData.quests.Where(q => q.giverNPCId == npcId).ToList();
    }

    #endregion
}

[System.Serializable]
public class ReputationData
{
    public Dictionary<string, int> reputations;
}