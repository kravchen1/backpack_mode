using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Controller for managing UI panel states (inventory, item description, character stats)
/// with game pause and input blocking functionality
/// </summary>
public class ButtonsController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _canvasInventory;
    [SerializeField] private GameObject _canvasMenuDescriptionItem;
    [SerializeField] private GameObject _canvasMenuCharacterStats;

    [Header("Input Settings")]
    [SerializeField] private KeyCode _inventoryKey = KeyCode.I;
    [SerializeField] private KeyCode _menuCharacterStatsKey = KeyCode.C;

    [Header("Behavior Settings")]
    [SerializeField] private bool _pauseGameWhenOpenInventory = true;
    [SerializeField] private bool _pauseGameWhenOpenMenuDescriptionItem = true;
    [SerializeField] private bool _pauseGameWhenOpenMenuCharacterStats = true;
    [SerializeField] private bool _preventInputWhenOpen = true;

    private bool _isInventoryOpen = false;
    private bool _isMenuDescriptionItemOpen = false;
    private bool _isMenuCharacterStatsOpen = false;

    private EventSystem _eventSystem;
    private InteractionController _interactionController;

    // Public properties for external access
    public bool IsInventoryOpen => _isInventoryOpen;
    public bool IsMenuDescriptionItemOpen => _isMenuDescriptionItemOpen;
    public bool IsMenuCharacterStatsOpen => _isMenuCharacterStatsOpen;

    private void Awake()
    {
        InitializeComponents();
        ValidateReferences();
        InitializeUIStates();
    }

    private void Update()
    {
        HandleInput();
    }

    private void InitializeComponents()
    {
        _eventSystem = EventSystem.current;
        _interactionController = FindObjectOfType<InteractionController>();

        if (_eventSystem == null)
        {
            Debug.LogWarning($"{nameof(ButtonsController)}: No EventSystem found in scene.");
        }
    }

    private void ValidateReferences()
    {
        if (_canvasInventory == null)
        {
            Debug.LogError($"{nameof(ButtonsController)}: Canvas Inventory reference is not assigned!");
            enabled = false;
            return;
        }
    }

    private void InitializeUIStates()
    {
        SetInventoryState(false, true);
        SetMenuDescriptionItemState(false, true);
        SetCharacterStatsState(false, true);
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(_inventoryKey))
        {
            ToggleInventory();
        }

        if (Input.GetKeyDown(_menuCharacterStatsKey))
        {
            ToggleCharacterStats();
        }
    }

    #region Inventory Methods
    public void ToggleInventory()
    {
        SetInventoryState(!_isInventoryOpen);
    }

    public void OpenInventory()
    {
        if(_isInventoryOpen)
        {
            CloseInventory();
        }
        SetInventoryState(true);
    }

    public void CloseInventory()
    {
        SetInventoryState(false);
    }

    private void SetInventoryState(bool isOpen, bool force = false)
    {
        if (_canvasInventory == null) return;
        if (_isInventoryOpen == isOpen && !force) return;

        _isInventoryOpen = isOpen;
        _canvasInventory.SetActive(isOpen);

        if (!force)
        {
            UpdateGameState();
        }
    }
    #endregion

    #region Menu Description Item Methods
    public void OpenMenuDescriptionItem()
    {
        SetMenuDescriptionItemState(true);
    }

    public void CloseMenuDescriptionItem()
    {
        SetMenuDescriptionItemState(false);
    }

    private void SetMenuDescriptionItemState(bool isOpen, bool force = false)
    {
        if (_canvasMenuDescriptionItem == null) return;
        if (_isMenuDescriptionItemOpen == isOpen && !force) return;

        if (!isOpen)
        {
            CleanupMenuDescriptionItem();
        }

        _isMenuDescriptionItemOpen = isOpen;
        _canvasMenuDescriptionItem.SetActive(isOpen);

        if (!force)
        {
            UpdateGameState();
        }
    }

    private void CleanupMenuDescriptionItem()
    {
        if (_canvasMenuDescriptionItem == null) return;

        Transform itemStats = _canvasMenuDescriptionItem.transform.GetChild(4);
        TextMeshProUGUI descriptionsStats = _canvasMenuDescriptionItem.transform.GetChild(5).GetComponent<TextMeshProUGUI>();

        if (descriptionsStats != null)
        {
            descriptionsStats.text = string.Empty;
        }

        // Destroy all child objects of itemStats
        for (int i = itemStats.childCount - 1; i >= 0; i--)
        {
            Destroy(itemStats.GetChild(i).gameObject);
        }
    }
    #endregion

    #region Character Stats Methods
    public void ToggleCharacterStats()
    {
        SetCharacterStatsState(!_isMenuCharacterStatsOpen);
    }

    public void OpenCharacterStats()
    {
        SetCharacterStatsState(true);
    }

    public void CloseCharacterStats()
    {
        SetCharacterStatsState(false);
    }

    private void SetCharacterStatsState(bool isOpen, bool force = false)
    {
        if (_canvasMenuCharacterStats == null) return;
        if (_isMenuCharacterStatsOpen == isOpen && !force) return;

        _isMenuCharacterStatsOpen = isOpen;
        _canvasMenuCharacterStats.SetActive(isOpen);

        if (!force)
        {
            UpdateGameState();
        }
    }
    #endregion

    private void UpdateGameState()
    {
        UpdateTimeScale();
        UpdateInputState();
    }

    private void UpdateTimeScale()
    {
        bool shouldPause = (_isInventoryOpen && _pauseGameWhenOpenInventory) ||
                          (_isMenuDescriptionItemOpen && _pauseGameWhenOpenMenuDescriptionItem) ||
                          (_isMenuCharacterStatsOpen && _pauseGameWhenOpenMenuCharacterStats);

        Time.timeScale = shouldPause ? 0f : 1f;
    }

    private void UpdateInputState()
    {
        if (_preventInputWhenOpen && _eventSystem != null)
        {
            bool shouldBlockInput = _isInventoryOpen || _isMenuDescriptionItemOpen || _isMenuCharacterStatsOpen;
            _eventSystem.sendNavigationEvents = !shouldBlockInput;
        }
    }

    private void OnDestroy()
    {
        // Ensure time scale is reset when object is destroyed
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // Resume game if focus is lost while UI is open
        if (!hasFocus && Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Toggle Inventory")]
    private void EditorToggleInventory()
    {
        if (Application.isPlaying)
        {
            ToggleInventory();
        }
    }

    // Editor validation
    private void OnValidate()
    {
        _inventoryKey = _inventoryKey == KeyCode.None ? KeyCode.I : _inventoryKey;
        _menuCharacterStatsKey = _menuCharacterStatsKey == KeyCode.None ? KeyCode.C : _menuCharacterStatsKey;
    }
#endif
}