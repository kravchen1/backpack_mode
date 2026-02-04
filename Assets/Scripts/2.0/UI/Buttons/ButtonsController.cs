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

    [Header("Inventory Position Settings")]
    [SerializeField] private Vector2 _inventoryVisiblePosition = Vector2.zero;
    [SerializeField] private Vector2 _inventoryHiddenPosition = new Vector2(5000f, 5000f);
    [SerializeField] private bool _useAlphaTransitionForInventory = false;
    [SerializeField] private float _inventoryTransitionSpeed = 10f;

    [Header("Input Settings")]
    [SerializeField] private KeyCode _inventoryKey = KeyCode.I;
    [SerializeField] private KeyCode _menuCharacterStatsKey = KeyCode.C;

    [Header("Behavior Settings")]
    [SerializeField] private bool _pauseGameWhenOpenInventory = true;
    [SerializeField] private bool _pauseGameWhenOpenMenuDescriptionItem = true;
    [SerializeField] private bool _pauseGameWhenOpenMenuCharacterStats = true;
    [SerializeField] private bool _preventInputWhenOpen = true;

    private bool _isInventoryVisible = true;
    private bool _isMenuDescriptionItemOpen = false;
    private bool _isMenuCharacterStatsOpen = false;

    private RectTransform _inventoryRectTransform;
    private CanvasGroup _inventoryCanvasGroup;
    private EventSystem _eventSystem;
    private InteractionController _interactionController;

    // Public properties for external access
    public bool IsInventoryOpen => _isInventoryVisible; // Теперь указывает видимость
    public bool IsMenuDescriptionItemOpen => _isMenuDescriptionItemOpen;
    public bool IsMenuCharacterStatsOpen => _isMenuCharacterStatsOpen;

    private void Awake()
    {
        InitializeComponents();
        ValidateReferences();
        InitializeUIStates();
        InitializeInventoryComponents();
    }

    private void Start()
    {
        // Инициализация позиции инвентаря
        if (_inventoryRectTransform != null)
        {
            _inventoryRectTransform.anchoredPosition = _inventoryVisiblePosition;
        }
    }

    private void Update()
    {
        HandleInput();
        UpdateInventoryPosition();
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

    private void InitializeInventoryComponents()
    {
        if (_canvasInventory != null)
        {
            _inventoryRectTransform = _canvasInventory.GetComponent<RectTransform>();
            if (_inventoryRectTransform == null)
            {
                Debug.LogError($"{nameof(ButtonsController)}: Canvas Inventory doesn't have RectTransform!");
            }

            _inventoryCanvasGroup = _canvasInventory.GetComponent<CanvasGroup>();
            if (_inventoryCanvasGroup == null && _useAlphaTransitionForInventory)
            {
                _inventoryCanvasGroup = _canvasInventory.AddComponent<CanvasGroup>();
            }
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
        // Инвентарь всегда активен, но может быть невидим
        if (_canvasInventory != null)
        {
            _canvasInventory.SetActive(true);
            _isInventoryVisible = true;
        }

        SetMenuDescriptionItemState(false, true);
        SetCharacterStatsState(false, true);
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(_inventoryKey))
        {
            ToggleInventoryVisibility();
        }

        if (Input.GetKeyDown(_menuCharacterStatsKey))
        {
            ToggleCharacterStats();
        }
    }

    private void UpdateInventoryPosition()
    {
        // Плавное перемещение инвентаря
        if (_inventoryRectTransform != null)
        {
            Vector2 targetPosition = _isInventoryVisible ?
                _inventoryVisiblePosition : _inventoryHiddenPosition;

            _inventoryRectTransform.anchoredPosition = Vector2.Lerp(
                _inventoryRectTransform.anchoredPosition,
                targetPosition,
                _inventoryTransitionSpeed * Time.unscaledDeltaTime
            );
        }

        // Плавное изменение альфа канала
        if (_useAlphaTransitionForInventory && _inventoryCanvasGroup != null)
        {
            float targetAlpha = _isInventoryVisible ? 1f : 0f;
            _inventoryCanvasGroup.alpha = Mathf.Lerp(
                _inventoryCanvasGroup.alpha,
                targetAlpha,
                _inventoryTransitionSpeed * Time.unscaledDeltaTime
            );

            // Отключаем интерактивность когда невидим
            _inventoryCanvasGroup.interactable = _isInventoryVisible;
            _inventoryCanvasGroup.blocksRaycasts = _isInventoryVisible;
        }
    }

    #region Inventory Methods
    public void ToggleInventoryVisibility()
    {
        _isInventoryVisible = !_isInventoryVisible;
        UpdateGameState();
    }

    public void ShowInventory()
    {
        if (!_isInventoryVisible)
        {
            _isInventoryVisible = true;
            UpdateGameState();
        }
    }

    public void HideInventory()
    {
        if (_isInventoryVisible)
        {
            _isInventoryVisible = false;
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

        _canvasMenuDescriptionItem.transform.GetChild(8).gameObject.SetActive(false);
        _canvasMenuDescriptionItem.transform.GetChild(9).gameObject.SetActive(false);
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
        // Инвентарь теперь влияет на паузу через видимость
        bool shouldPause = (_isInventoryVisible && _pauseGameWhenOpenInventory) ||
                          (_isMenuDescriptionItemOpen && _pauseGameWhenOpenMenuDescriptionItem) ||
                          (_isMenuCharacterStatsOpen && _pauseGameWhenOpenMenuCharacterStats);

        Time.timeScale = shouldPause ? 0f : 1f;
    }

    private void UpdateInputState()
    {
        if (_preventInputWhenOpen && _eventSystem != null)
        {
            // Инвентарь блокирует ввод только когда видим
            bool shouldBlockInput = (_isInventoryVisible && _pauseGameWhenOpenInventory) ||
                                   _isMenuDescriptionItemOpen ||
                                   _isMenuCharacterStatsOpen;

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
    [ContextMenu("Toggle Inventory Visibility")]
    private void EditorToggleInventory()
    {
        if (Application.isPlaying)
        {
            ToggleInventoryVisibility();
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