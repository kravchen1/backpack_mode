using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class QuestGiverManager : MonoBehaviour
{
    #region Fields
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI textTraderName;
    [SerializeField] private TextMeshProUGUI textTraderReputation;

    [SerializeField] private GameObject panelAvailableContent;
    [SerializeField] private GameObject panelActiveContent;
    [SerializeField] private GameObject panelHistoryContent;

    [SerializeField] private GameObject panelAvailableDescription;
    [SerializeField] private TextMeshProUGUI textAvailableDescription;
    [SerializeField] private TextMeshProUGUI textAvailableConditions;
    [SerializeField] private TextMeshProUGUI textAvailableRewards;
    [SerializeField] private Button buttonAvailableAccept;

    [SerializeField] private GameObject panelActiveDescription;
    [SerializeField] private TextMeshProUGUI textActiveDescription;
    [SerializeField] private TextMeshProUGUI textActiveConditions;
    [SerializeField] private TextMeshProUGUI textActiveRewards;
    [SerializeField] private Button buttonActiveComplete;

    [SerializeField] private GameObject panelHistoryDescription;
    [SerializeField] private TextMeshProUGUI textHistoryDescription;
    [SerializeField] private TextMeshProUGUI textHistoryConditions;
    [SerializeField] private TextMeshProUGUI textHistoryRewards;

    [SerializeField] private Transform contentQuestsAvailable;
    [SerializeField] private Transform contentQuestsActive;
    [SerializeField] private Transform contentQuestsHistory;

    [SerializeField] private GameObject prefabQuestButton;
    [SerializeField] private Button buttonAvailableTab;
    [SerializeField] private Button buttonActiveTab;
    [SerializeField] private Button buttonHistoryTab;

    [Header("Tab Colors")]
    [SerializeField] private Color tabActiveColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color tabInactiveColor = new Color(0.3f, 0.3f, 0.3f);

    [Header("Quest Data")]
    [SerializeField] private List<Quest> availableQuests = new List<Quest>();

    private QuestManager questManager;
    private Quest selectedQuest;
    private string currentGiverId;
    private int currentGiverReputation;
    private QuestPanel currentPanel = QuestPanel.Available;

    private Dictionary<QuestPanel, List<Quest>> panelQuests = new Dictionary<QuestPanel, List<Quest>>();
    #endregion

    #region Unity Methods
    private void Awake()
    {
        questManager = FindObjectOfType<QuestManager>();
        InitializePanelQuests();
        SetupButtonListeners();
    }

    private void Start()
    {
        HideAllPanels();
        SwitchToPanel(QuestPanel.Available);
    }
    #endregion

    #region Public Methods - Quest Giver Interface
    public void OpenQuestGiver(string npcName, string npcId)
    {
        currentGiverId = npcId;
        currentGiverReputation = questManager.GetReputation(npcId);

        // ќбновл€ем UI с информацией о NPC
        textTraderName.text = npcName;
        textTraderReputation.text = $"Reputation: {currentGiverReputation}";

        // √енерируем квесты на основе репутации
        GenerateQuestsBasedOnReputation(npcId, currentGiverReputation);

        // ќбновл€ем все панели
        UpdateAllPanels();

        // ѕоказываем UI
        gameObject.transform.GetChild(0).gameObject.SetActive(true);

        // ѕереключаемс€ на панель Available по умолчанию
        SwitchToPanel(QuestPanel.Available);
    }

    public void CloseQuestGiver()
    {
        gameObject.SetActive(false);
        ClearAllContent();
        selectedQuest = null;
    }
    #endregion

    #region Private Methods - UI Management
    private void InitializePanelQuests()
    {
        panelQuests[QuestPanel.Available] = new List<Quest>();
        panelQuests[QuestPanel.Active] = new List<Quest>();
        panelQuests[QuestPanel.History] = new List<Quest>();
    }

    private void SetupButtonListeners()
    {
        buttonAvailableTab.onClick.AddListener(() => SwitchToPanel(QuestPanel.Available));
        buttonActiveTab.onClick.AddListener(() => SwitchToPanel(QuestPanel.Active));
        buttonHistoryTab.onClick.AddListener(() => SwitchToPanel(QuestPanel.History));

        buttonAvailableAccept.onClick.AddListener(AcceptSelectedQuest);
        buttonActiveComplete.onClick.AddListener(CompleteSelectedQuest);
    }

    private void SwitchToPanel(QuestPanel panel)
    {
        currentPanel = panel;
        UpdateTabAppearance();
        HideAllContentPanels();
        ShowCurrentContentPanel();
        UpdatePanelContent();
        ClearDescriptionPanel();
    }

    private void UpdateTabAppearance()
    {
        // —брасываем все цвета
        buttonAvailableTab.image.color = tabInactiveColor;
        buttonActiveTab.image.color = tabInactiveColor;
        buttonHistoryTab.image.color = tabInactiveColor;

        // ”станавливаем активный цвет дл€ текущей вкладки
        switch (currentPanel)
        {
            case QuestPanel.Available:
                buttonAvailableTab.image.color = tabActiveColor;
                break;
            case QuestPanel.Active:
                buttonActiveTab.image.color = tabActiveColor;
                break;
            case QuestPanel.History:
                buttonHistoryTab.image.color = tabActiveColor;
                break;
        }
    }

    private void HideAllContentPanels()
    {
        panelAvailableContent.SetActive(false);
        panelActiveContent.SetActive(false);
        panelHistoryContent.SetActive(false);

        panelAvailableDescription.SetActive(false);
        panelActiveDescription.SetActive(false);
        panelHistoryDescription.SetActive(false);
    }

    private void ShowCurrentContentPanel()
    {
        switch (currentPanel)
        {
            case QuestPanel.Available:
                panelAvailableContent.SetActive(true);
                break;
            case QuestPanel.Active:
                panelActiveContent.SetActive(true);
                break;
            case QuestPanel.History:
                panelHistoryContent.SetActive(true);
                break;
        }
    }

    private void HideAllPanels()
    {
        panelAvailableContent.SetActive(false);
        panelActiveContent.SetActive(false);
        panelHistoryContent.SetActive(false);
    }

    private void ClearAllContent()
    {
        ClearContentContainer(contentQuestsAvailable);
        ClearContentContainer(contentQuestsActive);
        ClearContentContainer(contentQuestsHistory);
    }

    private void ClearContentContainer(Transform container)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

    private void ClearDescriptionPanel()
    {
        selectedQuest = null;

        textAvailableDescription.text = "Select a quest to view details";
        textAvailableConditions.text = "";
        textAvailableRewards.text = "";
        buttonAvailableAccept.interactable = false;

        textActiveDescription.text = "Select a quest to view details";
        textActiveConditions.text = "";
        textActiveRewards.text = "";
        buttonActiveComplete.interactable = false;

        textHistoryDescription.text = "Select a quest to view details";
        textHistoryConditions.text = "";
        textHistoryRewards.text = "";
    }
    #endregion

    #region Private Methods - Quest Content Management
    private void UpdateAllPanels()
    {
        UpdateAvailableQuests();
        UpdateActiveQuests();
        UpdateHistoryQuests();
    }

    private void UpdatePanelContent()
    {
        switch (currentPanel)
        {
            case QuestPanel.Available:
                UpdateAvailableContent();
                break;
            case QuestPanel.Active:
                UpdateActiveContent();
                break;
            case QuestPanel.History:
                UpdateHistoryContent();
                break;
        }
    }

    private void UpdateAvailableQuests()
    {
        panelQuests[QuestPanel.Available].Clear();

        // ‘ильтруем квесты которые еще не вз€ты и не завершены
        var availableQuestsFiltered = availableQuests.Where(q =>
            !questManager.HasQuest(q.id) ||
            (questManager.HasQuest(q.id) && questManager.IsQuestCompleted(q.id))
        ).ToList();

        panelQuests[QuestPanel.Available].AddRange(availableQuestsFiltered);
    }

    private void UpdateActiveQuests()
    {
        panelQuests[QuestPanel.Active].Clear();
        // «десь нужно получить активные квесты из QuestManager
        var activeQuests = questManager.GetActiveQuests();
        panelQuests[QuestPanel.Active].AddRange(activeQuests
            .Where(q => !q.isCompleted && q.giverNPCId == currentGiverId)
            .ToList());
    }

    private void UpdateHistoryQuests()
    {
        panelQuests[QuestPanel.History].Clear();
        // «десь нужно получить завершенные квесты из QuestManager
        var completedQuests = questManager.GetCompletedQuests();
        panelQuests[QuestPanel.History].AddRange(completedQuests
            .Where(q => q.isCompleted && q.giverNPCId == currentGiverId)
            .ToList());
    }

    private void UpdateAvailableContent()
    {
        ClearContentContainer(contentQuestsAvailable);

        foreach (var quest in panelQuests[QuestPanel.Available])
        {
            CreateQuestButton(quest, contentQuestsAvailable, QuestPanel.Available);
        }
    }

    private void UpdateActiveContent()
    {
        ClearContentContainer(contentQuestsActive);

        foreach (var quest in panelQuests[QuestPanel.Active])
        {
            CreateQuestButton(quest, contentQuestsActive, QuestPanel.Active);
        }
    }

    private void UpdateHistoryContent()
    {
        ClearContentContainer(contentQuestsHistory);

        foreach (var quest in panelQuests[QuestPanel.History])
        {
            CreateQuestButton(quest, contentQuestsHistory, QuestPanel.History);
        }
    }

    private void CreateQuestButton(Quest quest, Transform parent, QuestPanel panel)
    {
        GameObject buttonGO = Instantiate(prefabQuestButton, parent);
        Button questButton = buttonGO.GetComponent<Button>();
        TextMeshProUGUI buttonText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();

        string progressText = quest.GetProgressText();
        string statusText = quest.isCompleted ? " [COMPLETED]" : "";
        buttonText.text = $"{quest.questName}{progressText}{statusText}";

        // »змен€ем цвет в зависимости от статуса
        //if (quest.isCompleted)
        //{
        //    buttonText.color = Color.green;
        //}
        //else if (panel == QuestPanel.Active)
        //{
        //    buttonText.color = Color.yellow;
        //}
        //else
        //{
        //    buttonText.color = Color.white;
        //}

        questButton.onClick.AddListener(() => OnQuestSelected(quest, panel));
    }
    #endregion

    #region Private Methods - Quest Selection and Actions
    private void OnQuestSelected(Quest quest, QuestPanel panel)
    {
        selectedQuest = quest;
        UpdateDescriptionPanel(quest, panel);
    }

    private void UpdateDescriptionPanel(Quest quest, QuestPanel panel)
    {
        string conditions = GetQuestConditionsText(quest);
        string rewards = GetQuestRewardsText(quest);

        switch (panel)
        {
            case QuestPanel.Available:
                panelAvailableDescription.SetActive(true);
                textAvailableDescription.text = quest.description;
                textAvailableConditions.text = conditions;
                textAvailableRewards.text = rewards;
                buttonAvailableAccept.interactable = true;
                break;

            case QuestPanel.Active:
                panelActiveDescription.SetActive(true);
                textActiveDescription.text = quest.description;
                textActiveConditions.text = conditions;
                textActiveRewards.text = rewards;
                buttonActiveComplete.interactable = quest.IsProgressComplete();
                break;

            case QuestPanel.History:
                panelHistoryDescription.SetActive(true);
                textHistoryDescription.text = quest.description;
                textHistoryConditions.text = conditions;
                textHistoryRewards.text = rewards;
                break;
        }
    }

    private string GetQuestConditionsText(Quest quest)
    {
        string conditions = "Conditions:\n";

        switch (quest.questType)
        {
            case QuestType.Kill:
                conditions += $"Х Defeat {quest.necessaryProgress} {quest.targetEnemy}s";
                conditions += $"\nХ Progress: {quest.currentProgress}/{quest.necessaryProgress}";
                break;
            case QuestType.Collect:
                conditions += $"Х Collect {quest.necessaryProgress} {quest.targetItem}s";
                conditions += $"\nХ Progress: {quest.currentProgress}/{quest.necessaryProgress}";
                break;
            case QuestType.Explore:
                conditions += $"Х Explore specific locations";
                break;
            case QuestType.Bring:
                conditions += $"Х Deliver {quest.necessaryProgress} {quest.targetItem}s";
                conditions += $"\nХ Progress: {quest.currentProgress}/{quest.necessaryProgress}";
                break;
        }

        return conditions;
    }

    private string GetQuestRewardsText(Quest quest)
    {
        string rewards = "Rewards:\n";

        if (quest.rewardGold > 0)
            rewards += $"Х Gold: {quest.rewardGold}\n";
        if (quest.rewardExp > 0)
            rewards += $"Х Experience: {quest.rewardExp}\n";
        if (quest.reputationReward > 0)
            rewards += $"Х Reputation: +{quest.reputationReward}";

        return rewards;
    }

    private void AcceptSelectedQuest()
    {
        if (selectedQuest != null)
        {
            questManager.AddQuest(selectedQuest);

            // ”дал€ем прин€тый квест из списка доступных
            availableQuests.Remove(selectedQuest);
            panelQuests[QuestPanel.Available].Remove(selectedQuest);

            UpdateAllPanels();
            UpdatePanelContent();
            ClearDescriptionPanel();
            //Debug.Log($"Quest accepted: {selectedQuest.questName}");
        }
    }

    private void CompleteSelectedQuest()
    {
        if (selectedQuest != null && selectedQuest.IsProgressComplete())
        {
            bool success = questManager.CompleteQuest(selectedQuest.id);
            if (success)
            {
                // ”дал€ем завершенный квест из активных
                panelQuests[QuestPanel.Active].Remove(selectedQuest);

                UpdateAllPanels();
                UpdatePanelContent();
                ClearDescriptionPanel();
            }
        }
    }
    #endregion

    #region Private Methods - Quest Generation
    private void GenerateQuestsBasedOnReputation(string npcId, int reputation)
    {
        availableQuests.Clear();

        // Ѕазовые квесты доступные с репутацией 0
        if (reputation >= 0)
        {
            availableQuests.Add(new Quest(
                "Bandit Extermination",
                "Eliminate the bandits terrorizing the village",
                10,
                GenerateQuestId(npcId, 1),
                QuestType.Kill,
                "Bandit",
                100,
                50,
                15,
                npcId
            ));

            availableQuests.Add(new Quest(
                "Wood Delivery",
                "Collect wood for village construction",
                7,
                GenerateQuestId(npcId, 2),
                QuestType.Collect,
                "Wood",
                75,
                30,
                10,
                npcId
            ));
        }

        //  весты доступные с репутацией 25+
        if (reputation >= 25)
        {
            availableQuests.Add(new Quest(
                "Goblin Hunt",
                "Hunt dangerous goblins in the forest",
                5,
                GenerateQuestId(npcId, 3),
                QuestType.Kill,
                "Goblin",
                150,
                75,
                20,
                npcId
            ));
        }

        //  весты доступные с репутацией 50+
        if (reputation >= 50)
        {
            availableQuests.Add(new Quest(
                "Ancient Artifacts",
                "Collect rare ancient artifacts",
                3,
                GenerateQuestId(npcId, 4),
                QuestType.Collect,
                "AncientArtifact",
                300,
                150,
                30,
                npcId
            ));
        }

        //  весты доступные с репутацией 75+
        if (reputation >= 75)
        {
            availableQuests.Add(new Quest(
                "Dragon Slayer",
                "Slay the ancient dragon",
                1,
                GenerateQuestId(npcId, 5),
                QuestType.Kill,
                "Dragon",
                1000,
                500,
                50,
                npcId
            ));
        }
    }

    private int GenerateQuestId(string npcId, int questNumber)
    {
        return npcId.GetHashCode() + questNumber;
    }
    #endregion
}

public enum QuestPanel
{
    Available,
    Active,
    History
}