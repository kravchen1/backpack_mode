// EnduranceAttributeUI.cs
public class EnduranceAttributeUI : BaseAttributeUI
{
    protected override void SubscribeToEvents()
    {
        _getAttributeValue = () => _dataManager.Attributes.Endurance;
        _dataManager.Attributes.endurance.OnValueChanged += OnAttributeChanged;
    }

    protected override void UnsubscribeFromEvents()
    {
        if (_dataManager != null && _dataManager.Attributes != null)
        {
            _dataManager.Attributes.endurance.OnValueChanged -= OnAttributeChanged;
        }
    }
}