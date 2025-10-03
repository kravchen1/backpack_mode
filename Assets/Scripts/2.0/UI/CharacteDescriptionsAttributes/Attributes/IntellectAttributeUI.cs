// IntellectAttributeUI.cs
public class IntellectAttributeUI : BaseAttributeUI
{
    protected override void SubscribeToEvents()
    {
        _getAttributeValue = () => _dataManager.Attributes.Intellect;
        _dataManager.Attributes.intellect.OnValueChanged += OnAttributeChanged;
    }

    protected override void UnsubscribeFromEvents()
    {
        if (_dataManager != null && _dataManager.Attributes != null)
        {
            _dataManager.Attributes.intellect.OnValueChanged -= OnAttributeChanged;
        }
    }
}