using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

// Базовый тип для данных, которые отображаются в полоске (теперь float)
public struct BarData
{
    public float current;
    public float max;
    public float normalized => max > 0 ? Mathf.Clamp01(current / max) : 0f;

    public BarData(float current, float max)
    {
        this.current = current;
        this.max = max;
    }
}

// Базовый класс для всех типов полосок
public abstract class BaseBarUI : MonoBehaviour
{
    [Header("Base Bar References")]
    [SerializeField] protected Image _fillImage;
    [SerializeField] protected TextMeshProUGUI _valueText;

    [Header("Base Animation Settings")]
    [SerializeField] protected float _animationSpeed = 2f;
    [SerializeField] protected bool _animateText = false;
    [SerializeField] protected float _textAnimationDuration = 0.5f;

    [Header("Base Visual Settings")]
    [SerializeField] protected Color _fullColor = Color.green;
    [SerializeField] protected Color _mediumColor = Color.yellow;
    [SerializeField] protected Color _lowColor = Color.red;
    [SerializeField] protected float _lowThreshold = 0.3f;
    [SerializeField] protected float _mediumThreshold = 0.6f;

    protected float _currentFillTarget = 1f;
    protected Color _currentColorTarget;
    protected BarData _currentData;

    protected Coroutine _textAnimationCoroutine;
    protected float _currentDisplayValue;

    protected virtual void Start()
    {
        InitializeBar();
    }

    protected virtual void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    protected virtual void Update()
    {
        AnimateBar();
    }

    /// <summary>
    /// Инициализация полоски - должен быть реализован в дочернем классе
    /// </summary>
    protected abstract void InitializeBar();

    /// <summary>
    /// Подписка на события - должен быть реализован в дочернем классе
    /// </summary>
    protected abstract void SubscribeToEvents();

    /// <summary>
    /// Отписка от событий - должен быть реализован в дочернем классе
    /// </summary>
    protected abstract void UnsubscribeFromEvents();

    /// <summary>
    /// Обработчик изменения данных - должен быть реализован в дочернем классе
    /// </summary>
    protected abstract void OnDataChanged(float current, float max);

    /// <summary>
    /// Обновление полоски с новыми данными
    /// </summary>
    protected virtual void UpdateBar(BarData newData)
    {
        _currentData = newData;
        _currentFillTarget = newData.normalized;
        _currentColorTarget = GetColorForValue(newData.normalized);

        UpdateText(newData);
        UpdateFillImmediate();
    }

    /// <summary>
    /// Мгновенное обновление (для инициализации)
    /// </summary>
    protected virtual void UpdateBarImmediate(BarData newData)
    {
        _currentData = newData;
        _currentFillTarget = newData.normalized;
        _currentColorTarget = GetColorForValue(newData.normalized);

        _fillImage.fillAmount = newData.normalized;
        _fillImage.color = _currentColorTarget;

        UpdateTextImmediate(newData);
    }

    /// <summary>
    /// Определение цвета в зависимости от заполнения
    /// </summary>
    protected virtual Color GetColorForValue(float normalizedValue)
    {
        if (normalizedValue <= _lowThreshold)
            return _lowColor;
        else if (normalizedValue <= _mediumThreshold)
            return _mediumColor;
        else
            return _fullColor;
    }

    /// <summary>
    /// Обновление текста с анимацией или без
    /// </summary>
    protected virtual void UpdateText(BarData data)
    {
        if (_animateText)
        {
            if (_textAnimationCoroutine != null)
                StopCoroutine(_textAnimationCoroutine);

            _textAnimationCoroutine = StartCoroutine(AnimateTextValue(data.current, data.max));
        }
        else
        {
            UpdateTextImmediate(data);
        }
    }

    /// <summary>
    /// Мгновенное обновление текста
    /// </summary>
    protected virtual void UpdateTextImmediate(BarData data)
    {
        _currentDisplayValue = data.current;
        _valueText.text = FormatText(data.current, data.max);
    }

    /// <summary>
    /// Форматирование текста (можно переопределить в дочерних классах)
    /// </summary>
    protected virtual string FormatText(float current, float max)
    {
        return $"{current:F0}/{max:F0}";
    }

    /// <summary>
    /// Анимация числового значения
    /// </summary>
    protected virtual IEnumerator AnimateTextValue(float targetCurrent, float targetMax)
    {
        float elapsedTime = 0f;
        float startValue = _currentDisplayValue;

        while (elapsedTime < _textAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / _textAnimationDuration;

            _currentDisplayValue = Mathf.Lerp(startValue, targetCurrent, progress);

            _valueText.text = FormatText(_currentDisplayValue, targetMax);
            yield return null;
        }

        _currentDisplayValue = targetCurrent;
        _valueText.text = FormatText(_currentDisplayValue, targetMax);
    }

    /// <summary>
    /// Анимация заполнения и цвета
    /// </summary>
    protected virtual void AnimateBar()
    {
        // Анимация заполнения
        if (Mathf.Abs(_fillImage.fillAmount - _currentFillTarget) > 0.001f)
        {
            _fillImage.fillAmount = Mathf.Lerp(
                _fillImage.fillAmount,
                _currentFillTarget,
                _animationSpeed * Time.deltaTime
            );
        }

        // Анимация цвета
        if (_fillImage.color != _currentColorTarget)
        {
            _fillImage.color = Color.Lerp(
                _fillImage.color,
                _currentColorTarget,
                _animationSpeed * Time.deltaTime
            );
        }
    }

    /// <summary>
    /// Мгновенное обновление заполнения
    /// </summary>
    protected virtual void UpdateFillImmediate()
    {
        _fillImage.fillAmount = _currentFillTarget;
        _fillImage.color = _currentColorTarget;
    }

    /// <summary>
    /// Принудительное обновление полоски
    /// </summary>
    public abstract void RefreshBar();

    #region Editor Methods
#if UNITY_EDITOR
    [ContextMenu("Test Low Value")]
    protected void TestLowValue()
    {
        UpdateBar(new BarData(25f, 100f));
    }

    [ContextMenu("Test Medium Value")]
    protected void TestMediumValue()
    {
        UpdateBar(new BarData(60f, 100f));
    }

    [ContextMenu("Test Full Value")]
    protected void TestFullValue()
    {
        UpdateBar(new BarData(100f, 100f));
    }

    protected virtual void OnValidate()
    {
        if (_fillImage == null)
            _fillImage = GetComponentInChildren<Image>();

        if (_valueText == null)
            _valueText = GetComponentInChildren<TextMeshProUGUI>();
    }
#endif
    #endregion
}