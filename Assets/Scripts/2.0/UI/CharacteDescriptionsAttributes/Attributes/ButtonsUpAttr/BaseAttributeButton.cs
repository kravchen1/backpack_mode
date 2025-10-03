using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class BaseAttributeButton : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] protected Button _button;
    [SerializeField] protected TextMeshProUGUI _buttonText;
    [SerializeField] protected TextMeshProUGUI _attributeValueText;

    [Header("Button Settings")]
    [SerializeField] protected string _buttonPrefix = "+";
    [SerializeField] protected Color _activeColor = Color.green;
    [SerializeField] protected Color _inactiveColor = Color.gray;

    protected PlayerDataManager _dataManager;
    protected CanvasGroup _canvasGroup;

    protected virtual void Start()
    {
        InitializeButton();
    }

    protected virtual void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    protected virtual void InitializeButton()
    {
        _dataManager = PlayerDataManager.Instance;
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_dataManager == null) return;

        // Настраиваем кнопку
        if (_button != null)
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        if (_buttonText != null)
        {
            _buttonText.text = _buttonPrefix;
        }

        SubscribeToEvents();
        UpdateButtonState();
        UpdateAttributeValue();
    }

    protected abstract void SubscribeToEvents();
    protected abstract void UnsubscribeFromEvents();

    protected virtual void OnButtonClick()
    {
        if (_dataManager == null) return;

        bool success = TryUpgradeAttribute();

        if (success)
        {
            PlayUpgradeEffect();
        }
        else
        {
            PlayFailedEffect();
        }
    }

    protected abstract bool TryUpgradeAttribute();

    protected virtual void OnSkillPointsChanged(int unspentPoints)
    {
        UpdateButtonState();
    }

    protected virtual void OnAttributeChanged(int newValue)
    {
        UpdateAttributeValue();
    }

    protected virtual void UpdateButtonState()
    {
        if (_dataManager == null) return;

        bool hasPoints = _dataManager.Stats.UnspentSkillPoints > 0;

        // Управляем видимостью через CanvasGroup
        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = hasPoints ? 1f : 0.3f;
            _canvasGroup.interactable = hasPoints;
            _canvasGroup.blocksRaycasts = hasPoints;
        }

        // Меняем цвет кнопки
        if (_button != null)
        {
            ColorBlock colors = _button.colors;
            colors.normalColor = hasPoints ? _activeColor : _inactiveColor;
            colors.highlightedColor = hasPoints ? _activeColor * 1.2f : _inactiveColor;
            colors.pressedColor = hasPoints ? _activeColor * 0.8f : _inactiveColor;
            colors.disabledColor = _inactiveColor;
            _button.colors = colors;
        }
    }

    protected virtual void UpdateAttributeValue()
    {
        // Должен быть реализован в дочерних классах
    }

    protected virtual void PlayUpgradeEffect()
    {
        StartCoroutine(UpgradeAnimation());
    }

    protected virtual void PlayFailedEffect()
    {
        StartCoroutine(FailedAnimation());
    }

    protected virtual System.Collections.IEnumerator UpgradeAnimation()
    {
        if (_button == null) yield break;

        RectTransform rect = _button.GetComponent<RectTransform>();
        Vector3 originalScale = rect.localScale;

        // Анимация успешного улучшения
        rect.localScale = originalScale * 1.2f;
        yield return new WaitForSeconds(0.1f);

        rect.localScale = originalScale * 0.9f;
        yield return new WaitForSeconds(0.1f);

        rect.localScale = originalScale;
    }

    protected virtual System.Collections.IEnumerator FailedAnimation()
    {
        if (_button == null) yield break;

        RectTransform rect = _button.GetComponent<RectTransform>();
        Vector3 originalPos = rect.localPosition;

        // Анимация тряски при невозможности улучшить
        float shakeIntensity = 5f;
        float shakeDuration = 0.3f;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            rect.localPosition = originalPos + new Vector3(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                0
            );
            yield return null;
        }

        rect.localPosition = originalPos;
    }

    public virtual void RefreshButton()
    {
        UpdateButtonState();
        UpdateAttributeValue();
    }

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (_button == null)
            _button = GetComponent<Button>();

        if (_buttonText == null)
        {
            TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in texts)
            {
                if (text.name.Contains("Button") || text.name.Contains("Btn") || text.transform.parent == transform)
                {
                    _buttonText = text;
                    break;
                }
            }
        }

        if (_attributeValueText == null)
        {
            // Поиск текста значения атрибута (обычно находится рядом с кнопкой)
            TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in texts)
            {
                if (text != _buttonText && (text.name.Contains("Value") || text.name.Contains("Text")))
                {
                    _attributeValueText = text;
                    break;
                }
            }
        }
    }
#endif
}