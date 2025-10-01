// Интерфейсы для разных типов функциональности
using Unity.VisualScripting;

public interface IMeleeWeapon
{
    float MinDamageMelee { get; }
    float MaxDamageMelee { get; }
    float CoolDownMelee { get; }
    float BaseStaminaMelee { get; }
    int AccuracyMelee { get; }
    int CritChanceMelee { get; }
    int CritDamageMelee { get; }

    // Дополнительные методы для механики ближнего боя
    public float CalculateDPS() => (MinDamageMelee + MaxDamageMelee) / 2f / CoolDownMelee;
    public float CalculateAvgStamina() => (BaseStaminaMelee / CoolDownMelee);
}

public interface IRangeWeapon
{
    float MinDamageRange { get; }
    float MaxDamageRange { get; }
    float CoolDownRange { get; }
    float BaseStaminaRange { get; }
    int AccuracyRange { get; }
    int CritChanceRange { get; }
    int CritDamageRange { get; }

    // Дополнительные методы для механики дальнего боя
    public float CalculateDPS() => (MinDamageRange + MaxDamageRange) / 2f / CoolDownRange;

    public float CalculateAvgStamina() => (BaseStaminaRange / CoolDownRange);
}