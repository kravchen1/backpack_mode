// ���������� ��� ������ ����� ����������������
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

    // �������������� ������ ��� �������� �������� ���
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

    // �������������� ������ ��� �������� �������� ���
    public float CalculateDPS() => (MinDamageRange + MaxDamageRange) / 2f / CoolDownRange;

    public float CalculateAvgStamina() => (BaseStaminaRange / CoolDownRange);
}