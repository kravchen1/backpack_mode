using UnityEngine;

public class RangeWeaponStats : ItemStats, IRangeWeapon
{
    [Header("Range Weapon Stats")]
    [SerializeField] private float minDamage = 3f;
    [SerializeField] private float maxDamage = 10f;
    [SerializeField] private float coolDown = 0.5f;
    [SerializeField] private float baseStamina = 0.2f;
    [SerializeField] private int accuracy = 65;
    [SerializeField] private int critChance = 25;
    [SerializeField] private int critDamage = 220;

    [Header("Range Specific Stats")]
    public bool requiresAmmo = true;

    public enum RangeWeaponType { Pistol, Rifle, Shotgun, Sniper, Bow }
    public enum AmmoType { Pistol, AssaultRifle, Shotgun, Sniper, Arrows }

    // Реализация IRangeWeapon
    public float MinDamageRange => minDamage;
    public float MaxDamageRange => maxDamage;
    public float CoolDownRange => coolDown;
    public float BaseStaminaRange => baseStamina;
    public int AccuracyRange => accuracy;
    public int CritChanceRange => critChance;
    public int CritDamageRange => critDamage;
    public bool RequiresAmmo => requiresAmmo;

    public override void InitializeQuality()
    {
        base.InitializeQuality();

        float changeQualityStats2 = GetQualityMultiplier();

        minDamage *= changeQualityStats2;
        maxDamage *= changeQualityStats2;
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

            new DescriptionTriple("Requires Ammo", requiresAmmo ? "Yes" : "No", ""),
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
            case "Requires Ammo":
                return requiresAmmo ? "Yes" : "No";
            default:
                return base.GetSpecificStatValue(statKey);
        }
    }

    // Специфичные методы для дальнего боя
    public virtual bool CanShoot(int currentAmmo)
    {
        if (requiresAmmo && currentAmmo <= 0) return false;
        return true;
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