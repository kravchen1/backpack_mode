using TMPro;
using UnityEngine;

public class ChestDeathTrigger : EnvironmentTrigger
{
    [Header("Chest Settings")]
    [HideInInspector] public bool isOpened = false;
    [SerializeField] private GameObject ChestOpen;
    [SerializeField] private GameObject ChestClose;


    private TimedDestroyer timedDestroyer;

    protected override void Start()
    {
        base.Start();
        timedDestroyer = gameObject.AddComponent<TimedDestroyer>();
        // Запускаем таймер при создании сундука
        timedDestroyer.StartDestroyCountdown();
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
                    case "Open":
                        button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OpenChest());
                        break;
                    case "Destroy":
                        button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(DestroyChest);
                        break;
                    default:
                        //могут быть и другие ключи
                        break;
                }
            }
        }
    }

    private void OpenChest()
    {
        CloseMenuButtons();
        isOpened = true;
        Debug.Log($"Chest opened: {name}");

        // Отменяем таймер удаления при открытии
        timedDestroyer.CancelDestroy();

        ChestOpen.SetActive(true);
        ChestClose.SetActive(false);

        buttonsController.OpenInventory();
        canvasShop.SetActive(true);

        shopData.settingsKey = settingsKey;
        shopData.LoadData();
    }

    protected override void OnExitChild()
    {
        if (isOpened)
        {
            isOpened = false;
            shopData.SaveData();

            // Перезапускаем таймер удаления при закрытии сундука
            timedDestroyer.StartDestroyCountdown();
        }
        CloseAllUI();
    }

    private void DestroyChest()
    {
        CloseMenuButtons();
        Debug.Log($"Chest try Destroy: {name}");
    }
}