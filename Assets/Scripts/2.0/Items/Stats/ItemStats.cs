using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class ItemStats : MonoBehaviour
{
    [Header("Base Stats")]
    public string itemNameKey;
    public List<ItemType> itemTypes;
    public ItemRarity itemRarity;
    public ItemQuality itemQuality = ItemQuality.Normal;

    [Header("Numeric Base Stats")]
    public float weight = 1f;
    public float durability = 100f;
    public float maxDurability = 100f;
    public float price = 100;

    [Header("Requirements Stats")]
    public int requirementStr = 0;
    public int requirementDex = 0;
    public int requirementInt = 0;
    public int requirementChar = 0;

    [Header("Description Settings")]
    [SerializeField] protected List<DescriptionTriple> _descriptionTriples = new List<DescriptionTriple>();
    [SerializeField] private float _doubleClickTime = 0.3f;
    [SerializeField] private GameObject _containerDescriptionPrefab;

    private float _lastClickTime;
    private bool _hasCollider;

    // Свойства только для чтения
    public IReadOnlyList<DescriptionTriple> DescriptionTriples => _descriptionTriples;

    private ButtonsController _buttonsController;
    private GameObject menuDescriptionItem;
    private GameObject itemImage;
    private GameObject itemName;
    private GameObject itemStats;
    private GameObject descriptionsStats;

    protected virtual void Awake()
    {
        CheckCollider();
        InitializeDescriptionTriples();
    }

    protected virtual void CheckCollider()
    {
        _hasCollider = GetComponent<Collider2D>() != null;
        if (!_hasCollider)
        {
            Debug.LogWarning($"ItemStructure on {gameObject.name} requires a 2D Collider for click detection", this);
        }
    }

    protected virtual void Update()
    {
        if (!_hasCollider) return;
        HandleMouseInput();
    }

    public virtual void InitializeQuality()
    {
        float changeQualityStats1;
        float changeQualityStats2;

        switch (itemQuality)
        {
            case ItemQuality.VeryBad:
                changeQualityStats1 = 1.4f;
                changeQualityStats2 = 0.6f;
                break;
            case ItemQuality.Bad:
                changeQualityStats1 = 1.2f;
                changeQualityStats2 = 0.8f;
                break;
            case ItemQuality.Good:
                changeQualityStats1 = 0.8f;
                changeQualityStats2 = 1.2f;
                break;
            case ItemQuality.Excellent:
                changeQualityStats1 = 0.6f;
                changeQualityStats2 = 1.4f;
                break;
            default:
                changeQualityStats1 = 1f;
                changeQualityStats2 = 1f;
                break;
        }

        weight *= changeQualityStats1;
        maxDurability *= changeQualityStats2;
        durability = Mathf.Min(durability, maxDurability);
        price *= changeQualityStats2;
    }

    // Абстрактный метод для инициализации специфичных троек описания
    public abstract void InitializeDescriptionTriples();

    // Виртуальный метод для получения специфичных характеристик
    protected virtual string GetSpecificStatValue(string statKey)
    {
        switch (statKey)
        {
            case "Weight":
                return $"{weight:0.0}";
            case "Durability":
                return $"{durability:0.0}/{maxDurability:0.0}";
            case "Price":
                return $"{price:0}";
            case "Requirements":
                return GetRequirementsDescription();
            case "Type":
                return string.Join(", ", itemTypes);
            case "Rarity":
                return itemRarity.ToString();
            default:
                return "";
        }
    }

    protected virtual string GetRequirementsDescription()
    {
        string result = "";
        if (requirementStr > 0) result += $"Сила: {requirementStr}\n";
        if (requirementDex > 0) result += $"Ловкость: {requirementDex}\n";
        if (requirementInt > 0) result += $"Интеллект: {requirementInt}\n";
        if (requirementChar > 0) result += $"Харизма: {requirementChar}\n";
        return result.TrimEnd('\n');
    }

    // Общие методы для UI (остаются без изменений)
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
        Debug.Log($"Right click on: {gameObject.name}");
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

        itemImage.GetComponent<SpriteRenderer>().sprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        itemName.GetComponent<TextMeshProUGUI>().text = itemNameKey;

        foreach (var descriptionTriple in _descriptionTriples)
        {
            GameObject button = Instantiate(_containerDescriptionPrefab, itemStats.transform);
            AdvancedButtonEvents advancedButtonEvents = button.GetComponent<AdvancedButtonEvents>();
            advancedButtonEvents.ButtonKey = descriptionTriple.NameKey;

            string tempAnswerKey = GetStatValue(descriptionTriple.NameKey);
            string tempDescriptionKey = descriptionTriple.DescriptionKey;

            advancedButtonEvents.ButtonAnswerKey = tempAnswerKey;
            advancedButtonEvents.DescriptionKey = tempDescriptionKey;
            advancedButtonEvents.Initialized();
        }
    }

    private string GetStatValue(string statKey)
    {
        // Сначала проверяем общие статы
        string value = GetSpecificStatValue(statKey);
        if (!string.IsNullOrEmpty(value)) return value;

        // Затем специфичные для типа предмета
        switch (statKey)
        {
            case "Type":
                return string.Join(", ", itemTypes);
            case "Rarity":
                return itemRarity.ToString();
            case "Quality":
                return itemQuality.ToString();
            default:
                return "";
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
    }

    // Методы для работы с тройками описаний
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
}