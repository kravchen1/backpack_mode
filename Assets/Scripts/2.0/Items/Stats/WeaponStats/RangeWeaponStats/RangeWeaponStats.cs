using UnityEngine;

public class RangeWeaponStats : ItemStats
{
    #region Parametres
    [Header("Range Weapon Stats")]
    [HideInInspector][SerializeField] private int minDamage = 3;
    [HideInInspector][SerializeField] private int maxDamage = 10;
    [HideInInspector][SerializeField] private float coolDown = 0.5f;
    [HideInInspector][SerializeField] private float baseStamina = 0.2f;
    [HideInInspector][SerializeField] private int critChance = 25;
    [HideInInspector][SerializeField] private int critDamage = 220;

    [HideInInspector][SerializeField] private int distanceRadius = 220;
    [HideInInspector][SerializeField] private float projectileSpeed = 20f;
    [HideInInspector][SerializeField] private float projectileSize = 1f;
    [HideInInspector][SerializeField] private int projectileCount = 1;
    [HideInInspector][SerializeField] private float spreadAngle = 0f;
    [SerializeField] private GameObject projectilePrefab;

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
    public int DistanceRadius
    {
        get => distanceRadius;
        set => distanceRadius = value;
    }
    public float ProjectileSpeed
    {
        get => projectileSpeed;
        set => projectileSpeed = value;
    }
    public float ProjectileSize
    {
        get => projectileSize;
        set => projectileSize = value;
    }
    public int ProjectileCount
    {
        get => projectileCount;
        set => projectileCount = value;
    }
    public float SpreadAngle
    {
        get => spreadAngle;
        set => spreadAngle = value;
    }
    public GameObject ProjectilePrefab
    {
        get => projectilePrefab;
        set => projectilePrefab = value;
    }
    #endregion

    // Обновляем описание
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
                $"{((MinDamageRange + MaxDamageRange) / CoolDownRange):0.0}",
                $""),

            new DescriptionTriple("Projectile Count",
                $"{projectileCount}",
                $"Снарядов за выстрел"),

            new DescriptionTriple("Spread",
                $"{spreadAngle:0.0}°",
                $"Разброс выстрела"),

            new DescriptionTriple("Crit Chance",
                $"{CritChanceRange}%",
                $""),

            new DescriptionTriple("Crit Damage",
                $"{CritDamageRange}%",
                $""),

            new DescriptionTriple("Stamina",
                $"{BaseStaminaRange/CoolDownRange:0.0}",
                $""),
            new DescriptionTriple("Durability", "", ""),
            new DescriptionTriple("Price", "", "")
        });
    }

    protected override string GetSpecificStatValue(string statKey)
    {
        switch (statKey)
        {
            default:
                return base.GetSpecificStatValue(statKey);
        }
    }
}