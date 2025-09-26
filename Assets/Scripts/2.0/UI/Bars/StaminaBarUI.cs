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

        //// �������������� ������ ��� ������������ (��������, ������ ���������)
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

    // ����� �������������� �������� ����� ��� ������������
    //protected override Color GetColorForValue(float normalizedValue)
    //{
    //    // ���� �������� ����� ��� ������������
    //    if (normalizedValue <= _lowThreshold)
    //        return Color.red;
    //    else if (normalizedValue <= _mediumThreshold)
    //        return Color.yellow;
    //    else
    //        return Color.blue; // ����� ��� ������������
    //}

    // ����� �������������� ������ ������
    protected override string FormatText(float current, float max)
    {
        return $"{current:0.0}/{max:0.0}"; // ������ ���������� �������
    }

    //private void PlayExhaustionEffect()
    //{
    //    // ������ ��������� ������������
    //    StartCoroutine(ExhaustionFlashEffect());
    //}

    //private System.Collections.IEnumerator ExhaustionFlashEffect()
    //{
    //    // ���������� ������� ���������
    //    yield return null;
    //}
}