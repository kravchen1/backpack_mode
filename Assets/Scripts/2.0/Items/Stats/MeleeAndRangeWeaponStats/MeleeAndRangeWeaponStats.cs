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
    [SerializeField] private int critDamageMelee = 150;
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

    [Header("Combined Weapon Settings")]
    public bool canSwitchModes = true;
    public float modeSwitchTime = 1.5f;
    public WeaponMode currentMode = WeaponMode.Range;

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
        coolDownMelee *= qualityMultiplier;
        baseStaminaMelee *= inverseMultiplier;
        accuracyMelee = (int)(accuracyMelee * qualityMultiplier);
        critChanceMelee = (int)(critChanceMelee * qualityMultiplier);
        critDamageMelee = (int)(critDamageMelee * qualityMultiplier);

        // Применяем качество к характеристикам дальнего боя
        minDamageRange *= qualityMultiplier;
        maxDamageRange *= qualityMultiplier;
        coolDownRange *= qualityMultiplier;
        baseStaminaRange *= inverseMultiplier;
        accuracyRange = (int)(accuracyRange * qualityMultiplier);
        critChanceRange = (int)(critChanceRange * qualityMultiplier);
        critDamageRange = (int)(critDamageRange * qualityMultiplier);
    }

    protected override void InitializeDescriptionTriples()
    {
        _descriptionTriples.AddRange(new[]
        {
            new DescriptionTriple("Type", "", ""),
            new DescriptionTriple("Rarity", "", ""),
            new DescriptionTriple("Quality", "", ""),
            new DescriptionTriple("Current Mode", currentMode.ToString(), ""),
            new DescriptionTriple("Mode Switch Time", $"{modeSwitchTime:0.0}s", ""),
            
            // Ближний бой
            new DescriptionTriple("Melee Damage", $"{((minDamageMelee + maxDamageMelee) / coolDownMelee):0.0}", $"({minDamageMelee:0.0} - {maxDamageMelee:0.0}) / {coolDownMelee:0.0}s"),
            new DescriptionTriple("Melee Crit Chance", $"{critChanceMelee}%", ""),
            new DescriptionTriple("Melee Crit Damage", $"{critDamageMelee}%", ""),
            new DescriptionTriple("Melee Accuracy", $"{accuracyMelee}", ""),
            new DescriptionTriple("Melee Stamina", $"{baseStaminaMelee:0.0}", ""),
            
            // Дальний бой
            new DescriptionTriple("Range Damage", $"{((minDamageRange + maxDamageRange) / coolDownRange):0.0}", $"({minDamageRange:0.0} - {maxDamageRange:0.0}) / {coolDownRange:0.0}s"),
            new DescriptionTriple("Range Crit Chance", $"{critChanceRange}%", ""),
            new DescriptionTriple("Range Crit Damage", $"{critDamageRange}%", ""),
            new DescriptionTriple("Range Accuracy", $"{accuracyRange}", ""),
            new DescriptionTriple("Range Stamina", $"{baseStaminaRange:0.0}", ""),
            
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
            case "Current Mode":
                return currentMode.ToString();
            case "Mode Switch Time":
                return $"{modeSwitchTime:0.0}s";

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

    // Методы для смены режима
    public void SwitchToMeleeMode()
    {
        if (canSwitchModes)
        {
            currentMode = WeaponMode.Melee;
            Debug.Log($"{itemNameKey} switched to Melee mode");
        }
    }

    public void SwitchToRangeMode()
    {
        if (canSwitchModes)
        {
            currentMode = WeaponMode.Range;
            Debug.Log($"{itemNameKey} switched to Range mode");
        }
    }

    public void ToggleMode()
    {
        if (!canSwitchModes) return;

        currentMode = currentMode == WeaponMode.Melee ? WeaponMode.Range : WeaponMode.Melee;
        Debug.Log($"{itemNameKey} toggled to {currentMode} mode");
    }

    // Метод для получения текущих характеристик
    public (float minDmg, float maxDmg, float cd, float stamina, int acc, int critChance, int critDmg) GetCurrentModeStats()
    {
        return currentMode switch
        {
            WeaponMode.Melee => (minDamageMelee, maxDamageMelee, coolDownMelee, baseStaminaMelee, accuracyMelee, critChanceMelee, critDamageMelee),
            WeaponMode.Range => (minDamageRange, maxDamageRange, coolDownRange, baseStaminaRange, accuracyRange, critChanceRange, critDamageRange),
            _ => (minDamageRange, maxDamageRange, coolDownRange, baseStaminaRange, accuracyRange, critChanceRange, critDamageRange)
        };
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
