// Модификаторы оружия (прицелы, обоймы и т.д.)
using UnityEngine;

public class WeaponModStats : ItemStats
{
    [Header("Weapon Mod Stats")]
    public float damageModifier = 0f;
    public float accuracyModifier = 0f;
    public float critChanceModifier = 0f;
    public float reloadSpeedModifier = 0f;

    public override void InitializeDescriptionTriples()
    {
        if (_descriptionTriples.Count > 0)
        {
            _descriptionTriples.Clear();
        }
        _descriptionTriples.AddRange(new[]
        {
            new DescriptionTriple("Type", "", ""),
            new DescriptionTriple("Rarity", "", ""),
            new DescriptionTriple("Quality", "", ""),
            new DescriptionTriple("Damage Mod", $"{damageModifier:+#;-#;0}%", ""),
            new DescriptionTriple("Accuracy Mod", $"{accuracyModifier:+#;-#;0}", ""),
            new DescriptionTriple("Crit Chance Mod", $"{critChanceModifier:+#;-#;0}%", ""),
            new DescriptionTriple("Reload Speed Mod", $"{reloadSpeedModifier:+#;-#;0}%", ""),
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
            case "Damage Mod":
                return $"{damageModifier:+#;-#;0}%";
            case "Accuracy Mod":
                return $"{accuracyModifier:+#;-#;0}";
            case "Crit Chance Mod":
                return $"{critChanceModifier:+#;-#;0}%";
            case "Reload Speed Mod":
                return $"{reloadSpeedModifier:+#;-#;0}%";
            default:
                return base.GetSpecificStatValue(statKey);
        }
    }
}