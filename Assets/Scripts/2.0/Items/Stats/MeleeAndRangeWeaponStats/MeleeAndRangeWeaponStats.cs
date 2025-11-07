using UnityEngine;

public class MeleeAndRangeWeaponStats : ItemStats, IMeleeWeapon, IRangeWeapon
{
    #region Serialized Fields - Melee Weapon Stats
    [Header("Melee Weapon Stats")]
    [HideInInspector][SerializeField] private int minDamageMelee = 1;
    [HideInInspector][SerializeField] private int maxDamageMelee = 5;
    [HideInInspector][SerializeField] private float coolDownMelee = 3f;
    [HideInInspector][SerializeField] private float baseStaminaMelee = 2f;
    [HideInInspector][SerializeField] private int accuracyMelee = 75;
    [HideInInspector][SerializeField] private int critChanceMelee = 10;
    [HideInInspector][SerializeField] private int critDamageMelee = 180;
    #endregion

    #region Serialized Fields - Range Weapon Stats
    [Header("Range Weapon Stats")]
    [HideInInspector][SerializeField] private int minDamageRange = 3;
    [HideInInspector][SerializeField] private int maxDamageRange = 10;
    [HideInInspector][SerializeField] private float coolDownRange = 0.5f;
    [HideInInspector][SerializeField] private float baseStaminaRange = 0.2f;
    [HideInInspector][SerializeField] private int accuracyRange = 65;
    [HideInInspector][SerializeField] private int critChanceRange = 25;
    [HideInInspector][SerializeField] private int critDamageRange = 220;
    #endregion

    #region IMeleeWeapon Implementation
    public int MinDamageMelee
    {
        get => minDamageMelee;
        set => minDamageMelee = value;
    }

    public int MaxDamageMelee
    {
        get => maxDamageMelee;
        set => maxDamageMelee = value;
    }

    public float CoolDownMelee
    {
        get => coolDownMelee;
        set => coolDownMelee = value;
    }

    public float BaseStaminaMelee
    {
        get => baseStaminaMelee;
        set => baseStaminaMelee = value;
    }

    public int AccuracyMelee
    {
        get => accuracyMelee;
        set => accuracyMelee = value;
    }

    public int CritChanceMelee
    {
        get => critChanceMelee;
        set => critChanceMelee = value;
    }

    public int CritDamageMelee
    {
        get => critDamageMelee;
        set => critDamageMelee = value;
    }
    #endregion

    #region IRangeWeapon Implementation
    public int MinDamageRange
    {
        get => minDamageRange;
        set => minDamageRange = value;
    }

    public int MaxDamageRange
    {
        get => maxDamageRange;
        set => maxDamageRange = value;
    }

    public float CoolDownRange
    {
        get => coolDownRange;
        set => coolDownRange = value;
    }

    public float BaseStaminaRange
    {
        get => baseStaminaRange;
        set => baseStaminaRange = value;
    }

    public int AccuracyRange
    {
        get => accuracyRange;
        set => accuracyRange = value;
    }

    public int CritChanceRange
    {
        get => critChanceRange;
        set => critChanceRange = value;
    }

    public int CritDamageRange
    {
        get => critDamageRange;
        set => critDamageRange = value;
    }
    #endregion

    #region Quality Methods
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
    #endregion

    #region Override Methods
    public override void InitializeQuality()
    {
        base.InitializeQuality();

        float qualityMultiplier = GetQualityMultiplier();
        float inverseMultiplier = GetInverseQualityMultiplier();

        // Применяем качество к характеристикам ближнего боя
        minDamageMelee = (int)(minDamageMelee * qualityMultiplier);
        maxDamageMelee = (int)(maxDamageMelee * qualityMultiplier);
        coolDownMelee *= inverseMultiplier;
        baseStaminaMelee *= inverseMultiplier;
        accuracyMelee = (int)(accuracyMelee * qualityMultiplier);
        critChanceMelee = (int)(critChanceMelee * qualityMultiplier);
        critDamageMelee = (int)(critDamageMelee * qualityMultiplier);

        // Применяем качество к характеристикам дальнего боя
        minDamageRange = (int)(minDamageRange * qualityMultiplier);
        maxDamageRange = (int)(maxDamageRange * qualityMultiplier);
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
            new DescriptionTriple("Description", "", ""),
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
            //new DescriptionTriple("Requirements", "", ""),
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

    protected override void LoadFromDataManager()
    {
        base.LoadFromDataManager();

        if (string.IsNullOrEmpty(itemKey)) return;
        var dataManager = ItemDataManager.Instance;
        if (dataManager == null) return;

        // Загрузка параметров ближнего боя
        minDamageMelee = dataManager.GetItemData(itemKey, "minDamageMelee", minDamageMelee);
        maxDamageMelee = dataManager.GetItemData(itemKey, "maxDamageMelee", maxDamageMelee);
        coolDownMelee = dataManager.GetItemData(itemKey, "coolDownMelee", coolDownMelee);
        baseStaminaMelee = dataManager.GetItemData(itemKey, "baseStaminaMelee", baseStaminaMelee);
        accuracyMelee = dataManager.GetItemData(itemKey, "accuracyMelee", accuracyMelee);
        critChanceMelee = dataManager.GetItemData(itemKey, "critChanceMelee", critChanceMelee);
        critDamageMelee = dataManager.GetItemData(itemKey, "critDamageMelee", critDamageMelee);

        // Загрузка параметров дальнего боя
        minDamageRange = dataManager.GetItemData(itemKey, "minDamageRange", minDamageRange);
        maxDamageRange = dataManager.GetItemData(itemKey, "maxDamageRange", maxDamageRange);
        coolDownRange = dataManager.GetItemData(itemKey, "coolDownRange", coolDownRange);
        baseStaminaRange = dataManager.GetItemData(itemKey, "baseStaminaRange", baseStaminaRange);
        accuracyRange = dataManager.GetItemData(itemKey, "accuracyRange", accuracyRange);
        critChanceRange = dataManager.GetItemData(itemKey, "critChanceRange", critChanceRange);
        critDamageRange = dataManager.GetItemData(itemKey, "critDamageRange", critDamageRange);
    }
    #endregion
}