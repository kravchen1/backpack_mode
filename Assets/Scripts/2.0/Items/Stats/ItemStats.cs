using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ItemStats : MonoBehaviour
{
    #region Serialized Fields
    [Header("Base Stats")]
    public string itemKey;
    [HideInInspector] public List<ItemType> itemTypes;
    [HideInInspector] public ItemRarity itemRarity;

    [HideInInspector] public float maxDurability = 100f;
    [HideInInspector] public float basePrice = 100;

    [Header("Icons And Durability Display")]
    [SerializeField] private TextMeshPro _durabilityText;
    [SerializeField] private Gradient _durabilityColorGradient = CreateDefaultGradient();
    public bool isShowDurability = false;

    [Header("Description Settings")]
    [HideInInspector][SerializeField] protected List<DescriptionTriple> _descriptionTriples = new List<DescriptionTriple>();
    [SerializeField] private float _doubleClickTime = 0.3f;
    [SerializeField] private GameObject _containerDescriptionPrefab;
    #endregion

    #region Private Fields
    [HideInInspector] private bool _isUseFight = true;
    [HideInInspector] public float price = 100;

    private float _durability = 100f;
    private SpriteRenderer _isUseFightIcon;
    private float _lastClickTime;
    private bool _hasCollider;

    private ButtonsController _buttonsController;
    private GameObject menuDescriptionItem;
    private GameObject itemImage;
    private GameObject itemName;
    private GameObject itemStats;
    private GameObject descriptionsStats;
    private GameObject buttonUse;
    #endregion

    #region Rarity and Quality Colors
    private static readonly Dictionary<ItemRarity, Color> _rarityColors = new Dictionary<ItemRarity, Color>
    {
        // Common: Не просто серый, а тёплый, с налётом пыли.
        { ItemRarity.Common, new Color(0.55f, 0.55f, 0.55f, 0.5f) },
    
        // Rare: Приглушённый синий, как утреннее небо, а не неон.
        { ItemRarity.Rare, new Color(0.4f, 0.6f, 0.8f, 0.5f) },
    
        // Epic: Глубокий, но мягкий фиолетовый, как увядшие фиалки.
        { ItemRarity.Epic, new Color(0.6f, 0.4f, 0.75f, 0.5f) },
    
        // Legendary: Насыщенный, но не ядовитый золотой. Цвет мёда.
        { ItemRarity.Legendary, new Color(0.9f, 0.75f, 0.2f, 0.5f) },
    
        // Unique: Я изменил его на сложный аквамариновый/изумрудный. Он уникален и отличается от других, не вписываясь в стандартную радугу.
        { ItemRarity.Unique, new Color(0.3f, 0.7f, 0.6f, 0.5f) }
    };
    #endregion

    #region Properties
    public float durability
    {
        get => _durability;
        set
        {
            float oldValue = _durability;
            _durability = Mathf.Clamp(value, 0f, maxDurability);

            if (!Mathf.Approximately(oldValue, _durability))
            {
                UpdateDurabilityDisplay();
            }
        }
    }

    public IReadOnlyList<DescriptionTriple> DescriptionTriples => _descriptionTriples;
    #endregion

    #region Unity Lifecycle
    protected virtual void Awake()
    {
        LoadFromDataManager();
        CheckCollider();
        InitializeDescriptionTriples();
        InitializeUIComponents();
        InitializePrice();
        UpdateDurabilityDisplay();
    }

    protected virtual void Update()
    {
        if (!_hasCollider) return;
        HandleMouseInput();
    }
    #endregion

    #region Initialization Methods
    public void Initialized()
    {
        InitializeIsUseFightAndDurability();
    }

    public void InitializeIsUseFightAndDurability()
    {
        UpdateDurabilityDisplay();
    }

    public abstract void InitializeDescriptionTriples();
    #endregion

    #region UI Methods
    private void InitializeUIComponents()
    {
        if (_durabilityText == null)
        {
            _durabilityText = transform.Find("InfoText")?.GetComponent<TextMeshPro>();
        }
        if (_isUseFightIcon == null)
        {
            _isUseFightIcon = transform.Find("IsUseFight")?.GetComponent<SpriteRenderer>();
        }
    }

    private void InitializePrice()
    {
        if (GetComponent<ItemMove>().StackCount > 1)
        {
            price = basePrice * GetComponent<ItemMove>().StackCount;
        }
        else
        {
            price = basePrice;
        }
    }

    private void UpdateDurabilityDisplay()
    {
        if (_durabilityText == null || !isShowDurability) return;

        _durabilityText.text = $"{durability:0}/{maxDurability:0}";
        float durabilityRatio = _durability / maxDurability;
        Color newColor = _durabilityColorGradient.Evaluate(durabilityRatio);
        _durabilityText.color = newColor;
    }
    #endregion

    #region Input Handling
    private void HandleMouseInput()
    {
        if (IsMouseOverObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleLeftClick();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                HandleRightClick();
            }
        }
    }

    private bool IsMouseOverObject()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePosition);
        return hit != null && hit.gameObject == gameObject;
    }

    private void HandleLeftClick()
    {
        float timeSinceLastClick = Time.time - _lastClickTime;

        if (timeSinceLastClick < _doubleClickTime)
        {
            Debug.Log($"Double click on: {gameObject.name}");
            OnDoubleClick();
            _lastClickTime = 0f;
        }
        else
        {
            _lastClickTime = Time.time;
        }
    }

    private void HandleRightClick()
    {
        //Debug.Log($"Right click on: {gameObject.name}");
        OnRightClick();
    }

    private void OnDoubleClick()
    {
        ShowContextMenu();
    }

    private void OnRightClick()
    {
        ShowContextMenu();
    }
    #endregion

    #region Context Menu
    private void ShowContextMenu()
    {
        if (_buttonsController == null)
        {
            _buttonsController = GameObject.FindFirstObjectByType<ButtonsController>();
            menuDescriptionItem = GameObject.Find("MenuDescriptionItem");
        }

        InitializeDescriptionTriples();
        _buttonsController.OpenMenuDescriptionItem();
        InitializedDescriptionMenu();
        SetupContextMenuUI();
        PopulateDescriptionTriples();
        SetupContextMenuButtons();
    }

    private void SetupContextMenuUI()
    {
        itemImage.GetComponent<SpriteRenderer>().sprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        itemName.GetComponent<TextMeshProUGUI>().text = itemKey;
    }

    private void PopulateDescriptionTriples()
    {
        foreach (var descriptionTriple in _descriptionTriples)
        {
            GameObject button = Instantiate(_containerDescriptionPrefab, itemStats.transform);
            AdvancedButtonEvents advancedButtonEvents = button.GetComponent<AdvancedButtonEvents>();
            advancedButtonEvents.ButtonKey = descriptionTriple.NameKey;

            string tempAnswerKey = GetStatValue(descriptionTriple.NameKey);
            string tempDescriptionKey = descriptionTriple.DescriptionKey;

            // Новый функционал: для Description берем описание из DataManager
            if (descriptionTriple.NameKey == "Description")
            {
                tempDescriptionKey = GetDescriptionFromDataManager();
                tempAnswerKey = ""; // Оставляем пустым для Description
            }

            advancedButtonEvents.ButtonAnswerKey = tempAnswerKey;
            advancedButtonEvents.DescriptionKey = tempDescriptionKey;
            advancedButtonEvents.Initialized();
        }
    }

    private void SetupContextMenuButtons()
    {
        buttonUse.SetActive(true);
        var buttonComponent = buttonUse?.GetComponent<UnityEngine.UI.Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(() => UseNotFight());
        }
    }

    private void InitializedDescriptionMenu()
    {
        if (menuDescriptionItem == null)
        {
            menuDescriptionItem = GameObject.Find("MenuDescriptionItem");
        }
        if (itemImage == null)
        {
            itemImage = menuDescriptionItem.transform.GetChild(2).gameObject;
        }
        if (itemName == null)
        {
            itemName = menuDescriptionItem.transform.GetChild(3).gameObject;
        }
        if (itemStats == null)
        {
            itemStats = menuDescriptionItem.transform.GetChild(4).gameObject;
        }
        if (buttonUse == null)
        {
            buttonUse = menuDescriptionItem.transform.GetChild(9).gameObject;
        }
    }
    #endregion

    #region Utility Methods
    protected virtual void CheckCollider()
    {
        _hasCollider = GetComponent<Collider2D>() != null;
        if (!_hasCollider)
        {
            Debug.LogWarning($"ItemStructure on {gameObject.name} requires a 2D Collider for click detection", this);
        }
    }

    private static Gradient CreateDefaultGradient()
    {
        Gradient gradient = new Gradient();
        gradient.colorKeys = new GradientColorKey[]
        {
                new GradientColorKey(Color.red, 0f),
                new GradientColorKey(Color.yellow, 0.3f),
                new GradientColorKey(Color.gray, 0.7f),
                new GradientColorKey(Color.black, 1f)
        };
        return gradient;
    }

    protected virtual string GetSpecificStatValue(string statKey)
    {
        switch (statKey)
        {
            case "Description":
                return $"{itemKey}";
            case "Durability":
                return $"{durability:0.0}/{maxDurability:0.0}";
            case "Price":
                return $"{price:0}";
            case "Type":
                return string.Join(", ", itemTypes);
            case "Rarity":
                return itemRarity.ToString();
            default:
                return "";
        }
    }

    private string GetStatValue(string statKey)
    {
        string value = GetSpecificStatValue(statKey);
        if (!string.IsNullOrEmpty(value)) return value;

        switch (statKey)
        {
            case "Type":
                return string.Join(", ", itemTypes);
            case "Rarity":
                return itemRarity.ToString();
            default:
                return "";
        }
    }

    private string GetDescriptionFromDataManager()
    {
        if (string.IsNullOrEmpty(itemKey)) return "";

        var dataManager = ItemDataManager.Instance;
        if (dataManager == null) return "";

        return dataManager.GetItemData(itemKey, "description", "");
    }
    #endregion

    #region Public Methods
    public void UseNotFight()
    {
        // Implement use in non-fight context
    }

    public void AddDescriptionTriple(string nameKey, string answerKey, string descriptionKey)
    {
        _descriptionTriples.Add(new DescriptionTriple(nameKey, answerKey, descriptionKey));
    }

    public void RemoveDescriptionTripleAt(int index)
    {
        if (index >= 0 && index < _descriptionTriples.Count)
        {
            _descriptionTriples.RemoveAt(index);
        }
    }

    public void ClearDescriptionTriples()
    {
        _descriptionTriples.Clear();
    }

    public DescriptionTriple GetDescriptionTriple(int index)
    {
        if (index >= 0 && index < _descriptionTriples.Count)
            return _descriptionTriples[index];
        return null;
    }

    public bool TryGetDescriptionTriple(int index, out DescriptionTriple result)
    {
        if (index >= 0 && index < _descriptionTriples.Count)
        {
            result = _descriptionTriples[index];
            return true;
        }
        result = null;
        return false;
    }

    public void ApplyDamageDurability(float damage)
    {
        durability -= damage;
    }

    public void Repair(float repairAmount)
    {
        durability += repairAmount;
    }

    public void RepairFull()
    {
        durability = maxDurability;
    }

    public void SetDurability(float newDurability)
    {
        durability = newDurability;
    }
    #endregion

    #region jsonData
    protected virtual void LoadFromDataManager()
    {
        if (string.IsNullOrEmpty(itemKey)) return;

        var dataManager = ItemDataManager.Instance;
        if (dataManager == null) return;

        // Загрузка базовых параметров
        maxDurability = dataManager.GetItemData(itemKey, "maxDurability", maxDurability);
        basePrice = dataManager.GetItemData(itemKey, "basePrice", basePrice);

        // Загрузка enum значений
        string rarityStr = dataManager.GetItemData<string>(itemKey, "itemRarity", "");
        if (Enum.TryParse<ItemRarity>(rarityStr, out var rarity))
            itemRarity = rarity;

        // Загрузка списка типов
        string typesStr = dataManager.GetItemData<string>(itemKey, "itemTypes", "");
        itemTypes = ParseItemTypes(typesStr);
    }

    private List<ItemType> ParseItemTypes(string typesStr)
    {
        var types = new List<ItemType>();
        if (string.IsNullOrEmpty(typesStr)) return types;

        foreach (var typeStr in typesStr.Split(','))
        {
            if (Enum.TryParse<ItemType>(typeStr.Trim(), out var type))
                types.Add(type);
        }
        return types;
    }

    // Добавь свойство для локализованного имени
    public string LocalizedName =>
        ItemDataManager.Instance?.GetLocalizedString(itemKey, "name") ?? itemKey;

    public string LocalizedDescription =>
        ItemDataManager.Instance?.GetLocalizedString(itemKey, "description") ?? "";
    #endregion
}