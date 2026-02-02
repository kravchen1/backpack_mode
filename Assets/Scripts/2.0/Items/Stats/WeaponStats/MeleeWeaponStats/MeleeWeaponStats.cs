using UnityEngine;

public class MeleeWeaponStats : ItemStats, IMeleeWeapon
{
    [Header("Melee Weapon Stats")]
    [HideInInspector][SerializeField] private int minDamage = 1;
    [HideInInspector][SerializeField] private int maxDamage = 5;
    [HideInInspector][SerializeField] private float coolDown = 3f;
    [HideInInspector][SerializeField] private float baseStamina = 2f;
    [HideInInspector][SerializeField] private int accuracy = 75;
    [HideInInspector][SerializeField] private int critChance = 10;
    [HideInInspector][SerializeField] private int critDamage = 150;

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
            new DescriptionTriple("Damage",
                $"{((minDamage + maxDamage) / coolDown):0.0}",
                $""),

            new DescriptionTriple("Crit Chance",
                $"{critChance}%",
                $""),

            new DescriptionTriple("Crit Damage",
                $"{critDamage}%",
                $""),

            new DescriptionTriple("Accuracy",
                $"{accuracy}",
                $""),

            new DescriptionTriple("Stamina",
                $"{baseStamina/coolDown:0.0}",
                $""),
            new DescriptionTriple("Durability", "", ""),
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
}