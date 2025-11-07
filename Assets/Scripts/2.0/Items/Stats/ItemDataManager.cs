using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ItemDataManager : MonoBehaviour
{
    [System.Serializable]
    public class LocalizationData
    {
        public List<ItemCategory> items;
    }

    [System.Serializable]
    public class ItemCategory
    {
        public string category;
        public List<ItemData> items;
    }

    [System.Serializable]
    public class ItemData
    {
        // Базовые поля
        public string itemKey;
        public string name;
        public string description;
        public string itemTypes;
        public string itemRarity;
        public string itemQuality;
        public string weight;
        public string maxDurability;
        public string basePrice;

        // Поля для оружия
        public string minDamageMelee;
        public string maxDamageMelee;
        public string coolDownMelee;
        public string baseStaminaMelee;
        public string accuracyMelee;
        public string critChanceMelee;
        public string critDamageMelee;
        public string minDamageRange;
        public string maxDamageRange;
        public string coolDownRange;
        public string baseStaminaRange;
        public string accuracyRange;
        public string critChanceRange;
        public string critDamageRange;

        // Поля для брони
        public string damageConsumptionPerDurability;

        // Поля для фонариков
        public string _flashLightRadius;
        public string _flashLightIntensity;

        // Поля для часов
        public string isShowTime;
        public string isShowDate;

        // Поля для патронов
        public string damageModifier;
        public string accuracyModifier;
        public string critChanceModifier;
        public string critDamageModifire;

        // Поля для модификаторов оружия
        public string damageRangeModifierPercentage;
        public string accuracyRangeModifierPercentage;
        public string critChanceRangeModifierPercentage;
        public string critDamageRangeModifierPercentage;
        public string coolDownRangeModifierPercentage;
        public string staminaRangeModifierPercentage;
        public string damageRangeModifierValue;
        public string accuracyRangeModifierValue;
        public string critChanceRangeModifierValue;
        public string coolDownRangeModifierValue;
        public string staminaRangeModifierValue;
        public string critDamageRangeModifierValue;
        public string damageMeleeModifierPercentage;
        public string accuracyMeleeModifierPercentage;
        public string critChanceMeleeModifierPercentage;
        public string critDamageMeleeModifierPercentage;
        public string coolDownMeleeModifierPercentage;
        public string staminaMeleeModifierPercentage;
        public string damageMeleeModifierValue;
        public string accuracyMeleeModifierValue;
        public string critChanceMeleeModifierValue;
        public string coolDownMeleeModifierValue;
        public string staminaMeleeModifierValue;
        public string critDamageMeleeModifierValue;
    }

    private static ItemDataManager _instance;
    private Dictionary<string, ItemData> _itemData = new Dictionary<string, ItemData>();

    public static ItemDataManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAllItems();
    }

    private void LoadAllItems()
    {
        string language = PlayerPrefs.GetString("localization_language", "ru");
        TextAsset jsonFile = Resources.Load<TextAsset>($"Localization/Items_{language}");

        if (jsonFile != null)
        {
            var localizationData = JsonUtility.FromJson<LocalizationData>(jsonFile.text);

            if (localizationData?.items != null)
            {
                ProcessItemData(localizationData.items);
                Debug.Log($"Successfully loaded {_itemData.Count} items from {localizationData.items.Count} categories");
            }
            else
            {
                Debug.LogError("Failed to deserialize JSON structure");
            }
        }
        else
        {
            Debug.LogWarning($"Items file for language '{language}' not found in Resources/Localization/");
        }
    }

    private void ProcessItemData(List<ItemCategory> categories)
    {
        _itemData.Clear();

        foreach (var category in categories)
        {
            if (category?.items != null)
            {
                foreach (var item in category.items)
                {
                    if (!string.IsNullOrEmpty(item?.itemKey))
                    {
                        _itemData[item.itemKey] = item;
                    }
                }
            }
        }
    }

    public T GetItemData<T>(string itemKey, string fieldName, T defaultValue = default(T))
    {
        if (!_itemData.ContainsKey(itemKey))
            return defaultValue;

        var item = _itemData[itemKey];
        var field = typeof(ItemData).GetField(fieldName);

        if (field == null)
            return defaultValue;

        var value = field.GetValue(item);

        // Если ожидаемый тип - string, а значение не string, конвертируем в string
        if (typeof(T) == typeof(string))
        {
            var stringValue = value?.ToString();
            if (string.IsNullOrEmpty(stringValue))
                return defaultValue;
            return ConvertValue<T>(stringValue, defaultValue);
        }

        // Если типы совпадают, возвращаем как есть
        if (value is T typedValue)
            return typedValue;

        // Иначе пытаемся конвертировать через строковое представление
        var stringRepresentation = value?.ToString();
        if (string.IsNullOrEmpty(stringRepresentation))
            return defaultValue;

        return ConvertValue<T>(stringRepresentation, defaultValue);
    }

    private T ConvertValue<T>(string value, T defaultValue)
    {
        try
        {
            if (typeof(T) == typeof(string))
                return (T)(object)value;
            if (typeof(T) == typeof(int))
                return (T)(object)int.Parse(value);
            if (typeof(T) == typeof(float))
                return (T)(object)float.Parse(value);
            if (typeof(T) == typeof(bool))
            {
                // Поддержка разных форматов bool
                if (value.ToLower() == "true" || value == "1")
                    return (T)(object)true;
                if (value.ToLower() == "false" || value == "0")
                    return (T)(object)false;
                return defaultValue;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to convert value '{value}' to type {typeof(T)}: {e.Message}");
        }

        return defaultValue;
    }

    public string GetLocalizedString(string itemKey, string fieldName)
    {
        return GetItemData<string>(itemKey, fieldName, "");
    }

    public bool ItemExists(string itemKey) => _itemData.ContainsKey(itemKey);

    public ItemData GetFullItemData(string itemKey)
    {
        return _itemData.ContainsKey(itemKey) ? _itemData[itemKey] : null;
    }

    // Метод для перезагрузки при смене языка
    public void ReloadWithLanguage(string newLanguage)
    {
        PlayerPrefs.SetString("localization_language", newLanguage);
        LoadAllItems();
    }

    // Метод для отладки - получить все загруженные ключи
    public List<string> GetAllItemKeys()
    {
        return new List<string>(_itemData.Keys);
    }
}