using UnityEngine;
using System.Collections;

public class ItemArmorController : ItemActionController
{
    protected ArmorStats itemArmorStats;
    public string animationKeyTakeDamage = "TakeDamage";

    protected override void Awake()
    {
        base.Awake();
        itemArmorStats = GetComponent<ArmorStats>();
    }

    /// <summary>
    /// Применяет урон к предмету, уменьшая его прочность.
    /// Возвращает остаточный урон, превышающий прочность предмета.
    /// </summary>
    /// <param name="damage">Входящий урон</param>
    /// <returns>Остаток урона, не поглощенный прочностью предмета</returns>
    public virtual int TakeDamage(int damage)
    {
        if (!CanTakeDamage())
            return damage;

        int durabilityDamage = CalculateDurabilityDamage(damage);
        int absorbedDamage = Mathf.Min((int)itemStats.durability, durabilityDamage);

        itemStats.durability -= absorbedDamage;

        return damage - CalculateActualDamageReduction(absorbedDamage);
    }

    private bool CanTakeDamage()
    {
        return itemStats.isUseFight && itemStats.durability > 0;
    }

    private int CalculateDurabilityDamage(int damage)
    {
        if (itemArmorStats.damageConsumptionPerDurability <= 0)
            return damage;

        return Mathf.CeilToInt((float)damage / itemArmorStats.damageConsumptionPerDurability);
    }

    private int CalculateActualDamageReduction(int absorbedDurabilityDamage)
    {
        return absorbedDurabilityDamage * itemArmorStats.damageConsumptionPerDurability;
    }

}