using UnityEngine;

public class HealthBarUI : BaseBarUI
{
    private PlayerDataManager _dataManager;

    protected override void InitializeBar()
    {
        _dataManager = PlayerDataManager.Instance;

        if (_dataManager == null) return;

        SubscribeToEvents();

        var initialData = new BarData(
            _dataManager.Stats.CurrentHealth,
            _dataManager.Stats.MaxHealth
        );
        UpdateBarImmediate(initialData);
    }

    protected override void SubscribeToEvents()
    {
        _dataManager.Stats.OnHealthChanged += OnDataChanged;
        _dataManager.Stats.OnDamageTaken += OnDamageTaken; // Подписываемся на новое событие
    }

    protected override void UnsubscribeFromEvents()
    {
        if (_dataManager != null && _dataManager.Stats != null)
        {
            _dataManager.Stats.OnHealthChanged -= OnDataChanged;
            _dataManager.Stats.OnDamageTaken -= OnDamageTaken;
        }
    }

    protected void OnDataChanged(int current, int max)
    {
        // Обычное обновление полоски без эффектов
        UpdateBar(new BarData(current, max));
    }

    // Новый метод для обработки урона
    private void OnDamageTaken(int damage, int currentHealth, int maxHealth)
    {
        Debug.Log($"Damage event received! Damage: {damage}");

        // Обновляем полоску
        UpdateBar(new BarData(currentHealth, maxHealth));

        // Воспроизводим эффекты
        PlayDamageEffect(damage);
    }

    public override void RefreshBar()
    {
        if (_dataManager != null)
        {
            var data = new BarData(
                _dataManager.Stats.CurrentHealth,
                _dataManager.Stats.MaxHealth
            );
            UpdateBarImmediate(data);
        }
    }

    private void PlayDamageEffect(int damage)
    {
        StartCoroutine(DamageFlashEffect());

        // Дополнительные эффекты в зависимости от урона
        if (damage > 30)
        {
            StartCoroutine(HeavyDamageEffect());
        }
        else if (damage > 10)
        {
            StartCoroutine(MediumDamageEffect());
        }
    }

    private System.Collections.IEnumerator DamageFlashEffect()
    {
        if (_fillImage == null) yield break;

        Color originalColor = _fillImage.color;

        // Быстрое мигание
        for (int i = 0; i < 2; i++)
        {
            _fillImage.color = Color.black;
            yield return new WaitForSeconds(0.04f);
            _fillImage.color = originalColor;
            yield return new WaitForSeconds(0.04f);
        }
    }

    private System.Collections.IEnumerator MediumDamageEffect()
    {
        // Эффект для среднего урона
        yield return StartCoroutine(DamageFlashEffect());

        // Можно добавить звук
        // AudioManager.Play("DamageMedium");
    }

    private System.Collections.IEnumerator HeavyDamageEffect()
    {
        // Эффект для тяжелого урона
        yield return StartCoroutine(DamageFlashEffect());

        // Тряска
        yield return StartCoroutine(ShakeEffect(15f, 0.3f));

        // Звук тяжелого урона
        // AudioManager.Play("DamageHeavy");
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

    protected override void OnDataChanged(float current, float max)
    {
        throw new System.NotImplementedException();
    }
}