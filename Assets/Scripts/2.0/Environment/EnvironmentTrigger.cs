using TMPro;
using UnityEngine;

public class EnvironmentTrigger : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] protected string triggerAnimationName = "TriggerColor";
    [SerializeField] protected string resetAnimationName = "Idle";

    [Header("Interaction Settings")]
    [SerializeField] protected bool allowManualInteraction = true;
    [SerializeField] protected KeyCode interactionKey = KeyCode.E;
    [SerializeField] protected string interactionPrompt = "Press E to interact";
    [SerializeField] protected string[] ButtonsKeyTexts;
    [SerializeField] protected GameObject ButtonPrefab;


    protected bool playerInTrigger = false;
    protected Animator animator;
    protected int triggerAnimationHash;

    private InteractionController interactionController;
    private GameObject CanvasUI;

    protected GameObject MenuButtons, MenuContent;
    protected GameObject canvasInventory;
    protected GameObject canvasShop;
    //protected ShopGenerator shopGenerator;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            triggerAnimationHash = Animator.StringToHash(triggerAnimationName);
        }
        else
        {
            Debug.LogWarning($"Animator component not found on {gameObject.name}");
        }
        interactionController = FindObjectOfType<InteractionController>();

        if (animator != null && !string.IsNullOrEmpty(triggerAnimationName))
        {
            // Проверяем существование параметра
            if (!HasAnimationParameter(triggerAnimationName))
            {
                Debug.LogWarning($"Animation parameter '{triggerAnimationName}' not found on {name}");
            }
        }

        CanvasUI = GameObject.Find("CanvasUI");
        canvasInventory = GameObject.Find("CanvasInventory").transform.GetChild(0).gameObject;
        canvasShop = GameObject.Find("CanvasShop").transform.GetChild(0).gameObject;
        //shopGenerator = GameObject.Find("ShopGenerator").GetComponent<ShopGenerator>();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInTrigger = true;
        Debug.Log($"Player entered: {name}");

        // Проигрываем анимацию активации
        PlayTriggerAnimation();

        if (interactionController != null && allowManualInteraction)
        {
            interactionController.RegisterInteraction(this);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInTrigger = false;
        Debug.Log($"Player exited: {name}");

        // Проигрываем анимацию деактивации
        PlayResetAnimation();

        CloseMenuButtons();
        CloseAllUI();
    }

    protected virtual void PlayTriggerAnimation()
    {
        if (animator != null && !string.IsNullOrEmpty(triggerAnimationName))
        {
            animator.Play(triggerAnimationHash);
        }
    }

    protected virtual void PlayResetAnimation()
    {
        if (animator != null)
        {
            // Сбрасываем анимацию через переход в пустое состояние
            animator.Play("Empty", 0, 0f);

            // Сбрасываем параметры аниматора
            animator.Rebind();
            animator.Update(0f);
        }
    }


    
    public void PerformManualInteraction()
    {
        OpenMenuButtons();
        PerformManualInteractionChild();
    }

    public virtual void PerformManualInteractionChild()
    {
        if (!allowManualInteraction || !playerInTrigger || !gameObject.activeInHierarchy)
        {
            Debug.Log($"Interaction blocked - allowed: {allowManualInteraction}, in trigger: {playerInTrigger}, active: {gameObject.activeInHierarchy}");
            return;
        }

        Debug.Log($"Interaction performed: {name}");
    }

    private void OpenMenuButtons()
    {
        if (MenuButtons == null)
        {
            MenuButtons = CanvasUI.transform.GetChild(0).gameObject;
            MenuContent = MenuButtons.transform.GetChild(1).gameObject;
        }
        MenuButtons.SetActive(true);
    }

    protected void CloseMenuButtons()
    {
        if(MenuButtons != null)
        {
            MenuButtons.SetActive(false);

            foreach (Transform child in MenuContent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    protected void CloseAllUI()
    {
        if (canvasInventory.activeSelf)
        {
            canvasInventory.SetActive(false);
        }

        if (canvasShop.activeSelf)
        {
            canvasShop.SetActive(false);
            //shopGenerator.ClearItems();
        }
    }

    public bool IsPlayerInTrigger() => playerInTrigger && gameObject.activeInHierarchy;
    public string GetInteractionPrompt() => interactionPrompt;
    public KeyCode GetInteractionKey() => interactionKey;

    // Проверка существования параметра аниматора
    protected bool HasAnimationParameter(string paramName)
    {
        if (animator == null) return false;

        foreach (var param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }

    protected virtual void OnDestroy()
    {
        if (interactionController != null)
        {
            interactionController.UnregisterInteraction(this);
        }
    }
}