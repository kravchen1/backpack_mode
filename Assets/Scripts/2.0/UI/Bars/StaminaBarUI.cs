using UnityEngine;

public class StaminaBarUI : BaseBarUI
{
    private PlayerDataManager _dataManager;

    protected override void InitializeBar()
    {
        _dataManager = PlayerDataManager.Instance;

        if (_dataManager == null)
        {
            Debug.LogError("PlayerDataManager not found!", this);
            return;
        }

        SubscribeToEvents();

        var initialData = new BarData(
            (int)_dataManager.Stats.CurrentStamina,
            (int)_dataManager.Stats.MaxStamina
        );
        UpdateBarImmediate(initialData);
    }

    protected override void SubscribeToEvents()
    {
        _dataManager.Stats.OnStaminaChanged += OnDataChanged;
    }

    protected override void UnsubscribeFromEvents()
    {
        if (_dataManager != null && _dataManager.Stats != null)
        {
            _dataManager.Stats.OnStaminaChanged -= OnDataChanged;
        }
    }

    protected override void OnDataChanged(float current, float max)
    {
        UpdateBar(new BarData(current, max));

        //// Дополнительная логика для выносливости (например, эффект истощения)
        //if (current == 0)
        //{
        //    PlayExhaustionEffect();
        //}
    }

    public override void RefreshBar()
    {
        if (_dataManager != null)
        {
            var data = new BarData(
                (int)_dataManager.Stats.CurrentStamina,
                (int)_dataManager.Stats.MaxStamina
            );
            UpdateBarImmediate(data);
        }
    }

    // Можно переопределить цветовую схему для выносливости
    //protected override Color GetColorForValue(float normalizedValue)
    //{
    //    // Своя цветовая схема для выносливости
    //    if (normalizedValue <= _lowThreshold)
    //        return Color.red;
    //    else if (normalizedValue <= _mediumThreshold)
    //        return Color.yellow;
    //    else
    //        return Color.blue; // Синий для выносливости
    //}

    // Можно переопределить формат текста
    protected override string FormatText(float current, float max)
    {
        return $"{current:0.0}/{max:0.0}"; // Пример кастомного формата
    }

    //private void PlayExhaustionEffect()
    //{
    //    // Эффект истощения выносливости
    //    StartCoroutine(ExhaustionFlashEffect());
    //}

    //private System.Collections.IEnumerator ExhaustionFlashEffect()
    //{
    //    // Реализация эффекта истощения
    //    yield return null;
    //}
}