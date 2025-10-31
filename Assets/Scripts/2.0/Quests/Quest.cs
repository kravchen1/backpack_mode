using System;
using UnityEngine;

[Serializable]
public class Quest
{
    #region Fields
    public string questName;
    public string description;
    public bool isCompleted;
    public int currentProgress;
    public int necessaryProgress;
    public int id;
    public QuestType questType;
    public int rewardGold;
    public int rewardExp;
    public int reputationReward;
    public string targetItem;
    public string targetEnemy;
    public string giverNPCId; // ID NPC, который выдал квест
    #endregion

    #region Constructor
    public Quest(string name, string desc, int necessaryProgress = 0, int id = 0,
                QuestType type = QuestType.Kill, string target = "",
                int goldReward = 0, int expReward = 0, int reputation = 10, string giverId = "")
    {
        questName = name;
        description = desc;
        currentProgress = 0;
        this.necessaryProgress = necessaryProgress;
        isCompleted = false;
        this.id = id;
        questType = type;
        rewardGold = goldReward;
        rewardExp = expReward;
        reputationReward = reputation;
        giverNPCId = giverId;

        if (type == QuestType.Kill)
            targetEnemy = target;
        else if (type == QuestType.Collect)
            targetItem = target;
    }
    #endregion

    #region Public Methods
    public void IncrementProgress()
    {
        currentProgress++;
        CheckCompletion();
    }

    public void SetProgress(int progress)
    {
        currentProgress = progress;
        CheckCompletion();
    }

    private void CheckCompletion()
    {
        if (currentProgress >= necessaryProgress)
        {
            isCompleted = true;
        }
    }

    public bool IsProgressComplete()
    {
        return necessaryProgress <= 0 || currentProgress >= necessaryProgress;
    }

    public string GetProgressText()
    {
        return necessaryProgress > 0 ? $" ({currentProgress}/{necessaryProgress})" : "";
    }
    #endregion
}

public enum QuestType
{
    Kill,
    Collect,
    Explore,
    Bring//todo ДЛЯ DeepSeek в остальных местах. В этом случае торговцу нужно принести определённые предметы. Думмаю для этого нужно придумать новый класс ItemQuest
}