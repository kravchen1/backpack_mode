using UnityEngine;

public class MeleeWeaponStats : ItemStats, IMeleeWeapon
{
    [Header("Melee Weapon Stats")]
    [SerializeField] private int minDamage = 1;
    [SerializeField] private int maxDamage = 5;
    [SerializeField] private float coolDown = 3f;
    [SerializeField] private float baseStamina = 2f;
    [SerializeField] private int accuracy = 75;
    [SerializeField] private int critChance = 10;
    [SerializeField] private int critDamage = 150;

    // Реализация IMeleeWeapon с сеттерами
    public int MinDamageMelee
    {
        get => minDamage;
        set => minDamage = value;
    }

    public int MaxDamageMelee
    {
        get => maxDamage;
        set => maxDamage = value;
    }

    public float CoolDownMelee
    {
        get => coolDown;
        set => coolDown = value;
    }

    public float BaseStaminaMelee
    {
        get => baseStamina;
        set => baseStamina = value;
    }

    public int AccuracyMelee
    {
        get => accuracy;
        set => accuracy = value;
    }

    public int CritChanceMelee
    {
        get => critChance;
        set => critChance = value;
    }

    public int CritDamageMelee
    {
        get => critDamage;
        set => critDamage = value;
    }

    public override void InitializeQuality()
    {
        base.InitializeQuality();

        float changeQualityStats2 = GetQualityMultiplier();

        minDamage = (int)(minDamage * changeQualityStats2);
        maxDamage = (int)(maxDamage * changeQualityStats2);
        coolDown *= GetInverseQualityMultiplier();
        baseStamina *= GetInverseQualityMultiplier();
        accuracy = (int)(accuracy * changeQualityStats2);
        critChance = (int)(critChance * changeQualityStats2);
        critDamage = (int)(critDamage * changeQualityStats2);
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
            new DescriptionTriple("Damage",
                $"{((minDamage + maxDamage) / coolDown):0.0}",
                $"({minDamage/qualityMultiplier:0.0}×{qualityMultiplier:0.0}({minDamage:0.0}) + {maxDamage/qualityMultiplier:0.0}×{qualityMultiplier:0.0}({maxDamage:0.0})) / ({coolDown/inverseMultiplier:0.0}×{inverseMultiplier:0.0}({coolDown:0.0}s))"),

            new DescriptionTriple("Crit Chance",
                $"{critChance}%",
                $"{critChance/qualityMultiplier:0}×{qualityMultiplier:0.0}({critChance}%)"),

            new DescriptionTriple("Crit Damage",
                $"{critDamage}%",
                $"{critDamage/qualityMultiplier:0}×{qualityMultiplier:0.0}({critDamage}%)"),

            new DescriptionTriple("Accuracy",
                $"{accuracy}",
                $"{accuracy/qualityMultiplier:0}×{qualityMultiplier:0.0}({accuracy})"),

            new DescriptionTriple("Stamina",
                $"{baseStamina/coolDown:0.0}",
                $"{baseStamina/inverseMultiplier:0.0}×{inverseMultiplier:0.0}({baseStamina:0.0}) / {coolDown/inverseMultiplier:0.0}×{inverseMultiplier:0.0}({coolDown:0.0}s)"),
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
            //case "Weapon Type":
            //    return meleeType.ToString();
            case "Damage":
                return $"{((minDamage + maxDamage) / coolDown):0.0}";
            case "Crit Chance":
                return $"{critChance}%";
            case "Crit Damage":
                return $"{critDamage}%";
            case "Accuracy":
                return $"{accuracy}";
            case "Stamina Cost":
                return $"{baseStamina:0.0}";
            default:
                return base.GetSpecificStatValue(statKey);
        }
    }

    public virtual float CalculateDPS()
    {
        return (minDamage + maxDamage) / 2f / coolDown;
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