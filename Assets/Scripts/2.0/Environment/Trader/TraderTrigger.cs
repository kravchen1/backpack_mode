using TMPro;
using UnityEngine;

public class TraderTrigger : EnvironmentTrigger
{
    [Header("Trader Settings")]
    private bool isOpened = false;
    public int countItemsInside = 5;



    protected override void Start()
    {
        base.Start();
        settingsKey = "traderData" + gameObject.name;//todo запись каждого объекте в Saver

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
        isWasActive = true;
        isOpened = true;
        Debug.Log($"Chest opened: {name}");

       // ChestOpen.SetActive(true);
        //ChestClose.SetActive(false);

        buttonsController.OpenInventory();
        canvasShop.SetActive(true);

        if (string.IsNullOrEmpty(PlayerPrefs.GetString(settingsKey, "")))
        {
            shopGenerator.maxShopItems = countItemsInside;
            shopGenerator.GenerateItems(0f);
            shopData.settingsKey = settingsKey;
            shopData.SaveData();
        }
        else
        {
            shopData.settingsKey = settingsKey;
            shopData.LoadData();
        }


        // Отключаем дальнейшие взаимодействия
        //allowManualInteraction = false;
    }


    protected override void OnExitChild()
    {
        if (isOpened)
        {
            isOpened = false;
            shopData.SaveData();
        }
        CloseAllUI();
    }
    private void DestroyChest()
    {
        CloseMenuButtons();
        Debug.Log($"Chest try Destroy: {name}");
    }
}