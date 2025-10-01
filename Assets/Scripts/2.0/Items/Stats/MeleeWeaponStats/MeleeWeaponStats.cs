using UnityEngine;

public class MeleeWeaponStats : ItemStats, IMeleeWeapon
{
    [Header("Melee Weapon Stats")]
    [SerializeField] private float minDamage = 1f;
    [SerializeField] private float maxDamage = 5f;
    [SerializeField] private float coolDown = 3f;
    [SerializeField] private float baseStamina = 2f;
    [SerializeField] private int accuracy = 75;
    [SerializeField] private int critChance = 10;
    [SerializeField] private int critDamage = 150;

    //[Header("Melee Specific Stats")]
    //public MeleeType meleeType = MeleeType.Sword;

    public enum MeleeType { Sword, Axe, Mace, Dagger, Polearm }

    // Реализация IMeleeWeapon
    public float MinDamageMelee => minDamage;
    public float MaxDamageMelee => maxDamage;
    public float CoolDownMelee => coolDown;
    public float BaseStaminaMelee => baseStamina;
    public int AccuracyMelee => accuracy;
    public int CritChanceMelee => critChance;
    public int CritDamageMelee => critDamage;
    //public bool CanParry => canParry;
    //public float ParryWindow => parryWindow;

    public override void InitializeQuality()
    {
        base.InitializeQuality();

        float changeQualityStats2 = GetQualityMultiplier();

        minDamage *= changeQualityStats2;
        maxDamage *= changeQualityStats2;
        coolDown *= changeQualityStats2;
        baseStamina *= GetInverseQualityMultiplier();
        accuracy = (int)(accuracy * changeQualityStats2);
        critChance = (int)(critChance * changeQualityStats2);
        critDamage = (int)(critDamage * changeQualityStats2);
    }

    protected override void InitializeDescriptionTriples()
    {
        _descriptionTriples.AddRange(new[]
        {
            new DescriptionTriple("Type", "", ""),
            new DescriptionTriple("Rarity", "", ""),
            new DescriptionTriple("Quality", "", ""),
            //new DescriptionTriple("Weapon Type", meleeType.ToString(), ""),
            new DescriptionTriple("Damage", $"{((minDamage + maxDamage) / coolDown):0.0}", $"({minDamage:0.0} - {maxDamage:0.0}) / {coolDown:0.0}s"),
            new DescriptionTriple("Crit Chance", $"{critChance}%", ""),
            new DescriptionTriple("Crit Damage", $"{critDamage}%", ""),
            new DescriptionTriple("Accuracy", $"{accuracy}", ""),
            new DescriptionTriple("Stamina Cost", $"{baseStamina:0.0}", ""),
            //new DescriptionTriple("Can Parry", canParry ? "Yes" : "No", ""),
            //new DescriptionTriple("Parry Window", $"{parryWindow:0.0}s", ""),
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
            //case "Can Parry":
            //    return canParry ? "Yes" : "No";
            //case "Parry Window":
            //    return $"{parryWindow:0.0}s";
            default:
                return base.GetSpecificStatValue(statKey);
        }
    }

    // Специфичные методы для ближнего боя
    //public virtual bool TryParry()
    //{
    //    if (!canParry) return false;
    //    // Логика парирования
    //    Debug.Log($"Parry attempted with {itemNameKey}, window: {parryWindow}s");
    //    return true;
    //}

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