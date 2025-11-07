using UnityEngine;

public class ArmorStats : ItemStats
{
    //[Header("Armor Stats")]
    [HideInInspector] public int damageConsumptionPerDurability = 1;

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
            //new DescriptionTriple("Requirements", "", ""),
            new DescriptionTriple("Dmg / 1 Durability", $"{damageConsumptionPerDurability:0}", ""),
            new DescriptionTriple("Price", "", "")
        });
    }

    protected override string GetSpecificStatValue(string statKey)
    {
        switch (statKey)
        {
            case "Dmg / 1 Durability":
                return $"{damageConsumptionPerDurability:0}";
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
        damageConsumptionPerDurability = dataManager.GetItemData(itemKey, "damageConsumptionPerDurability", damageConsumptionPerDurability);
    }
}