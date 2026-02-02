using UnityEngine;

public class RangeWeaponStats : ItemStats, IRangeWeapon
{
    [Header("Range Weapon Stats")]
    [HideInInspector][SerializeField] private int minDamage = 3;
    [HideInInspector][SerializeField] private int maxDamage = 10;
    [HideInInspector][SerializeField] private float coolDown = 0.5f;
    [HideInInspector][SerializeField] private float baseStamina = 0.2f;
    [HideInInspector][SerializeField] private int accuracy = 65;
    [HideInInspector][SerializeField] private int critChance = 25;
    [HideInInspector][SerializeField] private int critDamage = 220;

    public int MinDamageRange
    {
        get => minDamage;
        set => minDamage = value;
    }

    public int MaxDamageRange
    {
        get => maxDamage;
        set => maxDamage = value;
    }

    public float CoolDownRange
    {
        get => coolDown;
        set => coolDown = value;
    }

    public float BaseStaminaRange
    {
        get => baseStamina;
        set => baseStamina = value;
    }

    public int AccuracyRange
    {
        get => accuracy;
        set => accuracy = value;
    }

    public int CritChanceRange
    {
        get => critChance;
        set => critChance = value;
    }

    public int CritDamageRange
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
}