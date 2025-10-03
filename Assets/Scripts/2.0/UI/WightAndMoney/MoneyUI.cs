using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [Header("Money UI References")]
    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private TextMeshProUGUI _moneyChangeText; // Опционально: текст изменения

    [Header("Visual Settings")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _positiveChangeColor = Color.green;
    [SerializeField] private Color _negativeChangeColor = Color.red;
    [SerializeField] private float _changeDisplayDuration = 2f;

    [Header("Format Settings")]
    [SerializeField] private string _moneyFormat = "F0";
    [SerializeField] private string _currencySymbol = "B";
    [SerializeField] private bool _symbolBefore = false;
    [SerializeField] private string _positivePrefix = "";
    [SerializeField] private string _negativePrefix = "";

    private PlayerDataManager _dataManager;
    private Coroutine _changeTextCoroutine;
    private float _lastMoneyValue;

    private void Start()
    {
        InitializeMoneyUI();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void InitializeMoneyUI()
    {
        _dataManager = PlayerDataManager.Instance;

        if (_dataManager == null) return;

        _lastMoneyValue = _dataManager.Stats.Money;
        SubscribeToEvents();
        UpdateMoneyDisplay();
    }

    private void SubscribeToEvents()
    {
        _dataManager.Stats.OnMoneyChanged += OnMoneyChanged;
    }

    private void UnsubscribeFromEvents()
    {
        if (_dataManager != null && _dataManager.Stats != null)
        {
            _dataManager.Stats.OnMoneyChanged -= OnMoneyChanged;
        }
    }

    private void OnMoneyChanged(float newMoney)
    {
        float change = newMoney - _lastMoneyValue;
        _lastMoneyValue = newMoney;

        UpdateMoneyDisplay();

        if (Mathf.Abs(change) > 0.01f) // Если изменение существенное
        {
            PlayMoneyChangeEffect(change);
        }
    }

    private void UpdateMoneyDisplay()
    {
        if (_moneyText != null && _dataManager != null)
        {
            float money = _dataManager.Stats.Money;
            if (_symbolBefore)
            {
                _moneyText.text = $"{_currencySymbol}{money.ToString(_moneyFormat)}";
            }
            else
            {
                _moneyText.text = $"{money.ToString(_moneyFormat)}{_currencySymbol}";
            }
        }
    }

    private void PlayMoneyChangeEffect(float change)
    {
        // Анимация основного текста
        StartCoroutine(MoneyChangeAnimation(change));

        // Показ текста изменения (если есть)
        if (_moneyChangeText != null)
        {
            ShowMoneyChangeText(change);
        }
    }

    private System.Collections.IEnumerator MoneyChangeAnimation(float change)
    {
        if (_moneyText == null) yield break;

        RectTransform rect = _moneyText.GetComponent<RectTransform>();
        Vector3 originalScale = rect.localScale;
        Color originalColor = _moneyText.color;

        // Определяем цвет и направление анимации
        Color targetColor = change >= 0 ? _positiveChangeColor : _negativeChangeColor;
        float scaleMultiplier = change >= 0 ? 1.2f : 0.9f;

        // Анимация масштаба и цвета
        rect.localScale = originalScale * scaleMultiplier;
        _moneyText.color = targetColor;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            rect.localScale = Vector3.Lerp(
                originalScale * scaleMultiplier,
                originalScale,
                progress
            );

            _moneyText.color = Color.Lerp(targetColor, originalColor, progress);

            yield return null;
        }

        rect.localScale = originalScale;
        _moneyText.color = originalColor;
    }

    private void ShowMoneyChangeText(float change)
    {
        if (_changeTextCoroutine != null)
            StopCoroutine(_changeTextCoroutine);

        _changeTextCoroutine = StartCoroutine(MoneyChangeTextCoroutine(change));
    }

    private System.Collections.IEnumerator MoneyChangeTextCoroutine(float change)
    {
        if (_moneyChangeText == null) yield break;

        // Настраиваем текст изменения
        string prefix = change >= 0 ? _positivePrefix : _negativePrefix;
        string changeText = $"{prefix}{Mathf.Abs(change).ToString(_moneyFormat)}{_currencySymbol}";

        _moneyChangeText.text = changeText;
        _moneyChangeText.color = change >= 0 ? _positiveChangeColor : _negativeChangeColor;
        _moneyChangeText.alpha = 1f;

        RectTransform rect = _moneyChangeText.GetComponent<RectTransform>();
        Vector3 startPos = rect.localPosition;
        Vector3 targetPos = startPos + Vector3.up * 50f;

        // Анимация всплывающего текста
        float duration = _changeDisplayDuration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Движение вверх и fade out
            rect.localPosition = Vector3.Lerp(startPos, targetPos, progress);
            _moneyChangeText.alpha = 1f - progress;

            yield return null;
        }

        _moneyChangeText.alpha = 0f;
        rect.localPosition = startPos;
    }

    public void RefreshMoney()
    {
        UpdateMoneyDisplay();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_moneyText == null)
            _moneyText = GetComponentInChildren<TextMeshProUGUI>();

        if (_moneyChangeText == null)
        {
            TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in texts)
            {
                if (text != _moneyText && (text.name.Contains("Change") || text.name.Contains("Delta")))
                {
                    _moneyChangeText = text;
                    break;
                }
            }
        }
    }

    [ContextMenu("Test Add Money")]
    private void TestAddMoney()
    {
        if (_dataManager != null)
        {
            _dataManager.AddMoney(100);
        }
    }

    [ContextMenu("Test Spend Money")]
    private void TestSpendMoney()
    {
        if (_dataManager != null)
        {
            _dataManager.SpendMoney(50);
        }
    }
#endif
}