// StrengthButton.cs
public class StrengthButton : BaseAttributeButton
{
    protected override void SubscribeToEvents()
    {
        _dataManager.Stats.OnSkillPointsChanged += OnSkillPointsChanged;
        _dataManager.Attributes.strength.OnValueChanged += OnAttributeChanged;
    }

    protected override void UnsubscribeFromEvents()
    {
        if (_dataManager != null)
        {
            if (_dataManager.Stats != null)
                _dataManager.Stats.OnSkillPointsChanged -= OnSkillPointsChanged;

            if (_dataManager.Attributes != null)
                _dataManager.Attributes.strength.OnValueChanged -= OnAttributeChanged;
        }
    }

    protected override bool TryUpgradeAttribute()
    {
        return _dataManager.UpgradeStrength();
    }

    protected override void UpdateAttributeValue()
    {
        if (_attributeValueText != null)
        {
            _attributeValueText.text = _dataManager.Attributes.Strength.ToString();
        }
    }
}