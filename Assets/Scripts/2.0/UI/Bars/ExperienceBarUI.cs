using UnityEngine;
using TMPro;

public class ExperienceBarUI : BaseBarUI
{
    [Header("Experience Bar References")]
    [SerializeField] protected TextMeshProUGUI _valueLvLText;

    private PlayerDataManager _dataManager;

    protected override void InitializeBar()
    {
        _dataManager = PlayerDataManager.Instance;

        if (_dataManager == null) return;

        SubscribeToEvents();

        var initialData = new BarData(
            _dataManager.Stats.CurrentExp,
            _dataManager.Stats.ExpToNextLevel
        );
        UpdateBarImmediate(initialData);
        UpdateLevelText(_dataManager.Stats.Level);
    }

    protected override void SubscribeToEvents()
    {
        _dataManager.Stats.OnExpChanged += OnExpChanged;
        _dataManager.Stats.OnLevelUp += OnLevelUp;
    }

    protected override void UnsubscribeFromEvents()
    {
        if (_dataManager != null && _dataManager.Stats != null)
        {
            _dataManager.Stats.OnExpChanged -= OnExpChanged;
            _dataManager.Stats.OnLevelUp -= OnLevelUp;
        }
    }

    protected override void OnDataChanged(float current, float max)
    {
        // Этот метод не используется для опыта, так как у нас есть специальные события
    }

    private void OnExpChanged(int currentExp, int expToNextLevel, int addedExp)
    {
        // Обновляем полоску
        UpdateBar(new BarData(currentExp, expToNextLevel));

        // Воспроизводим эффекты при получении опыта
        PlayExpGainEffect(addedExp);
    }

    private void OnLevelUp(int newLevel)
    {
        // При повышении уровня сбрасываем полоску и показываем эффект
        var data = new BarData(0, _dataManager.Stats.ExpToNextLevel);
        UpdateBarImmediate(data);

        // Обновляем текст уровня
        UpdateLevelText(newLevel);

        PlayLevelUpEffect(newLevel);
    }

    public override void RefreshBar()
    {
        if (_dataManager != null)
        {
            var data = new BarData(
                _dataManager.Stats.CurrentExp,
                _dataManager.Stats.ExpToNextLevel
            );
            UpdateBarImmediate(data);
            UpdateLevelText(_dataManager.Stats.Level);
        }
    }

    protected override string FormatText(float current, float max)
    {
        return $"{current:F0}/{max:F0}";
    }

    /// <summary>
    /// Обновление текста уровня
    /// </summary>
    protected virtual void UpdateLevelText(int level)
    {
        if (_valueLvLText != null)
        {
            _valueLvLText.text = $"{level}";
        }
    }

    /// <summary>
    /// Анимация изменения уровня
    /// </summary>
    protected virtual void AnimateLevelUpText(int newLevel)
    {
        if (_valueLvLText != null)
        {
            StartCoroutine(LevelUpTextAnimation(newLevel));
        }
    }

    private System.Collections.IEnumerator LevelUpTextAnimation(int newLevel)
    {
        RectTransform rect = _valueLvLText.GetComponent<RectTransform>();
        Vector3 originalScale = rect.localScale;
        Color originalColor = _valueLvLText.color;

        // Увеличиваем масштаб
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Пульсация масштаба
            float scale = 1f + Mathf.Sin(progress * Mathf.PI) * 0.3f;
            rect.localScale = originalScale * scale;

            // Изменение цвета
            _valueLvLText.color = Color.Lerp(Color.yellow, originalColor, progress);

            yield return null;
        }

        rect.localScale = originalScale;
        _valueLvLText.color = originalColor;
        _valueLvLText.text = $"{newLevel}";
    }

    private void PlayExpGainEffect(int expAmount)
    {
        StartCoroutine(ExpGainFlashEffect());

        // Разные эффекты в зависимости от количества полученного опыта
        if (expAmount > 100)
        {
            StartCoroutine(LargeExpGainEffect());
        }
        else if (expAmount > 50)
        {
            StartCoroutine(MediumExpGainEffect());
        }
    }

    private void PlayLevelUpEffect(int newLevel)
    {
        StartCoroutine(LevelUpFlashEffect());
        StartCoroutine(LevelUpScaleEffect());
        AnimateLevelUpText(newLevel);

        // Можно добавить звук и частицы
        // AudioManager.Play("LevelUp");
        // ParticleManager.Play("LevelUp", transform.position);
    }

    private System.Collections.IEnumerator ExpGainFlashEffect()
    {
        if (_fillImage == null) yield break;

        Color originalColor = _fillImage.color;

        // Мигание золотым цветом
        for (int i = 0; i < 2; i++)
        {
            _fillImage.color = Color.yellow;
            yield return new WaitForSeconds(0.05f);
            _fillImage.color = originalColor;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private System.Collections.IEnumerator MediumExpGainEffect()
    {
        yield return StartCoroutine(ExpGainFlashEffect());

        // Легкая тряска
        yield return StartCoroutine(ShakeEffect(5f, 0.2f));
    }

    private System.Collections.IEnumerator LargeExpGainEffect()
    {
        yield return StartCoroutine(ExpGainFlashEffect());

        // Более сильная тряска
        yield return StartCoroutine(ShakeEffect(10f, 0.3f));

        // Дополнительное мигание
        _fillImage.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        _fillImage.color = GetColorForValue(_currentFillTarget);
    }

    private System.Collections.IEnumerator LevelUpFlashEffect()
    {
        if (_fillImage == null) yield break;

        Color originalColor = _fillImage.color;

        // Интенсивное мигание при повышении уровня
        for (int i = 0; i < 4; i++)
        {
            _fillImage.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            _fillImage.color = originalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private System.Collections.IEnumerator LevelUpScaleEffect()
    {
        RectTransform rect = GetComponent<RectTransform>();
        if (rect == null) yield break;

        Vector3 originalScale = rect.localScale;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Пульсация: увеличиваем и уменьшаем масштаб
            float scale = 1f + Mathf.Sin(progress * Mathf.PI * 2) * 0.2f;
            rect.localScale = originalScale * scale;

            yield return null;
        }

        rect.localScale = originalScale;
    }

    private System.Collections.IEnumerator ShakeEffect(float intensity, float duration)
    {
        RectTransform rect = GetComponent<RectTransform>();
        if (rect == null) yield break;

        Vector3 originalPos = rect.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;
            float currentIntensity = intensity * (1f - progress);

            rect.localPosition = originalPos + new Vector3(
                Random.Range(-currentIntensity, currentIntensity),
                Random.Range(-currentIntensity, currentIntensity),
                0
            );

            yield return null;
        }

        rect.localPosition = originalPos;
    }

    #region Editor Methods
#if UNITY_EDITOR
    [ContextMenu("Test Exp Gain")]
    protected void TestExpGain()
    {
        UpdateBar(new BarData(75f, 100f));
    }

    [ContextMenu("Test Level Up")]
    protected void TestLevelUp()
    {
        // Имитация повышения уровня - сброс полоски
        UpdateBarImmediate(new BarData(0f, 150f));
        UpdateLevelText(2);
        AnimateLevelUpText(2);
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        if (_valueLvLText == null)
        {
            // Попробуем найти текстовый компонент уровня по тегу или имени
            TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in texts)
            {
                if (text.name.Contains("Level") || text.name.Contains("Lvl") || text.name.Contains("Lv"))
                {
                    _valueLvLText = text;
                    break;
                }
            }
        }
    }
#endif
    #endregion
}