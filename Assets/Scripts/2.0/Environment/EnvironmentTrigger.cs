using TMPro;
using UnityEngine;

public class EnvironmentTrigger : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] protected bool allowManualInteraction = true;
    [SerializeField] protected KeyCode interactionKey = KeyCode.E;
    [SerializeField] protected string[] ButtonsKeyTexts;
    [SerializeField] protected GameObject ButtonPrefab;

    [HideInInspector] public string settingsKey;
    public bool isWasActive;

    protected bool playerInTrigger = false;

    private InteractionController interactionController;
    private GameObject CanvasUI;

    protected GameObject menuButtons, menuContent;
    protected GameObject canvasInventory;
    protected GameObject canvasShop;
    protected ShopGenerator shopGenerator;
    protected CellsData shopData;
    protected ButtonsController buttonsController;



    protected virtual void Start()
    {
        interactionController = FindFirstObjectByType<InteractionController>();

        CanvasUI = GameObject.Find("CanvasUI");
        canvasInventory = GameObject.Find("CanvasInventory").transform.GetChild(0).gameObject;
        canvasShop = GameObject.Find("CanvasShop").transform.GetChild(0).gameObject;
        shopGenerator = GameObject.Find("ShopGenerator").GetComponent<ShopGenerator>();
        shopData = canvasShop.transform.GetChild(0).GetComponent<CellsData>();
        buttonsController = GameObject.Find("ButtonsController").GetComponent<ButtonsController>();
        if (menuButtons == null)
        {
            menuButtons = CanvasUI.transform.GetChild(0).gameObject;
            menuContent = menuButtons.transform.GetChild(1).gameObject;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Проверяем, что это именно триггерный коллайдер
        if (!other.isTrigger) return;
        // Проверяем, что это сработал именно наш триггерный коллайдер
        //if (!GetComponent<Collider2D>().isTrigger) return;

        playerInTrigger = true;
        //Debug.Log($"Player entered: {name}");

        if (interactionController != null && allowManualInteraction)
        {
            interactionController.RegisterInteraction(this);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Проверяем, что это именно триггерный коллайдер
        if (!other.isTrigger) return;

        ExitTrigger();
    }

    protected virtual void ExitTrigger()
    {
        playerInTrigger = false;
        Debug.Log($"Player exited: {name}");

        CloseMenuButtons();
        OnExitChild();
    }


    
    public void PerformManualInteraction()
    {
        if (!menuButtons.activeSelf)
        {
            PerformManualInteractionChild();
        }
    }

    protected virtual void PerformManualInteractionChild()
    {
        if (!allowManualInteraction || !playerInTrigger || !gameObject.activeInHierarchy)
        {
            Debug.Log($"Interaction blocked - allowed: {allowManualInteraction}, in trigger: {playerInTrigger}, active: {gameObject.activeInHierarchy}");
            return;
        }

        Debug.Log($"Interaction performed: {name}");
    }


    protected virtual void OnExitChild()
    {
        CloseAllUI();
    }
    protected void OpenMenuButtons()
    {
        menuButtons.SetActive(true);
    }

    protected void CloseMenuButtons()
    {
        if(menuButtons != null)
        {
            menuButtons.SetActive(false);

            foreach (Transform child in menuContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    protected void CloseAllUI()
    {
        if (canvasInventory != null)
        {
            if (canvasInventory.activeSelf)
            {
                buttonsController.CloseInventory();
                //canvasInventory.SetActive(false);
            }
        }
        if (canvasShop != null)
        {
            if (canvasShop.activeSelf)
            {
                canvasShop.SetActive(false);
            }
        }
        DragManager.Instance.isDragActive = true;
        RobManager.Instance.EndRob();
    }

    public bool IsPlayerInTrigger() => playerInTrigger && gameObject.activeInHierarchy;
    public KeyCode GetInteractionKey() => interactionKey;


    protected virtual void OnDestroy()
    {
        if (interactionController != null)
        {
            interactionController.UnregisterInteraction(this);
        }
    }
}