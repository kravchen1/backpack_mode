using UnityEngine;

public class ArmorStats : ItemStats
{
    [Header("Armor Stats")]
    public int damageConsumptionPerDurability = 1;

    public override void InitializeDescriptionTriples()
    {
        if (_descriptionTriples.Count > 0)
        {
            _descriptionTriples.Clear();
        }

        _descriptionTriples.AddRange(new[]
        {
            new DescriptionTriple("Description", "", ""),
            new DescriptionTriple("Dmg / 1 Durability", $"{damageConsumptionPerDurability:0}", ""),
            new DescriptionTriple("Weight", "", ""),
            new DescriptionTriple("Durability", "", ""),
            new DescriptionTriple("Requirements", "", ""),
            new DescriptionTriple("Price", "", "")
        });
    }

    protected override string GetSpecificStatValue(string statKey)
    {
        switch (statKey)
        {
            case "Damage consumption per durability":
                return $"{damageConsumptionPerDurability:0}";
            default:
                return base.GetSpecificStatValue(statKey);
        }
    }
}