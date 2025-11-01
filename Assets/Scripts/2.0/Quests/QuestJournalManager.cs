using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class QuestJournalManager : MonoBehaviour
{
    #region Fields
    [Header("UI References")]
    [SerializeField] private GameObject panelActiveContent;
    [SerializeField] private GameObject panelHistoryContent;

    [SerializeField] private GameObject panelActiveDescription;
    [SerializeField] private TextMeshProUGUI textActiveDescription;
    [SerializeField] private TextMeshProUGUI textActiveConditions;
    [SerializeField] private TextMeshProUGUI textActiveRewards;

    [SerializeField] private GameObject panelHistoryDescription;
    [SerializeField] private TextMeshProUGUI textHistoryDescription;
    [SerializeField] private TextMeshProUGUI textHistoryConditions;
    [SerializeField] private TextMeshProUGUI textHistoryRewards;

    [SerializeField] private Transform contentQuestsActive;
    [SerializeField] private Transform contentQuestsHistory;

    [SerializeField] private GameObject prefabQuestJournalButton;

    [SerializeField] private Button buttonActiveTab;
    [SerializeField] private Button buttonHistoryTab;

    [Header("Tab Colors")]
    [SerializeField] private Color tabActiveColor = new Color(0.2f, 0.6f, 1f);
    [SerializeField] private Color tabInactiveColor = new Color(0.3f, 0.3f, 0.3f);

    private QuestManager questManager;
    private Quest selectedQuest;
    private QuestPanel currentPanel = QuestPanel.Active;

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
        SwitchToPanel(QuestPanel.Active);
    }

    private void Update()
    {
        // ќбработка открыти€/закрыти€ по клавише J
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleJournal();
        }
    }
    #endregion

    #region Public Methods - Journal Interface
    public void OpenJournal()
    {
        UpdateAllPanels();
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        SwitchToPanel(QuestPanel.Active);
    }

    public void CloseJournal()
    {
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        ClearAllContent();
        selectedQuest = null;
    }

    public void ToggleJournal()
    {
        bool isActive = gameObject.transform.GetChild(0).gameObject.activeSelf;
        if (isActive)
        {
            CloseJournal();
        }
        else
        {
            OpenJournal();
        }
    }
    #endregion

    #region Private Methods - UI Management
    private void InitializePanelQuests()
    {
        panelQuests[QuestPanel.Active] = new List<Quest>();
        panelQuests[QuestPanel.History] = new List<Quest>();
    }

    private void SetupButtonListeners()
    {
        buttonActiveTab.onClick.AddListener(() => SwitchToPanel(QuestPanel.Active));
        buttonHistoryTab.onClick.AddListener(() => SwitchToPanel(QuestPanel.History));
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
        buttonActiveTab.image.color = tabInactiveColor;
        buttonHistoryTab.image.color = tabInactiveColor;

        // ”станавливаем активный цвет дл€ текущей вкладки
        switch (currentPanel)
        {
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
        panelActiveContent.SetActive(false);
        panelHistoryContent.SetActive(false);

        panelActiveDescription.SetActive(false);
        panelHistoryDescription.SetActive(false);
    }

    private void ShowCurrentContentPanel()
    {
        switch (currentPanel)
        {
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
        panelActiveContent.SetActive(false);
        panelHistoryContent.SetActive(false);
    }

    private void ClearAllContent()
    {
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

        textActiveDescription.text = "Select a quest to view details";
        textActiveConditions.text = "";
        textActiveRewards.text = "";

        textHistoryDescription.text = "Select a quest to view details";
        textHistoryConditions.text = "";
        textHistoryRewards.text = "";
    }
    #endregion

    #region Private Methods - Quest Content Management
    private void UpdateAllPanels()
    {
        UpdateActiveQuests();
        UpdateHistoryQuests();
    }

    private void UpdatePanelContent()
    {
        switch (currentPanel)
        {
            case QuestPanel.Active:
                UpdateActiveContent();
                break;
            case QuestPanel.History:
                UpdateHistoryContent();
                break;
        }
    }

    private void UpdateActiveQuests()
    {
        panelQuests[QuestPanel.Active].Clear();
        var activeQuests = questManager.GetActiveQuests();
        panelQuests[QuestPanel.Active].AddRange(activeQuests);
    }

    private void UpdateHistoryQuests()
    {
        panelQuests[QuestPanel.History].Clear();
        var completedQuests = questManager.GetCompletedQuests();
        panelQuests[QuestPanel.History].AddRange(completedQuests);
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
        GameObject buttonGO = Instantiate(prefabQuestJournalButton, parent);
        Button questButton = buttonGO.GetComponent<Button>();

        // ѕолучаем текстовые компоненты из префаба кнопки
        TextMeshProUGUI[] textComponents = buttonGO.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI questGiverText = null;
        TextMeshProUGUI nameQuestText = null;

        foreach (var textComponent in textComponents)
        {
            if (textComponent.name == "QuestGiverText")
                questGiverText = textComponent;
            else if (textComponent.name == "NameQuest")
                nameQuestText = textComponent;
        }

        // ”станавливаем текст дл€ кнопки
        if (nameQuestText != null)
        {
            string progressText = quest.GetProgressText();
            string statusText = quest.isCompleted ? " [COMPLETED]" : "";
            nameQuestText.text = $"{quest.questName}{progressText}{statusText}";
        }

        if (questGiverText != null)
        {
            // ѕолучаем им€ NPC и репутацию
            string npcName = GetNPCName(quest.giverNPCId);
            int reputation = questManager.GetReputation(quest.giverNPCId);
            questGiverText.text = $"{npcName}:{reputation}";
        }

        questButton.onClick.AddListener(() => OnQuestSelected(quest, panel));
    }

    private string GetNPCName(string npcId)
    {
        // «десь должна быть логика получени€ имени NPC по его ID
        // ¬ременно возвращаем ID, можно заменить на словарь или базу данных NPC
        return !string.IsNullOrEmpty(npcId) ? npcId : "Unknown NPC";
    }
    #endregion

    #region Private Methods - Quest Selection
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
            case QuestPanel.Active:
                panelActiveDescription.SetActive(true);
                textActiveDescription.text = quest.description;
                textActiveConditions.text = conditions;
                textActiveRewards.text = rewards;
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
        string conditions = "Conditions:";

        switch (quest.questType)
        {
            case QuestType.Kill:
                conditions += $" Х Defeat {quest.necessaryProgress} {quest.targetEnemy}s";
                conditions += $" Х Progress: {quest.currentProgress}/{quest.necessaryProgress}";
                break;
            case QuestType.Collect:
                conditions += $" Х Collect {quest.necessaryProgress} {quest.targetItem}s";
                conditions += $" Х Progress: {quest.currentProgress}/{quest.necessaryProgress}";
                break;
            case QuestType.Explore:
                conditions += $"Х Explore specific locations";
                break;
            case QuestType.Bring:
                conditions += $" Х Deliver {quest.necessaryProgress} {quest.targetItem}s";
                conditions += $" Х Progress: {quest.currentProgress}/{quest.necessaryProgress}";
                break;
        }

        return conditions;
    }

    private string GetQuestRewardsText(Quest quest)
    {
        string rewards = "Rewards:";

        if (quest.rewardGold > 0)
            rewards += $" Х Gold: {quest.rewardGold}";
        if (quest.rewardExp > 0)
            rewards += $" Х Experience: {quest.rewardExp}";
        if (quest.reputationReward > 0)
            rewards += $" Х Reputation: +{quest.reputationReward}";

        return rewards;
    }
    #endregion
}