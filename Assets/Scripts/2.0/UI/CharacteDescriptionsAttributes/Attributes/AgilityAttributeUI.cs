// AgilityAttributeUI.cs
public class AgilityAttributeUI : BaseAttributeUI
{
    protected override void SubscribeToEvents()
    {
        _getAttributeValue = () => _dataManager.Attributes.Agility;
        _dataManager.Attributes.agility.OnValueChanged += OnAttributeChanged;
    }

    protected override void UnsubscribeFromEvents()
    {
        if (_dataManager != null && _dataManager.Attributes != null)
        {
            _dataManager.Attributes.agility.OnValueChanged -= OnAttributeChanged;
        }
    }
}