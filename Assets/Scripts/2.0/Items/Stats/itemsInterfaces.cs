// Интерфейсы для разных типов функциональности
using Unity.VisualScripting;

public interface IMeleeWeapon
{
    int MinDamageMelee { get; set; }
    int MaxDamageMelee { get; set; }
    float CoolDownMelee { get; set; }
    float BaseStaminaMelee { get; set; }
    int AccuracyMelee { get; set; }
    int CritChanceMelee { get; set; }
    int CritDamageMelee { get; set; }

    // Дополнительные методы для механики ближнего боя
    public float CalculateDPS() => (MinDamageMelee + MaxDamageMelee) / 2f / CoolDownMelee;
    public float CalculateAvgStamina() => (BaseStaminaMelee / CoolDownMelee);
}

public interface IRangeWeapon
{
    int MinDamageRange { get; set; }
    int MaxDamageRange { get; set; }
    float CoolDownRange { get; set; }
    float BaseStaminaRange { get; set; }
    int AccuracyRange { get; set; }
    int CritChanceRange { get; set; }
    int CritDamageRange { get; set; }

    // Дополнительные методы для механики дальнего боя
    public float CalculateDPS() => (MinDamageRange + MaxDamageRange) / 2f / CoolDownRange;

    public float CalculateAvgStamina() => (BaseStaminaRange / CoolDownRange);
}