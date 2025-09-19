using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonsController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject canvasInventory;
    [SerializeField] private KeyCode inventoryKey = KeyCode.I;
    [SerializeField] private bool closeWithSameKey = true;

    [Header("Settings")]
    [SerializeField] private bool pauseGameWhenOpen = true;
    [SerializeField] private bool preventInputWhenOpen = true;

    private bool isInventoryOpen = false;
    private EventSystem eventSystem;
    private InteractionController interactionController;

    private void Awake()
    {
        InitializeComponents();
        ValidateReferences();
    }

    private void InitializeComponents()
    {
        eventSystem = EventSystem.current;
        interactionController = FindObjectOfType<InteractionController>(); // Находим контроллер

        if (eventSystem == null)
        {
            Debug.LogWarning("No EventSystem found in scene.");
        }
    }

    private void ValidateReferences()
    {
        if (canvasInventory == null)
        {
            Debug.LogError("Canvas Inventory reference is not assigned!");
            enabled = false;
            return;
        }

        SetInventoryState(false, true);
    }

    private void Update()
    {
        HandleInventoryInput();
    }

    private void HandleInventoryInput()
    {
        // Проверяем есть ли активные взаимодействия перед открытием инвентаря
        bool canOpenInventory = true;

        if (interactionController != null)
        {
            canOpenInventory = !interactionController.HasAvailableInteractions();
        }

        if (Input.GetKeyDown(inventoryKey) && canOpenInventory)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        SetInventoryState(!isInventoryOpen);
    }

    public void OpenInventory()
    {
        SetInventoryState(true);
    }

    public void CloseInventory()
    {
        SetInventoryState(false);
    }

    public void SetInventoryState(bool isOpen, bool force = false)
    {
        if (canvasInventory == null) return;
        if (isInventoryOpen == isOpen && !force) return;

        isInventoryOpen = isOpen;
        canvasInventory.SetActive(isOpen);

        if (!force)
        {
            HandleGamePause();
            HandleInputBlocking();
        }
    }

    private void HandleGamePause()
    {
        if (pauseGameWhenOpen)
        {
            Time.timeScale = isInventoryOpen ? 0f : 1f;
        }
    }

    private void HandleInputBlocking()
    {
        if (preventInputWhenOpen && eventSystem != null)
        {
            eventSystem.sendNavigationEvents = !isInventoryOpen;
        }
    }

    public bool IsInventoryOpen() => isInventoryOpen;

    private void OnDestroy()
    {
        if (Time.timeScale == 0f && pauseGameWhenOpen)
        {
            Time.timeScale = 1f;
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && isInventoryOpen && pauseGameWhenOpen)
        {
            Time.timeScale = 1f;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Toggle Inventory")]
    private void EditorToggleInventory()
    {
        ToggleInventory();
    }
#endif
}