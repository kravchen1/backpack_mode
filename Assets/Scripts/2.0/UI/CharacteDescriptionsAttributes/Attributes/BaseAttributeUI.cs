using UnityEngine;
using TMPro;
using System;

public abstract class BaseAttributeUI : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _valueText;
    [SerializeField] protected TextMeshProUGUI _modifierText; // Для будущих модификаторов
    [SerializeField] protected string _prefix = "";
    [SerializeField] protected string _suffix = "";

    protected PlayerDataManager _dataManager;
    protected Func<int> _getAttributeValue;

    protected virtual void Start()
    {
        InitializeAttributeUI();
    }

    protected virtual void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    protected virtual void InitializeAttributeUI()
    {
        _dataManager = PlayerDataManager.Instance;

        if (_dataManager == null) return;

        SubscribeToEvents();
        UpdateAttributeValue();
    }

    protected abstract void SubscribeToEvents();
    protected abstract void UnsubscribeFromEvents();

    protected virtual void UpdateAttributeValue()
    {
        if (_getAttributeValue != null && _valueText != null)
        {
            int value = _getAttributeValue();
            _valueText.text = $"{_prefix}{value}{_suffix}";
        }
    }

    protected virtual void OnAttributeChanged(int newValue)
    {
        UpdateAttributeValue();
        PlayAttributeChangeEffect();
    }

    protected virtual void PlayAttributeChangeEffect()
    {
        StartCoroutine(AttributeChangeAnimation());
    }

    protected virtual System.Collections.IEnumerator AttributeChangeAnimation()
    {
        if (_valueText == null) yield break;

        Color originalColor = _valueText.color;
        Vector3 originalScale = _valueText.transform.localScale;

        // Анимация увеличения и изменения цвета
        _valueText.color = Color.green;
        _valueText.transform.localScale = originalScale * 1.2f;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            _valueText.color = Color.Lerp(Color.green, originalColor, progress);
            _valueText.transform.localScale = Vector3.Lerp(
                originalScale * 1.2f,
                originalScale,
                progress
            );

            yield return null;
        }

        _valueText.color = originalColor;
        _valueText.transform.localScale = originalScale;
    }

    public virtual void RefreshAttribute()
    {
        UpdateAttributeValue();
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (_valueText == null)
            _valueText = GetComponentInChildren<TextMeshProUGUI>();
    }
#endif
}