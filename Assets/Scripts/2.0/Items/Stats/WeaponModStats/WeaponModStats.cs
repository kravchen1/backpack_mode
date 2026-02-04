// Модификаторы оружия (прицелы, обоймы и т.д.)
using UnityEngine;

public class WeaponModStats : ItemStats
{
    //[Header("Range Weapon Mod Stats Percentage")]
    [HideInInspector] public int damageRangeModifierPercentage = 0;
    [HideInInspector] public int critChanceRangeModifierPercentage = 0;
    [HideInInspector] public int critDamageRangeModifierPercentage = 0;
    [HideInInspector] public int coolDownRangeModifierPercentage = 0;
    [HideInInspector] public int staminaRangeModifierPercentage = 0;
    [HideInInspector] public int projectileSpeedModifierPercentage = 0;
    [HideInInspector] public int projectileSizeModifierPercentage = 0;
    [HideInInspector] public int projectileCountModifierPercentage = 0;
    [HideInInspector] public int spreadAngleModifierPercentage = 0;
    //[Header("Range Weapon Mod Stats Value")]
    [HideInInspector] public int damageRangeModifierValue = 0;
    [HideInInspector] public int critChanceRangeModifierValue = 0;
    [HideInInspector] public float coolDownRangeModifierValue = 0.0f;
    [HideInInspector] public float staminaRangeModifierValue = 0.0f;
    [HideInInspector] public int critDamageRangeModifierValue = 0;
    [HideInInspector] public float projectileSpeedModifierValue = 0;
    [HideInInspector] public float projectileSizeModifierValue = 0;
    [HideInInspector] public float spreadAngleModifierValue = 0;
    [HideInInspector] public int projectileCountModifierValue = 0;

    //[Header("Melee Weapon Mod Stats Percentage")]
    [HideInInspector] public int damageMeleeModifierPercentage = 0;
    [HideInInspector] public int critChanceMeleeModifierPercentage = 0;
    [HideInInspector] public int critDamageMeleeModifierPercentage = 0;
    [HideInInspector] public int coolDownMeleeModifierPercentage = 0;
    [HideInInspector] public int staminaMeleeModifierPercentage = 0;
    //[Header("Melee Weapon Mod Stats Value")]
    [HideInInspector] public int damageMeleeModifierValue = 0;
    [HideInInspector] public int critChanceMeleeModifierValue = 0;
    [HideInInspector] public float coolDownMeleeModifierValue = 0.0f;
    [HideInInspector] public float staminaMeleeModifierValue = 0.0f;
    [HideInInspector] public int critDamageMeleeModifierValue = 0;

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
        });

        // Range Percentage modifiers
        if (damageRangeModifierPercentage != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Range Damage Percentage", $"{damageRangeModifierPercentage:+#;-#;0}%", ""));
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
            _descriptionTriples.Add(new DescriptionTriple("Range CoolDown Value", $"{coolDownRangeModifierValue:+#;-#;0.00}", ""));
        }
        if (staminaRangeModifierValue != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Range Stamina Value", $"{staminaRangeModifierValue:+#;-#;0.00}", ""));
        }

        // Melee Percentage modifiers
        if (damageMeleeModifierPercentage != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Melee Damage Percentage", $"{damageMeleeModifierPercentage:+#;-#;0}%", ""));
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
            _descriptionTriples.Add(new DescriptionTriple("Melee CoolDown Value", $"{coolDownMeleeModifierValue:+#;-#;0.00}", ""));
        }
        if (staminaMeleeModifierValue != 0)
        {
            _descriptionTriples.Add(new DescriptionTriple("Melee Stamina Value", $"{staminaMeleeModifierValue:+#;-#;0.00}", ""));
        }

        _descriptionTriples.Add(new DescriptionTriple("Price", "", ""));
    }

    protected override string GetSpecificStatValue(string statKey)
    {
        switch (statKey)
        {
            // Range Percentage
            case "Range Damage Percentage":
                return $"{damageRangeModifierPercentage:+#;-#;0}%";
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
            case "Range Crit Chance Value":
                return $"{critChanceRangeModifierValue:+#;-#;0}";
            case "Range Crit Damage Value":
                return $"{critDamageRangeModifierValue:+#;-#;0}";
            case "Range CoolDown Value":
                return $"{coolDownRangeModifierValue:+0.##;-0.##}";
            case "Range Stamina Value":
                return $"{staminaRangeModifierValue:+0.##;-0.##}";

            // Melee Percentage
            case "Melee Damage Percentage":
                return $"{damageMeleeModifierPercentage:+#;-#;0}%";
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
            case "Melee Crit Chance Value":
                return $"{critChanceMeleeModifierValue:+#;-#;0}";
            case "Melee Crit Damage Value":
                return $"{critDamageMeleeModifierValue:+#;-#;0}";
            case "Melee CoolDown Value":
                return $"{coolDownMeleeModifierValue:+0.##;-0.##}";
            case "Melee Stamina Value":
                return $"{staminaMeleeModifierValue:+0.##;-0.##}";

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

        damageRangeModifierPercentage = dataManager.GetItemData(itemKey, "damageRangeModifierPercentage", damageRangeModifierPercentage);
        critChanceRangeModifierPercentage = dataManager.GetItemData(itemKey, "critChanceRangeModifierPercentage", critChanceRangeModifierPercentage);
        critDamageRangeModifierPercentage = dataManager.GetItemData(itemKey, "critDamageRangeModifierPercentage", critDamageRangeModifierPercentage);
        coolDownRangeModifierPercentage = dataManager.GetItemData(itemKey, "coolDownRangeModifierPercentage", coolDownRangeModifierPercentage);
        staminaRangeModifierPercentage = dataManager.GetItemData(itemKey, "staminaRangeModifierPercentage", staminaRangeModifierPercentage);

        damageRangeModifierValue = dataManager.GetItemData(itemKey, "damageRangeModifierValue", damageRangeModifierValue);
        critChanceRangeModifierValue = dataManager.GetItemData(itemKey, "critChanceRangeModifierValue", critChanceRangeModifierValue);
        critDamageRangeModifierValue = dataManager.GetItemData(itemKey, "critDamageRangeModifierValue", critDamageRangeModifierValue);
        coolDownRangeModifierValue = dataManager.GetItemData(itemKey, "coolDownRangeModifierValue", 0.0f);
        staminaRangeModifierValue = dataManager.GetItemData(itemKey, "staminaRangeModifierValue", 0.0f);

        damageMeleeModifierPercentage = dataManager.GetItemData(itemKey, "damageMeleeModifierPercentage", damageMeleeModifierPercentage);
        critChanceMeleeModifierPercentage = dataManager.GetItemData(itemKey, "critChanceMeleeModifierPercentage", critChanceMeleeModifierPercentage);
        critDamageMeleeModifierPercentage = dataManager.GetItemData(itemKey, "critDamageMeleeModifierPercentage", critDamageMeleeModifierPercentage);
        coolDownMeleeModifierPercentage = dataManager.GetItemData(itemKey, "coolDownMeleeModifierPercentage", coolDownMeleeModifierPercentage);
        staminaMeleeModifierPercentage = dataManager.GetItemData(itemKey, "staminaMeleeModifierPercentage", staminaMeleeModifierPercentage);

        damageMeleeModifierValue = dataManager.GetItemData(itemKey, "damageMeleeModifierValue", damageMeleeModifierValue);
        critChanceMeleeModifierValue = dataManager.GetItemData(itemKey, "critChanceMeleeModifierValue", critChanceMeleeModifierValue);
        critDamageMeleeModifierValue = dataManager.GetItemData(itemKey, "critDamageMeleeModifierValue", critDamageMeleeModifierValue);
        coolDownMeleeModifierValue = dataManager.GetItemData(itemKey, "coolDownMeleeModifierValue", 0.0f);
        staminaMeleeModifierValue = dataManager.GetItemData(itemKey, "staminaMeleeModifierValue", 0.0f);
    }
}