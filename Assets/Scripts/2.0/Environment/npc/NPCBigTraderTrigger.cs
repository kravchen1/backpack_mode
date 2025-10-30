using TMPro;
using UnityEngine;

public class NPCBigTraderTrigger : EnvironmentTrigger
{
    [Header("Trader Settings")]
    public NPC NPCController;
    public TradeController tradeController;
    public float boostTradeStuff = 0f;

    private DayManager dayManager;


    protected override void Start()
    {
        base.Start();
        NPCController = transform.parent.GetComponent<NPC>();
        settingsKey = "NPCTradeTrigger" + NPCController.Config.settingKey;//todo запись каждого объекте в Saver

        // Находим DayManager и подписываемся
        dayManager = FindObjectOfType<DayManager>();
        if (dayManager != null)
        {
            dayManager.OnDayChanged += OnDayChanged;
        }
    }

    //private override void OnDestroy()
    //{
    //    // Отписываемся при уничтожении
    //    if (dayManager != null)
    //    {
    //        dayManager.OnDayChanged -= OnDayChanged;
    //    }
    //}

    private void OnDayChanged()
    {
        // Очищаем сохраненные данные этого торговца
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


    protected override void OnExitChild()
    {
        CloseAllUI();
        tradeController.EndTrade();
    }
}