using UnityEngine;

public class MeleeAndRangeWeaponStats : ItemStats, IMeleeWeapon, IRangeWeapon
{
    [Header("Melee Weapon Stats")]
    [SerializeField] private float minDamageMelee = 1f;
    [SerializeField] private float maxDamageMelee = 5f;
    [SerializeField] private float coolDownMelee = 3f;
    [SerializeField] private float baseStaminaMelee = 2f;
    [SerializeField] private int accuracyMelee = 75;
    [SerializeField] private int critChanceMelee = 10;
    [SerializeField] private int critDamageMelee = 180;
    [SerializeField] private bool canParry = true;
    [SerializeField] private float parryWindow = 0.3f;

    [Header("Range Weapon Stats")]
    [SerializeField] private float minDamageRange = 3f;
    [SerializeField] private float maxDamageRange = 10f;
    [SerializeField] private float coolDownRange = 0.5f;
    [SerializeField] private float baseStaminaRange = 0.2f;
    [SerializeField] private int accuracyRange = 65;
    [SerializeField] private int critChanceRange = 25;
    [SerializeField] private int critDamageRange = 220;
    [SerializeField] private int ammoCapacity = 30;
    [SerializeField] private float reloadTime = 2.0f;
    [SerializeField] private bool requiresAmmo = true;

    public enum WeaponMode { Melee, Range }

    // Реализация IMeleeWeapon
    public float MinDamageMelee => minDamageMelee;
    public float MaxDamageMelee => maxDamageMelee;
    public float CoolDownMelee => coolDownMelee;
    public float BaseStaminaMelee => baseStaminaMelee;
    public int AccuracyMelee => accuracyMelee;
    public int CritChanceMelee => critChanceMelee;
    public int CritDamageMelee => critDamageMelee;
    public bool CanParry => canParry;
    public float ParryWindow => parryWindow;

    // Реализация IRangeWeapon
    public float MinDamageRange => minDamageRange;
    public float MaxDamageRange => maxDamageRange;
    public float CoolDownRange => coolDownRange;
    public float BaseStaminaRange => baseStaminaRange;
    public int AccuracyRange => accuracyRange;
    public int CritChanceRange => critChanceRange;
    public int CritDamageRange => critDamageRange;
    public int AmmoCapacity => ammoCapacity;
    public float ReloadTime => reloadTime;
    public bool RequiresAmmo => requiresAmmo;

    public override void InitializeQuality()
    {
        base.InitializeQuality();

        float qualityMultiplier = GetQualityMultiplier();
        float inverseMultiplier = GetInverseQualityMultiplier();

        // Применяем качество к характеристикам ближнего боя
        minDamageMelee *= qualityMultiplier;
        maxDamageMelee *= qualityMultiplier;
        coolDownMelee *= inverseMultiplier;
        baseStaminaMelee *= inverseMultiplier;
        accuracyMelee = (int)(accuracyMelee * qualityMultiplier);
        critChanceMelee = (int)(critChanceMelee * qualityMultiplier);
        critDamageMelee = (int)(critDamageMelee * qualityMultiplier);

        // Применяем качество к характеристикам дальнего боя
        minDamageRange *= qualityMultiplier;
        maxDamageRange *= qualityMultiplier;
        coolDownRange *= inverseMultiplier;
        baseStaminaRange *= inverseMultiplier;
        accuracyRange = (int)(accuracyRange * qualityMultiplier);
        critChanceRange = (int)(critChanceRange * qualityMultiplier);
        critDamageRange = (int)(critDamageRange * qualityMultiplier);
    }

    public override void InitializeDescriptionTriples()
    {
        if (_descriptionTriples.Count > 0)
        {
            _descriptionTriples.Clear();
        }

        float qualityMultiplier = GetQualityMultiplier();
        float inverseMultiplier = GetInverseQualityMultiplier();

        _descriptionTriples.AddRange(new[]
        {
        new DescriptionTriple("Type", "", ""),
        new DescriptionTriple("Rarity", "", ""),
        new DescriptionTriple("Quality", "", ""),
        
        // Ближний бой
        new DescriptionTriple("Melee Damage",
            $"{((minDamageMelee + maxDamageMelee) / coolDownMelee):0.0}",
            $"({minDamageMelee/qualityMultiplier:0.0}×{qualityMultiplier:0.0}({minDamageMelee:0.0}) + {maxDamageMelee/qualityMultiplier:0.0}×{qualityMultiplier:0.0}({maxDamageMelee:0.0})) / ({coolDownMelee/inverseMultiplier:0.0}×{inverseMultiplier:0.0}({coolDownMelee:0.0}s))"),

        new DescriptionTriple("Melee Crit Chance",
            $"{critChanceMelee}%",
            $"{critChanceMelee/qualityMultiplier:0}×{qualityMultiplier:0.0}({critChanceMelee}%)"),

        new DescriptionTriple("Melee Crit Damage",
            $"{critDamageMelee}%",
            $"{critDamageMelee/qualityMultiplier:0}×{qualityMultiplier:0.0}({critDamageMelee}%)"),

        new DescriptionTriple("Melee Accuracy",
            $"{accuracyMelee}",
            $"{accuracyMelee/qualityMultiplier:0}×{qualityMultiplier:0.0}({accuracyMelee})"),

        new DescriptionTriple("Melee Stamina",
            $"{baseStaminaMelee/coolDownMelee:0.0}",
            $"{baseStaminaMelee/inverseMultiplier:0.0}×{inverseMultiplier:0.0}({baseStaminaMelee:0.0}) / {coolDownMelee/inverseMultiplier:0.0}×{inverseMultiplier:0.0}({coolDownMelee:0.0}s)"),
        
        // Дальний бой
        new DescriptionTriple("Range Damage",
            $"{((minDamageRange + maxDamageRange) / coolDownRange):0.0}",
            $"({minDamageRange/qualityMultiplier:0.0}×{qualityMultiplier:0.0}({minDamageRange:0.0}) + {maxDamageRange/qualityMultiplier:0.0}×{qualityMultiplier:0.0}({maxDamageRange:0.0})) / ({coolDownRange/inverseMultiplier:0.0}×{inverseMultiplier:0.0}({coolDownRange:0.0}s))"),

        new DescriptionTriple("Range Crit Chance",
            $"{critChanceRange}%",
            $"{critChanceRange/qualityMultiplier:0}×{qualityMultiplier:0.0}({critChanceRange}%)"),

        new DescriptionTriple("Range Crit Damage",
            $"{critDamageRange}%",
            $"{critDamageRange/qualityMultiplier:0}×{qualityMultiplier:0.0}({critDamageRange}%)"),

        new DescriptionTriple("Range Accuracy",
            $"{accuracyRange}",
            $"{accuracyRange/qualityMultiplier:0}×{qualityMultiplier:0.0}({accuracyRange})"),

        new DescriptionTriple("Range Stamina",
            $"{baseStaminaRange/coolDownRange:0.0}",
            $"{baseStaminaRange/inverseMultiplier:0.0}×{inverseMultiplier:0.0}({baseStaminaRange:0.0}) / {coolDownRange/inverseMultiplier:0.0}×{inverseMultiplier:0.0}({coolDownRange:0.0}s)"),
        
        // Общие
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
            // Ближний бой
            case "Melee Damage":
                return $"{((minDamageMelee + maxDamageMelee) / coolDownMelee):0.0}";
            case "Melee Crit Chance":
                return $"{critChanceMelee}%";
            case "Melee Crit Damage":
                return $"{critDamageMelee}%";
            case "Melee Accuracy":
                return $"{accuracyMelee}";
            case "Melee Stamina":
                return $"{baseStaminaMelee:0.0}";

            // Дальний бой
            case "Range Damage":
                return $"{((minDamageRange + maxDamageRange) / coolDownRange):0.0}";
            case "Range Crit Chance":
                return $"{critChanceRange}%";
            case "Range Crit Damage":
                return $"{critDamageRange}%";
            case "Range Accuracy":
                return $"{accuracyRange}";
            case "Range Stamina":
                return $"{baseStaminaRange:0.0}";

            default:
                return base.GetSpecificStatValue(statKey);
        }
    }

    private float GetQualityMultiplier()
    {
        return itemQuality switch
        {
            ItemQuality.VeryBad => 0.6f,
            ItemQuality.Bad => 0.8f,
            ItemQuality.Good => 1.2f,
            ItemQuality.Excellent => 1.4f,
            _ => 1f
        };
    }

    private float GetInverseQualityMultiplier()
    {
        return itemQuality switch
        {
            ItemQuality.VeryBad => 1.4f,
            ItemQuality.Bad => 1.2f,
            ItemQuality.Good => 0.8f,
            ItemQuality.Excellent => 0.6f,
            _ => 1f
        };
    }
}
