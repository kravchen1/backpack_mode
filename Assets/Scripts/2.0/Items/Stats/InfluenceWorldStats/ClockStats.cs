using UnityEngine;

public class ClockStats : ItemStats
{
    //[Header("Clock Stats")]
    [HideInInspector] public bool isShowTime = true;
    [HideInInspector] public bool isShowDate = false;

    public override void InitializeDescriptionTriples()
    {
        if (_descriptionTriples.Count > 0)
        {
            _descriptionTriples.Clear();
        }

        _descriptionTriples.AddRange(new[]
        {
            new DescriptionTriple("Description", "", ""),
            new DescriptionTriple("Type", "", ""),
            new DescriptionTriple("Rarity", "", ""),
            new DescriptionTriple("Quality", "", ""),
            new DescriptionTriple("Weight", "", ""),
            new DescriptionTriple("Durability", "", ""),
            new DescriptionTriple("Price", "", "")
        });

        if(isShowTime)
        {
            _descriptionTriples.Add(new DescriptionTriple("Show Time","",""));        
        }
        if (isShowDate)
        {
            _descriptionTriples.Add(new DescriptionTriple("Show Date", "", ""));
        }
    }

    protected override string GetSpecificStatValue(string statKey)
    {
        switch (statKey)
        {
            case "Show Time":
                return isShowTime.ToString();
            case "Show Date":
                return isShowDate.ToString();
            default:
                return base.GetSpecificStatValue(statKey);
        }
    }

    protected override void LoadFromDataManager()
    {
        base.LoadFromDataManager();

        if (string.IsNullOrEmpty(itemKey)) return;
        var dataManager = ItemDataManager.Instance;
        if (dataManager == null) return;

        // Загрузка параметров ближнего боя
        isShowTime = dataManager.GetItemData(itemKey, "isShowTime", isShowTime);
        isShowDate = dataManager.GetItemData(itemKey, "isShowDate", isShowDate);
    }
}