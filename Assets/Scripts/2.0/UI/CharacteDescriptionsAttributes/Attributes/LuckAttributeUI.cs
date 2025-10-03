// LuckAttributeUI.cs
public class LuckAttributeUI : BaseAttributeUI
{
    protected override void SubscribeToEvents()
    {
        _getAttributeValue = () => _dataManager.Attributes.Luck;
        _dataManager.Attributes.luck.OnValueChanged += OnAttributeChanged;
    }

    protected override void UnsubscribeFromEvents()
    {
        if (_dataManager != null && _dataManager.Attributes != null)
        {
            _dataManager.Attributes.luck.OnValueChanged -= OnAttributeChanged;
        }
    }
}