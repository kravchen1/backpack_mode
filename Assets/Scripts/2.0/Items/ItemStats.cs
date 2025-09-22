using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemStats : MonoBehaviour
{
    [Header("BaseStats")]
    public string itemNameKey;
    public List<ItemType> itemTypes;
    public ItemRarity itemRarity;
    public string qualityKey = "Good";

    [Header("NumericBaseStats")]
    public float weight = 1f;
    public float durability = 100f;
    public float maxDurability = 100f;
    public int price = 100;

    [Header("NumericMeleeWeaponStats")]
    public float minDamageMelee = 1f;
    public float maxDamageMelee = 5f;
    public float coolDownMelee = 3f;
    public float baseStaminaMelee = 2f;
    public int accuracyMelee = 75;
    public int critChanceMelee = 10;
    public int critDamageMelee = 150;

    [Header("NumericRangeWeaponStats")]
    public float minDamageRange = 3f;
    public float maxDamageRange = 10f;
    public float coolDownRange = 0.5f;
    public float baseStaminaRange = 0.2f;
    public int accuracyRange = 65;
    public int critChanceRange = 25;
    public int critDamageRange = 220;

    [Header("NumericOtherStats")]
    public int countHeal = 80;


    [Header("DescriptionClick Settings")]
    [SerializeField] private List<DescriptionTriple> _descriptionTriples = new List<DescriptionTriple>();
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
    
    private void Awake()
    {
        CheckCollider();
    }

    private void CheckCollider()
    {
        _hasCollider = GetComponent<Collider2D>() != null;
        if (!_hasCollider)
        {
            Debug.LogWarning($"ItemStructure on {gameObject.name} requires a 2D Collider for click detection", this);
        }
    }

    private void Update()
    {
        if (!_hasCollider) return;

        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        // Проверяем hover над объектом
        if (IsMouseOverObject())
        {
            if (Input.GetMouseButtonDown(0)) // ЛКМ
            {
                HandleLeftClick();
            }
            else if (Input.GetMouseButtonDown(1)) // ПКМ
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
        //Debug.Log($"Left click on: {gameObject.name}");

        // Логика одинарного клика
        //OnLeftClick();

        // Проверка на двойной клик
        float timeSinceLastClick = Time.time - _lastClickTime;

        if (timeSinceLastClick < _doubleClickTime)
        {
            Debug.Log($"Double click on: {gameObject.name}");
            OnDoubleClick();
            _lastClickTime = 0f; // Сбрасываем таймер
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

    // Методы с логикой обработки кликов
    private void OnDoubleClick()
    {
        // Логика двойного клика
        Debug.Log("Double click action executed!");

        //ToggleDetailedInfo();
        ShowContextMenu();
    }

    private void OnRightClick()
    {
        // Логика правого клика
        Debug.Log("Right click action executed!");

        ShowContextMenu();
    }

    // Примеры конкретных действий
    private void ToggleDetailedInfo()
    {

    }

    private void ShowContextMenu()
    {
        if(_buttonsController == null)
        {
            _buttonsController = GameObject.FindFirstObjectByType<ButtonsController>();
            menuDescriptionItem = GameObject.Find("MenuDescriptionItem");
        }

        _buttonsController.OpenMenuDescriptionItem();
        InitializedDescriptionMenu();


        itemImage.GetComponent<SpriteRenderer>().sprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        itemName.GetComponent<TextMeshProUGUI>().text = itemNameKey;
        foreach (var _descriptionTriple in _descriptionTriples)
        {
            GameObject button = Instantiate(_containerDescriptionPrefab, itemStats.transform);
            AdvancedButtonEvents advancedButtonEvents = button.GetComponent<AdvancedButtonEvents>();
            advancedButtonEvents.ButtonKey = _descriptionTriple.NameKey;

            string tempAnswerKey, tempDescriptionKey;

            switch (_descriptionTriple.NameKey)
            {
                case "Type":
                    tempAnswerKey = string.Join(", ", itemTypes);
                    tempDescriptionKey = "";
                    break;
                case "Rarity":
                    tempAnswerKey = itemRarity.ToString();
                    tempDescriptionKey = "";
                    break;
                case "Quality":
                    tempAnswerKey = qualityKey;
                    tempDescriptionKey = "";
                    break;
                //case "Description":
                //    tempAnswerKey = "";
                //    tempDescriptionKey = "";
                //    break;
                case "Weight":
                    tempAnswerKey = $"{weight:0.0}";
                    tempDescriptionKey = "";
                    break;
                case "Durability":
                    tempAnswerKey = $"{durability:0.0}/{maxDurability:0.0}"; //string result = $"{durability:0.##}/{maxDurability:0.##}";
                    tempDescriptionKey = "";
                    break;
                case "AvgDamageMelee":
                    tempAnswerKey = $"{((minDamageMelee + maxDamageMelee) / coolDownMelee):0.0}";
                    tempDescriptionKey = $"(minDamageMelee: {minDamageMelee:0.0} + maxDamageMelee: {maxDamageMelee:0.0}) / coolDownMelee: {coolDownMelee:0.0} = {((minDamageMelee + maxDamageMelee) / coolDownMelee):0.0}";
                    break;
                case "AvgDamageRange":
                    tempAnswerKey = $"{((minDamageRange + maxDamageRange) / coolDownRange):0.0}";
                    tempDescriptionKey = $"(minDamageRange: {minDamageRange:0.0} + maxDamageRange: {maxDamageRange:0.0}) / coolDownRange: {coolDownRange:0.0} = {((minDamageRange + maxDamageRange) / coolDownRange):0.0}";
                    break;
                case "CoolDownMelee":
                    tempAnswerKey = $"{coolDownMelee:0.0}";
                    tempDescriptionKey = "";
                    break;
                case "CoolDownRange":
                    tempAnswerKey = $"{coolDownRange:0.0}";
                    tempDescriptionKey = "";
                    break;
                case "CritChanceMelee":
                    tempAnswerKey = $"{critChanceMelee:0.0}";
                    tempDescriptionKey = "";
                    break;
                case "CritChanceRange":
                    tempAnswerKey = $"{critChanceRange:0.0}";
                    tempDescriptionKey = "";
                    break;
                case "CritDamageMelee":
                    tempAnswerKey = $"{critDamageMelee:0.0}";
                    tempDescriptionKey = "";
                    break;
                case "CritDamageRange":
                    tempAnswerKey = $"{critDamageRange:0.0}";
                    tempDescriptionKey = "";
                    break;
                case "BaseStaminaMelee":
                    tempAnswerKey = $"{baseStaminaMelee:0.0}";
                    tempDescriptionKey = "";
                    break;
                case "BaseStaminaRange":
                    tempAnswerKey = $"{baseStaminaRange:0.0}";
                    tempDescriptionKey = "";
                    break;
                case "AccuracyMelee":
                    tempAnswerKey = $"{accuracyMelee}";
                    tempDescriptionKey = "";
                    break;
                case "AccuracyRange":
                    tempAnswerKey = $"{accuracyRange}";
                    tempDescriptionKey = "";
                    break;
                case "Price":
                    tempAnswerKey = $"{price}";
                    tempDescriptionKey = "";
                    break;
                case "CountHeal":
                    tempAnswerKey = $"{countHeal}";
                    tempDescriptionKey = "";
                    break;
                //case "ActivationConditions":
                //    tempAnswerKey = "";
                //    tempDescriptionKey = "тут описано как ";
                //    break;
                //case "Modifiers":
                //    tempAnswerKey = "";
                //    tempDescriptionKey = "";
                //    break;
                default:
                    tempAnswerKey = _descriptionTriple.AnswerKey;
                    tempDescriptionKey = _descriptionTriple.DescriptionKey;
                    break;
            }

            advancedButtonEvents.ButtonAnswerKey = tempAnswerKey;
            advancedButtonEvents.DescriptionKey = tempDescriptionKey;

            advancedButtonEvents.Initialized();
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


    private void SelectItem()
    {
        // Выделение предмета
        Debug.Log("Item selected!");

        // Можно добавить визуальное выделение, например изменить цвет
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