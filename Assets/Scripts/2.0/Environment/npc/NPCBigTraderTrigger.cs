using TMPro;
using UnityEngine;

public class NPCBigTraderTrigger : EnvironmentTrigger
{
    [Header("Trader Settings")]
    public NPC NPCController;
    public TradeController tradeController;
    public float boostTradeStuff = 0f;

    private DayManager dayManager;
    private QuestGiverManager questGiverManager;
    private QuestManager questManager;

    protected override void Start()
    {
        base.Start();
        NPCController = transform.parent.GetComponent<NPC>();
        settingsKey = "NPCTradeTrigger" + NPCController.Config.settingKey;

        // Находим менеджеры
        dayManager = FindFirstObjectByType<DayManager>();
        questGiverManager = FindFirstObjectByType<QuestGiverManager>();
        questManager = FindFirstObjectByType<QuestManager>();
        if (dayManager != null)
        {
            dayManager.OnDayChanged += OnDayChanged;
        }
    }

    private void OnDayChanged()
    {
        string traderKey = "NPCTradeTrigger" + NPCController.Config.settingKey;
        if (PlayerPrefs.HasKey(traderKey))
        {
            PlayerPrefs.DeleteKey(traderKey);
            PlayerPrefs.Save();
            Debug.Log($"Ассортимент торговца {NPCController.Config.settingKey} обновлен due to new day");
        }
    }

    protected override void PerformManualInteractionChild()
    {
        OpenMenuButtons();
        foreach (var buttonsKeyText in ButtonsKeyTexts)
        {
            GameObject button = Instantiate(ButtonPrefab, menuContent.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = buttonsKeyText;

            if (buttonsKeyText != null)
            {
                button.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();

                switch (buttonsKeyText)
                {
                    case "Trade":
                        button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => Trade());
                        break;
                    case "Quests":
                        button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => Quests());
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void Trade()
    {
        CloseMenuButtons();
        tradeController.StartTrade(boostTradeStuff, settingsKey);
    }

    private void Quests()
    {
        CloseMenuButtons();

        if (questGiverManager != null)
        {
            // Получаем текущую репутацию с этим NPC
            int currentReputation = questManager?.GetReputation(NPCController.Config.settingKey) ?? 0;

            questGiverManager.OpenQuestGiver("Merchant John", "merchant_john_01");
        }
        else
        {
            Debug.LogError("QuestGiverManager not found!");
        }
    }

    private void OnReputationIncreased(string npcId, int amount)
    {
        // Обработка увеличения репутации
        if (questManager != null)
        {
            questManager.IncreaseReputation(npcId, amount);
            Debug.Log($"Reputation with {npcId} increased by {amount}");
        }
    }

    protected override void OnExitChild()
    {
        CloseAllUI();
        tradeController.EndTrade();

        // Закрываем меню квестов если открыто
        if (questGiverManager != null)
        {
            //questGiverManager.CloseQuestMenu();
        }
    }
}