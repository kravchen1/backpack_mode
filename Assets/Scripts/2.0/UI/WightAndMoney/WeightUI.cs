using UnityEngine;
using TMPro;

public class WeightUI : MonoBehaviour
{
    [Header("Weight UI References")]
    [SerializeField] private TextMeshProUGUI _weightText;
    [SerializeField] private TextMeshProUGUI _maxWeightText;
    [SerializeField] private TextMeshProUGUI _combinedWeightText; // Опционально: общий текст
    [SerializeField] private TextMeshProUGUI _loadCategoryText; // Опционально: текст категории нагрузки

    [Header("Visual Settings")]
    [SerializeField] private Color _lightLoadColor = Color.green;
    [SerializeField] private Color _mediumLoadColor = Color.yellow;
    [SerializeField] private Color _heavyLoadColor = Color.red;
    [SerializeField] private Color _overloadedColor = Color.magenta;

    [Header("Format Settings")]
    [SerializeField] private string _weightFormat = "F1";
    [SerializeField] private string _maxWeightFormat = "F0";
    [SerializeField] private string _separator = " / ";
    [SerializeField] private string _unitSuffix = " kg";

    private PlayerDataManager _dataManager;

    private void Start()
    {
        InitializeWeightUI();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void InitializeWeightUI()
    {
        _dataManager = PlayerDataManager.Instance;

        if (_dataManager == null) return;

        SubscribeToEvents();
        UpdateWeightDisplay();
    }

    private void SubscribeToEvents()
    {
        _dataManager.Stats.OnWeightChanged += OnWeightChanged;
        // Также подписываемся на изменения максимального веса (при улучшении силы)
        _dataManager.Attributes.strength.OnValueChanged += OnStrengthChanged;
    }

    private void UnsubscribeFromEvents()
    {
        if (_dataManager != null)
        {
            if (_dataManager.Stats != null)
                _dataManager.Stats.OnWeightChanged -= OnWeightChanged;

            if (_dataManager.Attributes != null)
                _dataManager.Attributes.strength.OnValueChanged -= OnStrengthChanged;
        }
    }

    private void OnWeightChanged(float currentWeight, float maxWeight)
    {
        UpdateWeightDisplay();
        PlayWeightChangeEffect();
    }

    private void OnStrengthChanged(int newStrength)
    {
        // Максимальный вес изменился при улучшении силы
        UpdateWeightDisplay();
    }

    private void UpdateWeightDisplay()
    {
        if (_dataManager == null) return;

        float currentWeight = _dataManager.Stats.CurrentWeight;
        float maxWeight = _dataManager.Stats.MaxWeight;
        var loadCategory = _dataManager.Stats.GetCurrentLoadCategory();

        // Обновляем отдельные тексты
        if (_weightText != null)
        {
            _weightText.text = currentWeight.ToString(_weightFormat);
            _weightText.color = GetWeightColor(loadCategory);
        }

        if (_maxWeightText != null)
        {
            _maxWeightText.text = maxWeight.ToString(_maxWeightFormat);
        }

        // Обновляем комбинированный текст (если используется)
        if (_combinedWeightText != null)
        {
            _combinedWeightText.text = $"{currentWeight.ToString(_weightFormat)}{_separator}{maxWeight.ToString(_maxWeightFormat)}{_unitSuffix}";
            _combinedWeightText.color = GetWeightColor(loadCategory);
        }

        // Обновляем текст категории нагрузки (если используется)
        if (_loadCategoryText != null)
        {
            _loadCategoryText.text = GetLoadCategoryName(loadCategory);
            _loadCategoryText.color = GetWeightColor(loadCategory);
        }
    }

    private Color GetWeightColor(LoadCategory loadCategory)
    {
        switch (loadCategory)
        {
            case LoadCategory.Light:
                return _lightLoadColor;
            case LoadCategory.Medium:
                return _mediumLoadColor;
            case LoadCategory.Heavy:
                return _heavyLoadColor;
            case LoadCategory.Overloaded:
                return _overloadedColor;
            default:
                return _lightLoadColor;
        }
    }

    private string GetLoadCategoryName(LoadCategory loadCategory)
    {
        switch (loadCategory)
        {
            case LoadCategory.Light:
                return "Light Load";
            case LoadCategory.Medium:
                return "Medium Load";
            case LoadCategory.Heavy:
                return "Heavy Load";
            case LoadCategory.Overloaded:
                return "OVERLOADED!";
            default:
                return "Unknown";
        }
    }

    private void PlayWeightChangeEffect()
    {
        StartCoroutine(WeightChangeAnimation());
    }

    private System.Collections.IEnumerator WeightChangeAnimation()
    {
        if (_weightText == null && _combinedWeightText == null) yield break;

        TextMeshProUGUI targetText = _combinedWeightText ?? _weightText;
        RectTransform rect = targetText.GetComponent<RectTransform>();
        Vector3 originalScale = rect.localScale;

        // Легкая пульсация при изменении веса
        rect.localScale = originalScale * 1.1f;
        yield return new WaitForSeconds(0.1f);
        rect.localScale = originalScale;
    }

    public void RefreshWeight()
    {
        UpdateWeightDisplay();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_weightText == null || _maxWeightText == null || _combinedWeightText == null || _loadCategoryText == null)
        {
            TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in texts)
            {
                if (text.name.Contains("Current") || text.name.Contains("Weight") && !text.name.Contains("Max"))
                    _weightText = text;
                else if (text.name.Contains("Max") || text.name.Contains("Limit"))
                    _maxWeightText = text;
                else if (text.name.Contains("Category") || text.name.Contains("Load"))
                    _loadCategoryText = text;
                else if (text.name.Contains("Combined") || text == GetComponent<TextMeshProUGUI>())
                    _combinedWeightText = text;
            }
        }
    }

    [ContextMenu("Test Light Load")]
    private void TestLightLoad()
    {
        if (_dataManager != null)
        {
            // Устанавливаем вес для легкой нагрузки (менее 30%)
            float targetWeight = _dataManager.Stats.MaxWeight * 0.2f;
            _dataManager.Stats.CurrentWeight = targetWeight;
        }
    }

    [ContextMenu("Test Medium Load")]
    private void TestMediumLoad()
    {
        if (_dataManager != null)
        {
            // Устанавливаем вес для средней нагрузки (30-60%)
            float targetWeight = _dataManager.Stats.MaxWeight * 0.45f;
            _dataManager.Stats.CurrentWeight = targetWeight;
        }
    }

    [ContextMenu("Test Heavy Load")]
    private void TestHeavyLoad()
    {
        if (_dataManager != null)
        {
            // Устанавливаем вес для тяжелой нагрузки (60-90%)
            float targetWeight = _dataManager.Stats.MaxWeight * 0.75f;
            _dataManager.Stats.CurrentWeight = targetWeight;
        }
    }

    [ContextMenu("Test Overloaded")]
    private void TestOverloaded()
    {
        if (_dataManager != null)
        {
            // Устанавливаем вес для перегрузки (более 90%)
            float targetWeight = _dataManager.Stats.MaxWeight * 0.95f;
            _dataManager.Stats.CurrentWeight = targetWeight;
        }
    }
#endif
}