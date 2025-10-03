// StrengthAttributeUI.cs
public class StrengthAttributeUI : BaseAttributeUI
{
    protected override void SubscribeToEvents()
    {
        _getAttributeValue = () => _dataManager.Attributes.Strength;
        _dataManager.Attributes.strength.OnValueChanged += OnAttributeChanged;
    }

    protected override void UnsubscribeFromEvents()
    {
        if (_dataManager != null && _dataManager.Attributes != null)
        {
            _dataManager.Attributes.strength.OnValueChanged -= OnAttributeChanged;
        }
    }
}