// CharismaAttributeUI.cs
public class CharismaAttributeUI : BaseAttributeUI
{
    protected override void SubscribeToEvents()
    {
        _getAttributeValue = () => _dataManager.Attributes.Charisma;
        _dataManager.Attributes.charisma.OnValueChanged += OnAttributeChanged;
    }

    protected override void UnsubscribeFromEvents()
    {
        if (_dataManager != null && _dataManager.Attributes != null)
        {
            _dataManager.Attributes.charisma.OnValueChanged -= OnAttributeChanged;
        }
    }
}