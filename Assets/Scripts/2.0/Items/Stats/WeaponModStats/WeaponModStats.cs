// Модификаторы оружия (прицелы, обоймы и т.д.)
using UnityEngine;

public class WeaponModStats : ItemStats
{
    [Header("Range Weapon Mod Stats Percentage")]
    public int damageRangeModifierPercentage = 0;
    public int accuracyRangeModifierPercentage = 0;
    public int critChanceRangeModifierPercentage = 0;
    public int critDamageRangeModifierPercentage = 0;
    public int coolDownRangeModifierPercentage = 0;
    public int staminaRangeModifierPercentage = 0;
    [Header("Range Weapon Mod Stats Value")]
    public int damageRangeModifierValue = 0;
    public int accuracyRangeModifierValue = 0;
    public int critChanceRangeModifierValue = 0;
    public int coolDownRangeModifierValue = 0;
    public int staminaRangeModifierValue = 0;
    public int critDamageRangeModifierValue = 0;

    [Header("Melee Weapon Mod Stats Percentage")]
    public int damageMeleeModifierPercentage = 0;
    public int accuracyMeleeModifierPercentage = 0;
    public int critChanceMeleeModifierPercentage = 0;
    public int critDamageMeleeModifierPercentage = 0;
    public int coolDownMeleeModifierPercentage = 0;
    public int staminaMeleeModifierPercentage = 0;
    [Header("Melee Weapon Mod Stats Value")]
    public int damageMeleeModifierValue = 0;
    public int accuracyMeleeModifierValue = 0;
    public int critChanceMeleeModifierValue = 0;
    public int coolDownMeleeModifierValue = 0;
    public int staminaMeleeModifierValue = 0;
    public int critDamageMeleeModifierValue = 0;

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
            new DescriptionTriple("Weight", "", ""),
            new DescriptionTriple("Durability", "", ""),
            new DescriptionTriple("Requirements", "", ""),
            new DescriptionTriple("Price", "", "")
        });

        // Range Percentage modifiers
        if (damageRangeModifierPercentage != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Range Damage Percentage", $"{damageRangeModifierPercentage:+#;-#;0}%", ""));
        }
        if (accuracyRangeModifierPercentage != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Range Accuracy Percentage", $"{accuracyRangeModifierPercentage:+#;-#;0}%", ""));
        }
        if (critChanceRangeModifierPercentage != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Range Crit Chance Percentage", $"{critChanceRangeModifierPercentage:+#;-#;0}%", ""));
        }
        if (critDamageRangeModifierPercentage != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Range Crit Damage Percentage", $"{critDamageRangeModifierPercentage:+#;-#;0}%", ""));
        }
        if (coolDownRangeModifierPercentage != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Range CoolDown Percentage", $"{coolDownRangeModifierPercentage:+#;-#;0}%", ""));
        }
        if (staminaRangeModifierPercentage != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Range Stamina Percentage", $"{staminaRangeModifierPercentage:+#;-#;0}%", ""));
        }

        // Range Value modifiers
        if (damageRangeModifierValue != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Range Damage Value", $"{damageRangeModifierValue:+#;-#;0}", ""));
        }
        if (accuracyRangeModifierValue != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Range Accuracy Value", $"{accuracyRangeModifierValue:+#;-#;0}", ""));
        }
        if (critChanceRangeModifierValue != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Range Crit Chance Value", $"{critChanceRangeModifierValue:+#;-#;0}", ""));
        }
        if (critDamageRangeModifierValue != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Range Crit Damage Value", $"{critDamageRangeModifierValue:+#;-#;0}", ""));
        }
        if (coolDownRangeModifierValue != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Range CoolDown Value", $"{coolDownRangeModifierValue:+#;-#;0}", ""));
        }
        if (staminaRangeModifierValue != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Range Stamina Value", $"{staminaRangeModifierValue:+#;-#;0}", ""));
        }

        // Melee Percentage modifiers
        if (damageMeleeModifierPercentage != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Melee Damage Percentage", $"{damageMeleeModifierPercentage:+#;-#;0}%", ""));
        }
        if (accuracyMeleeModifierPercentage != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Melee Accuracy Percentage", $"{accuracyMeleeModifierPercentage:+#;-#;0}%", ""));
        }
        if (critChanceMeleeModifierPercentage != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Melee Crit Chance Percentage", $"{critChanceMeleeModifierPercentage:+#;-#;0}%", ""));
        }
        if (critDamageMeleeModifierPercentage != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Melee Crit Damage Percentage", $"{critDamageMeleeModifierPercentage:+#;-#;0}%", ""));
        }
        if (coolDownMeleeModifierPercentage != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Melee CoolDown Percentage", $"{coolDownMeleeModifierPercentage:+#;-#;0}%", ""));
        }
        if (staminaMeleeModifierPercentage != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Melee Stamina Percentage", $"{staminaMeleeModifierPercentage:+#;-#;0}%", ""));
        }

        // Melee Value modifiers
        if (damageMeleeModifierValue != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Melee Damage Value", $"{damageMeleeModifierValue:+#;-#;0}", ""));
        }
        if (accuracyMeleeModifierValue != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Melee Accuracy Value", $"{accuracyMeleeModifierValue:+#;-#;0}", ""));
        }
        if (critChanceMeleeModifierValue != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Melee Crit Chance Value", $"{critChanceMeleeModifierValue:+#;-#;0}", ""));
        }
        if (critDamageMeleeModifierValue != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Melee Crit Damage Value", $"{critDamageMeleeModifierValue:+#;-#;0}", ""));
        }
        if (coolDownMeleeModifierValue != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Melee CoolDown Value", $"{coolDownMeleeModifierValue:+#;-#;0}", ""));
        }
        if (staminaMeleeModifierValue != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Melee Stamina Value", $"{staminaMeleeModifierValue:+#;-#;0}", ""));
        }
    }

    protected override string GetSpecificStatValue(string statKey)
    {
        switch (statKey)
        {
            // Range Percentage
            case "Range Damage Percentage":
                return $"{damageRangeModifierPercentage:+#;-#;0}%";
            case "Range Accuracy Percentage":
                return $"{accuracyRangeModifierPercentage:+#;-#;0}%";
            case "Range Crit Chance Percentage":
                return $"{critChanceRangeModifierPercentage:+#;-#;0}%";
            case "Range Crit Damage Percentage":
                return $"{critDamageRangeModifierPercentage:+#;-#;0}%";
            case "Range CoolDown Percentage":
                return $"{coolDownRangeModifierPercentage:+#;-#;0}%";
            case "Range Stamina Percentage":
                return $"{staminaRangeModifierPercentage:+#;-#;0}%";

            // Range Value
            case "Range Damage Value":
                return $"{damageRangeModifierValue:+#;-#;0}";
            case "Range Accuracy Value":
                return $"{accuracyRangeModifierValue:+#;-#;0}";
            case "Range Crit Chance Value":
                return $"{critChanceRangeModifierValue:+#;-#;0}";
            case "Range Crit Damage Value":
                return $"{critDamageRangeModifierValue:+#;-#;0}";
            case "Range CoolDown Value":
                return $"{coolDownRangeModifierValue:+#;-#;0}";
            case "Range Stamina Value":
                return $"{staminaRangeModifierValue:+#;-#;0}";

            // Melee Percentage
            case "Melee Damage Percentage":
                return $"{damageMeleeModifierPercentage:+#;-#;0}%";
            case "Melee Accuracy Percentage":
                return $"{accuracyMeleeModifierPercentage:+#;-#;0}%";
            case "Melee Crit Chance Percentage":
                return $"{critChanceMeleeModifierPercentage:+#;-#;0}%";
            case "Melee Crit Damage Percentage":
                return $"{critDamageMeleeModifierPercentage:+#;-#;0}%";
            case "Melee CoolDown Percentage":
                return $"{coolDownMeleeModifierPercentage:+#;-#;0}%";
            case "Melee Stamina Percentage":
                return $"{staminaMeleeModifierPercentage:+#;-#;0}%";

            // Melee Value
            case "Melee Damage Value":
                return $"{damageMeleeModifierValue:+#;-#;0}";
            case "Melee Accuracy Value":
                return $"{accuracyMeleeModifierValue:+#;-#;0}";
            case "Melee Crit Chance Value":
                return $"{critChanceMeleeModifierValue:+#;-#;0}";
            case "Melee Crit Damage Value":
                return $"{critDamageMeleeModifierValue:+#;-#;0}";
            case "Melee CoolDown Value":
                return $"{coolDownMeleeModifierValue:+#;-#;0}";
            case "Melee Stamina Value":
                return $"{staminaMeleeModifierValue:+#;-#;0}";

            default:
                return base.GetSpecificStatValue(statKey);
        }
    }
}