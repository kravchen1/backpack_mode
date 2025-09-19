using TMPro;
using UnityEngine;

public class ChestTrigger : EnvironmentTrigger
{
    [Header("Chest Settings")]
    [SerializeField] private bool isOpened = false;
    [SerializeField] private GameObject ChestOpen;
    [SerializeField] private GameObject ChestClose;
    [SerializeField] private int countItemsInside = 5;

    protected override void Start()
    {
        base.Start();


        if(isOpened)
        {
            OpenChest();
        }
    }

    public override void PerformManualInteractionChild()
    {
        foreach (var ButtonsKeyText in ButtonsKeyTexts)
        {
            GameObject button = Instantiate(ButtonPrefab, MenuContent.transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = ButtonsKeyText;

            if(ButtonsKeyText == "Open")
            {
                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OpenChest());
            }

            if (ButtonsKeyText == "Destroy")
            {
                button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => DestroyChest());
            }

            //могут быть и другие ключи
        }

    }


    private void OpenChest()
    {
        CloseMenuButtons();
        isOpened = true;
        Debug.Log($"Chest opened: {name}");

        ChestOpen.SetActive(true);
        ChestClose.SetActive(false);

        canvasInventory.SetActive(true);
        canvasShop.SetActive(true);

        //shopGenerator.maxShopItems = countItemsInside;
        //shopGenerator.GenerateItems();

        // Отключаем дальнейшие взаимодействия
        allowManualInteraction = false;
    }

    private void DestroyChest()
    {
        CloseMenuButtons();
        Debug.Log($"Chest try Destroy: {name}");
    }
}