using UnityEngine;

public class ItemActionModifyWeaponController : ItemActionModifyController
{
    #region Fields and Properties
    [SerializeField] WeaponModStats weaponModStats;
    public bool isMelee = false;
    public bool isRange = false;
    #endregion

    #region Initialization
    protected override void Awake()
    {
        base.Awake();
        weaponModStats = GetComponent<WeaponModStats>();
    }
    #endregion

    #region Public Modification Methods
    public override void ModifyEnableItem(GameObject item)
    {
        if (itemStats.durability > 0)
        {
            itemStats.durability--;
            if (isMelee && isRange)
            {
                var stat = item.GetComponent<MeleeAndRangeWeaponStats>();
                if (stat != null)
                {
                    ModifyMeleeAndRangeWeapon(stat);
                }
                return;
            }
            if (isMelee)
            {
                var stat = item.GetComponent<MeleeWeaponStats>();
                if (stat != null)
                {
                    ModifyMeleeWeapon(stat);
                }
                return;
            }
            if (isRange)
            {
                var stat = item.GetComponent<RangeWeaponStats>();
                if (stat != null)
                {
                    ModifyRangeWeapon(stat);
                }
                return;
            }
            else
            {
                Debug.Log("у модификатора не выбран тип влияния на оружие");
            }
        }
    }

    public override void ModifyDisableItem(GameObject item)
    {
        if (isMelee && isRange)
        {
            var stat = item.GetComponent<MeleeAndRangeWeaponStats>();
            if (stat != null)
            {
                RevertMeleeAndRangeWeapon(stat);
            }
        }
        else if (isMelee)
        {
            var stat = item.GetComponent<MeleeWeaponStats>();
            if (stat != null)
            {
                RevertMeleeWeapon(stat);
            }
        }
        else if (isRange)
        {
            var stat = item.GetComponent<RangeWeaponStats>();
            if (stat != null)
            {
                RevertRangeWeapon(stat);
            }
        }
    }
    #endregion

    #region Percentage Modification Methods
    private int ApplyPercentageModifier(int currentValue, int percentageModifier)
    {
        if (percentageModifier != 0)
        {
            float newValue = currentValue * (1 + percentageModifier / 100.0f);
            return Mathf.Max(1, (int)newValue);
        }
        return currentValue;
    }

    private float ApplyPercentageModifier(float currentValue, int percentageModifier)
    {
        if (percentageModifier != 0)
        {
            float newValue = currentValue * (1 + percentageModifier / 100.0f);
            return Mathf.Max(0.1f, newValue);
        }
        return currentValue;
    }

    private int RevertPercentageModifier(int currentValue, int percentageModifier)
    {
        if (percentageModifier != 0)
        {
            float originalValue = currentValue / (1 + percentageModifier / 100.0f);
            return Mathf.Max(1, (int)originalValue);
        }
        return currentValue;
    }

    private float RevertPercentageModifier(float currentValue, int percentageModifier)
    {
        if (percentageModifier != 0)
        {
            float originalValue = currentValue / (1 + percentageModifier / 100.0f);
            return Mathf.Max(0.1f, originalValue);
        }
        return currentValue;
    }
    #endregion

    #region Value Modification Methods
    private int ApplyValueModifier(int currentValue, int valueModifier)
    {
        if (valueModifier != 0)
        {
            return Mathf.Max(1, currentValue + valueModifier);
        }
        return currentValue;
    }

    private float ApplyValueModifier(float currentValue, float valueModifier)
    {
        if (valueModifier != 0)
        {
            return Mathf.Max(0.1f, currentValue + valueModifier);
        }
        return currentValue;
    }

    private float ApplyValueModifier(float currentValue, int valueModifier)
    {
        if (valueModifier != 0)
        {
            return Mathf.Max(0.1f, currentValue + valueModifier);
        }
        return currentValue;
    }

    private int RevertValueModifier(int currentValue, int valueModifier)
    {
        if (valueModifier != 0)
        {
            return Mathf.Max(1, currentValue - valueModifier);
        }
        return currentValue;
    }

    private float RevertValueModifier(float currentValue, float valueModifier)
    {
        if (valueModifier != 0)
        {
            return Mathf.Max(0.1f, currentValue - valueModifier);
        }
        return currentValue;
    }

    private float RevertValueModifier(float currentValue, int valueModifier)
    {
        if (valueModifier != 0)
        {
            return Mathf.Max(0.1f, currentValue - valueModifier);
        }
        return currentValue;
    }
    #endregion

    #region MeleeAndRange Weapon Modification
    public void ModifyMeleeAndRangeWeapon(MeleeAndRangeWeaponStats stat)
    {
        // Range Percentage modifiers
        if (weaponModStats.damageRangeModifierPercentage != 0)
        {
            stat.MinDamageRange = ApplyPercentageModifier(stat.MinDamageRange, weaponModStats.damageRangeModifierPercentage);
            stat.MaxDamageRange = ApplyPercentageModifier(stat.MaxDamageRange, weaponModStats.damageRangeModifierPercentage);
        }

        if (weaponModStats.accuracyRangeModifierPercentage != 0)
        {
            stat.AccuracyRange = ApplyPercentageModifier(stat.AccuracyRange, weaponModStats.accuracyRangeModifierPercentage);
        }

        if (weaponModStats.critChanceRangeModifierPercentage != 0)
        {
            stat.CritChanceRange = ApplyPercentageModifier(stat.CritChanceRange, weaponModStats.critChanceRangeModifierPercentage);
        }

        if (weaponModStats.critDamageRangeModifierPercentage != 0)
        {
            stat.CritDamageRange = ApplyPercentageModifier(stat.CritDamageRange, weaponModStats.critDamageRangeModifierPercentage);
        }

        if (weaponModStats.coolDownRangeModifierPercentage != 0)
        {
            stat.CoolDownRange = ApplyPercentageModifier(stat.CoolDownRange, weaponModStats.coolDownRangeModifierPercentage);
        }

        if (weaponModStats.staminaRangeModifierPercentage != 0)
        {
            stat.BaseStaminaRange = ApplyPercentageModifier(stat.BaseStaminaRange, weaponModStats.staminaRangeModifierPercentage);
        }

        // Range Value modifiers
        if (weaponModStats.damageRangeModifierValue != 0)
        {
            stat.MinDamageRange = ApplyValueModifier(stat.MinDamageRange, weaponModStats.damageRangeModifierValue);
            stat.MaxDamageRange = ApplyValueModifier(stat.MaxDamageRange, weaponModStats.damageRangeModifierValue);
        }

        if (weaponModStats.accuracyRangeModifierValue != 0)
        {
            stat.AccuracyRange = ApplyValueModifier(stat.AccuracyRange, weaponModStats.accuracyRangeModifierValue);
        }

        if (weaponModStats.critChanceRangeModifierValue != 0)
        {
            stat.CritChanceRange = ApplyValueModifier(stat.CritChanceRange, weaponModStats.critChanceRangeModifierValue);
        }

        if (weaponModStats.critDamageRangeModifierValue != 0)
        {
            stat.CritDamageRange = ApplyValueModifier(stat.CritDamageRange, weaponModStats.critDamageRangeModifierValue);
        }

        if (weaponModStats.coolDownRangeModifierValue != 0)
        {
            stat.CoolDownRange = ApplyValueModifier(stat.CoolDownRange, weaponModStats.coolDownRangeModifierValue);
        }

        if (weaponModStats.staminaRangeModifierValue != 0)
        {
            stat.BaseStaminaRange = ApplyValueModifier(stat.BaseStaminaRange, weaponModStats.staminaRangeModifierValue);
        }

        // Melee Percentage modifiers
        if (weaponModStats.damageMeleeModifierPercentage != 0)
        {
            stat.MinDamageMelee = ApplyPercentageModifier(stat.MinDamageMelee, weaponModStats.damageMeleeModifierPercentage);
            stat.MaxDamageMelee = ApplyPercentageModifier(stat.MaxDamageMelee, weaponModStats.damageMeleeModifierPercentage);
        }

        if (weaponModStats.accuracyMeleeModifierPercentage != 0)
        {
            stat.AccuracyMelee = ApplyPercentageModifier(stat.AccuracyMelee, weaponModStats.accuracyMeleeModifierPercentage);
        }

        if (weaponModStats.critChanceMeleeModifierPercentage != 0)
        {
            stat.CritChanceMelee = ApplyPercentageModifier(stat.CritChanceMelee, weaponModStats.critChanceMeleeModifierPercentage);
        }

        if (weaponModStats.critDamageMeleeModifierPercentage != 0)
        {
            stat.CritDamageMelee = ApplyPercentageModifier(stat.CritDamageMelee, weaponModStats.critDamageMeleeModifierPercentage);
        }

        if (weaponModStats.coolDownMeleeModifierPercentage != 0)
        {
            stat.CoolDownMelee = ApplyPercentageModifier(stat.CoolDownMelee, weaponModStats.coolDownMeleeModifierPercentage);
        }

        if (weaponModStats.staminaMeleeModifierPercentage != 0)
        {
            stat.BaseStaminaMelee = ApplyPercentageModifier(stat.BaseStaminaMelee, weaponModStats.staminaMeleeModifierPercentage);
        }

        // Melee Value modifiers
        if (weaponModStats.damageMeleeModifierValue != 0)
        {
            stat.MinDamageMelee = ApplyValueModifier(stat.MinDamageMelee, weaponModStats.damageMeleeModifierValue);
            stat.MaxDamageMelee = ApplyValueModifier(stat.MaxDamageMelee, weaponModStats.damageMeleeModifierValue);
        }

        if (weaponModStats.accuracyMeleeModifierValue != 0)
        {
            stat.AccuracyMelee = ApplyValueModifier(stat.AccuracyMelee, weaponModStats.accuracyMeleeModifierValue);
        }

        if (weaponModStats.critChanceMeleeModifierValue != 0)
        {
            stat.CritChanceMelee = ApplyValueModifier(stat.CritChanceMelee, weaponModStats.critChanceMeleeModifierValue);
        }

        if (weaponModStats.critDamageMeleeModifierValue != 0)
        {
            stat.CritDamageMelee = ApplyValueModifier(stat.CritDamageMelee, weaponModStats.critDamageMeleeModifierValue);
        }

        if (weaponModStats.coolDownMeleeModifierValue != 0)
        {
            stat.CoolDownMelee = ApplyValueModifier(stat.CoolDownMelee, weaponModStats.coolDownMeleeModifierValue);
        }

        if (weaponModStats.staminaMeleeModifierValue != 0)
        {
            stat.BaseStaminaMelee = ApplyValueModifier(stat.BaseStaminaMelee, weaponModStats.staminaMeleeModifierValue);
        }
    }

    public void RevertMeleeAndRangeWeapon(MeleeAndRangeWeaponStats stat)
    {
        // Range Percentage modifiers
        if (weaponModStats.damageRangeModifierPercentage != 0)
        {
            stat.MinDamageRange = RevertPercentageModifier(stat.MinDamageRange, weaponModStats.damageRangeModifierPercentage);
            stat.MaxDamageRange = RevertPercentageModifier(stat.MaxDamageRange, weaponModStats.damageRangeModifierPercentage);
        }

        if (weaponModStats.accuracyRangeModifierPercentage != 0)
        {
            stat.AccuracyRange = RevertPercentageModifier(stat.AccuracyRange, weaponModStats.accuracyRangeModifierPercentage);
        }

        if (weaponModStats.critChanceRangeModifierPercentage != 0)
        {
            stat.CritChanceRange = RevertPercentageModifier(stat.CritChanceRange, weaponModStats.critChanceRangeModifierPercentage);
        }

        if (weaponModStats.critDamageRangeModifierPercentage != 0)
        {
            stat.CritDamageRange = RevertPercentageModifier(stat.CritDamageRange, weaponModStats.critDamageRangeModifierPercentage);
        }

        if (weaponModStats.coolDownRangeModifierPercentage != 0)
        {
            stat.CoolDownRange = RevertPercentageModifier(stat.CoolDownRange, weaponModStats.coolDownRangeModifierPercentage);
        }

        if (weaponModStats.staminaRangeModifierPercentage != 0)
        {
            stat.BaseStaminaRange = RevertPercentageModifier(stat.BaseStaminaRange, weaponModStats.staminaRangeModifierPercentage);
        }

        // Range Value modifiers
        if (weaponModStats.damageRangeModifierValue != 0)
        {
            stat.MinDamageRange = RevertValueModifier(stat.MinDamageRange, weaponModStats.damageRangeModifierValue);
            stat.MaxDamageRange = RevertValueModifier(stat.MaxDamageRange, weaponModStats.damageRangeModifierValue);
        }

        if (weaponModStats.accuracyRangeModifierValue != 0)
        {
            stat.AccuracyRange = RevertValueModifier(stat.AccuracyRange, weaponModStats.accuracyRangeModifierValue);
        }

        if (weaponModStats.critChanceRangeModifierValue != 0)
        {
            stat.CritChanceRange = RevertValueModifier(stat.CritChanceRange, weaponModStats.critChanceRangeModifierValue);
        }

        if (weaponModStats.critDamageRangeModifierValue != 0)
        {
            stat.CritDamageRange = RevertValueModifier(stat.CritDamageRange, weaponModStats.critDamageRangeModifierValue);
        }

        if (weaponModStats.coolDownRangeModifierValue != 0)
        {
            stat.CoolDownRange = RevertValueModifier(stat.CoolDownRange, weaponModStats.coolDownRangeModifierValue);
        }

        if (weaponModStats.staminaRangeModifierValue != 0)
        {
            stat.BaseStaminaRange = RevertValueModifier(stat.BaseStaminaRange, weaponModStats.staminaRangeModifierValue);
        }

        // Melee Percentage modifiers
        if (weaponModStats.damageMeleeModifierPercentage != 0)
        {
            stat.MinDamageMelee = RevertPercentageModifier(stat.MinDamageMelee, weaponModStats.damageMeleeModifierPercentage);
            stat.MaxDamageMelee = RevertPercentageModifier(stat.MaxDamageMelee, weaponModStats.damageMeleeModifierPercentage);
        }

        if (weaponModStats.accuracyMeleeModifierPercentage != 0)
        {
            stat.AccuracyMelee = RevertPercentageModifier(stat.AccuracyMelee, weaponModStats.accuracyMeleeModifierPercentage);
        }

        if (weaponModStats.critChanceMeleeModifierPercentage != 0)
        {
            stat.CritChanceMelee = RevertPercentageModifier(stat.CritChanceMelee, weaponModStats.critChanceMeleeModifierPercentage);
        }

        if (weaponModStats.critDamageMeleeModifierPercentage != 0)
        {
            stat.CritDamageMelee = RevertPercentageModifier(stat.CritDamageMelee, weaponModStats.critDamageMeleeModifierPercentage);
        }

        if (weaponModStats.coolDownMeleeModifierPercentage != 0)
        {
            stat.CoolDownMelee = RevertPercentageModifier(stat.CoolDownMelee, weaponModStats.coolDownMeleeModifierPercentage);
        }

        if (weaponModStats.staminaMeleeModifierPercentage != 0)
        {
            stat.BaseStaminaMelee = RevertPercentageModifier(stat.BaseStaminaMelee, weaponModStats.staminaMeleeModifierPercentage);
        }

        // Melee Value modifiers
        if (weaponModStats.damageMeleeModifierValue != 0)
        {
            stat.MinDamageMelee = RevertValueModifier(stat.MinDamageMelee, weaponModStats.damageMeleeModifierValue);
            stat.MaxDamageMelee = RevertValueModifier(stat.MaxDamageMelee, weaponModStats.damageMeleeModifierValue);
        }

        if (weaponModStats.accuracyMeleeModifierValue != 0)
        {
            stat.AccuracyMelee = RevertValueModifier(stat.AccuracyMelee, weaponModStats.accuracyMeleeModifierValue);
        }

        if (weaponModStats.critChanceMeleeModifierValue != 0)
        {
            stat.CritChanceMelee = RevertValueModifier(stat.CritChanceMelee, weaponModStats.critChanceMeleeModifierValue);
        }

        if (weaponModStats.critDamageMeleeModifierValue != 0)
        {
            stat.CritDamageMelee = RevertValueModifier(stat.CritDamageMelee, weaponModStats.critDamageMeleeModifierValue);
        }

        if (weaponModStats.coolDownMeleeModifierValue != 0)
        {
            stat.CoolDownMelee = RevertValueModifier(stat.CoolDownMelee, weaponModStats.coolDownMeleeModifierValue);
        }

        if (weaponModStats.staminaMeleeModifierValue != 0)
        {
            stat.BaseStaminaMelee = RevertValueModifier(stat.BaseStaminaMelee, weaponModStats.staminaMeleeModifierValue);
        }
    }
    #endregion

    #region Melee Weapon Modification
    public void ModifyMeleeWeapon(MeleeWeaponStats stat)
    {
        // Melee Percentage modifiers
        if (weaponModStats.damageMeleeModifierPercentage != 0)
        {
            stat.MinDamageMelee = ApplyPercentageModifier(stat.MinDamageMelee, weaponModStats.damageMeleeModifierPercentage);
            stat.MaxDamageMelee = ApplyPercentageModifier(stat.MaxDamageMelee, weaponModStats.damageMeleeModifierPercentage);
        }

        if (weaponModStats.accuracyMeleeModifierPercentage != 0)
        {
            stat.AccuracyMelee = ApplyPercentageModifier(stat.AccuracyMelee, weaponModStats.accuracyMeleeModifierPercentage);
        }

        if (weaponModStats.critChanceMeleeModifierPercentage != 0)
        {
            stat.CritChanceMelee = ApplyPercentageModifier(stat.CritChanceMelee, weaponModStats.critChanceMeleeModifierPercentage);
        }

        if (weaponModStats.critDamageMeleeModifierPercentage != 0)
        {
            stat.CritDamageMelee = ApplyPercentageModifier(stat.CritDamageMelee, weaponModStats.critDamageMeleeModifierPercentage);
        }

        if (weaponModStats.coolDownMeleeModifierPercentage != 0)
        {
            stat.CoolDownMelee = ApplyPercentageModifier(stat.CoolDownMelee, weaponModStats.coolDownMeleeModifierPercentage);
        }

        if (weaponModStats.staminaMeleeModifierPercentage != 0)
        {
            stat.BaseStaminaMelee = ApplyPercentageModifier(stat.BaseStaminaMelee, weaponModStats.staminaMeleeModifierPercentage);
        }

        // Melee Value modifiers
        if (weaponModStats.damageMeleeModifierValue != 0)
        {
            stat.MinDamageMelee = ApplyValueModifier(stat.MinDamageMelee, weaponModStats.damageMeleeModifierValue);
            stat.MaxDamageMelee = ApplyValueModifier(stat.MaxDamageMelee, weaponModStats.damageMeleeModifierValue);
        }

        if (weaponModStats.accuracyMeleeModifierValue != 0)
        {
            stat.AccuracyMelee = ApplyValueModifier(stat.AccuracyMelee, weaponModStats.accuracyMeleeModifierValue);
        }

        if (weaponModStats.critChanceMeleeModifierValue != 0)
        {
            stat.CritChanceMelee = ApplyValueModifier(stat.CritChanceMelee, weaponModStats.critChanceMeleeModifierValue);
        }

        if (weaponModStats.critDamageMeleeModifierValue != 0)
        {
            stat.CritDamageMelee = ApplyValueModifier(stat.CritDamageMelee, weaponModStats.critDamageMeleeModifierValue);
        }

        if (weaponModStats.coolDownMeleeModifierValue != 0)
        {
            stat.CoolDownMelee = ApplyValueModifier(stat.CoolDownMelee, weaponModStats.coolDownMeleeModifierValue);
        }

        if (weaponModStats.staminaMeleeModifierValue != 0)
        {
            stat.BaseStaminaMelee = ApplyValueModifier(stat.BaseStaminaMelee, weaponModStats.staminaMeleeModifierValue);
        }
    }

    public void RevertMeleeWeapon(MeleeWeaponStats stat)
    {
        // Melee Percentage modifiers
        if (weaponModStats.damageMeleeModifierPercentage != 0)
        {
            stat.MinDamageMelee = RevertPercentageModifier(stat.MinDamageMelee, weaponModStats.damageMeleeModifierPercentage);
            stat.MaxDamageMelee = RevertPercentageModifier(stat.MaxDamageMelee, weaponModStats.damageMeleeModifierPercentage);
        }

        if (weaponModStats.accuracyMeleeModifierPercentage != 0)
        {
            stat.AccuracyMelee = RevertPercentageModifier(stat.AccuracyMelee, weaponModStats.accuracyMeleeModifierPercentage);
        }

        if (weaponModStats.critChanceMeleeModifierPercentage != 0)
        {
            stat.CritChanceMelee = RevertPercentageModifier(stat.CritChanceMelee, weaponModStats.critChanceMeleeModifierPercentage);
        }

        if (weaponModStats.critDamageMeleeModifierPercentage != 0)
        {
            stat.CritDamageMelee = RevertPercentageModifier(stat.CritDamageMelee, weaponModStats.critDamageMeleeModifierPercentage);
        }

        if (weaponModStats.coolDownMeleeModifierPercentage != 0)
        {
            stat.CoolDownMelee = RevertPercentageModifier(stat.CoolDownMelee, weaponModStats.coolDownMeleeModifierPercentage);
        }

        if (weaponModStats.staminaMeleeModifierPercentage != 0)
        {
            stat.BaseStaminaMelee = RevertPercentageModifier(stat.BaseStaminaMelee, weaponModStats.staminaMeleeModifierPercentage);
        }

        // Melee Value modifiers
        if (weaponModStats.damageMeleeModifierValue != 0)
        {
            stat.MinDamageMelee = RevertValueModifier(stat.MinDamageMelee, weaponModStats.damageMeleeModifierValue);
            stat.MaxDamageMelee = RevertValueModifier(stat.MaxDamageMelee, weaponModStats.damageMeleeModifierValue);
        }

        if (weaponModStats.accuracyMeleeModifierValue != 0)
        {
            stat.AccuracyMelee = RevertValueModifier(stat.AccuracyMelee, weaponModStats.accuracyMeleeModifierValue);
        }

        if (weaponModStats.critChanceMeleeModifierValue != 0)
        {
            stat.CritChanceMelee = RevertValueModifier(stat.CritChanceMelee, weaponModStats.critChanceMeleeModifierValue);
        }

        if (weaponModStats.critDamageMeleeModifierValue != 0)
        {
            stat.CritDamageMelee = RevertValueModifier(stat.CritDamageMelee, weaponModStats.critDamageMeleeModifierValue);
        }

        if (weaponModStats.coolDownMeleeModifierValue != 0)
        {
            stat.CoolDownMelee = RevertValueModifier(stat.CoolDownMelee, weaponModStats.coolDownMeleeModifierValue);
        }

        if (weaponModStats.staminaMeleeModifierValue != 0)
        {
            stat.BaseStaminaMelee = RevertValueModifier(stat.BaseStaminaMelee, weaponModStats.staminaMeleeModifierValue);
        }
    }
    #endregion

    #region Range Weapon Modification
    public void ModifyRangeWeapon(RangeWeaponStats stat)
    {
        // Range Percentage modifiers
        if (weaponModStats.damageRangeModifierPercentage != 0)
        {
            stat.MinDamageRange = ApplyPercentageModifier(stat.MinDamageRange, weaponModStats.damageRangeModifierPercentage);
            stat.MaxDamageRange = ApplyPercentageModifier(stat.MaxDamageRange, weaponModStats.damageRangeModifierPercentage);
        }

        if (weaponModStats.accuracyRangeModifierPercentage != 0)
        {
            stat.AccuracyRange = ApplyPercentageModifier(stat.AccuracyRange, weaponModStats.accuracyRangeModifierPercentage);
        }

        if (weaponModStats.critChanceRangeModifierPercentage != 0)
        {
            stat.CritChanceRange = ApplyPercentageModifier(stat.CritChanceRange, weaponModStats.critChanceRangeModifierPercentage);
        }

        if (weaponModStats.critDamageRangeModifierPercentage != 0)
        {
            stat.CritDamageRange = ApplyPercentageModifier(stat.CritDamageRange, weaponModStats.critDamageRangeModifierPercentage);
        }

        if (weaponModStats.coolDownRangeModifierPercentage != 0)
        {
            stat.CoolDownRange = ApplyPercentageModifier(stat.CoolDownRange, weaponModStats.coolDownRangeModifierPercentage);
        }

        if (weaponModStats.staminaRangeModifierPercentage != 0)
        {
            stat.BaseStaminaRange = ApplyPercentageModifier(stat.BaseStaminaRange, weaponModStats.staminaRangeModifierPercentage);
        }

        // Range Value modifiers
        if (weaponModStats.damageRangeModifierValue != 0)
        {
            stat.MinDamageRange = ApplyValueModifier(stat.MinDamageRange, weaponModStats.damageRangeModifierValue);
            stat.MaxDamageRange = ApplyValueModifier(stat.MaxDamageRange, weaponModStats.damageRangeModifierValue);
        }

        if (weaponModStats.accuracyRangeModifierValue != 0)
        {
            stat.AccuracyRange = ApplyValueModifier(stat.AccuracyRange, weaponModStats.accuracyRangeModifierValue);
        }

        if (weaponModStats.critChanceRangeModifierValue != 0)
        {
            stat.CritChanceRange = ApplyValueModifier(stat.CritChanceRange, weaponModStats.critChanceRangeModifierValue);
        }

        if (weaponModStats.critDamageRangeModifierValue != 0)
        {
            stat.CritDamageRange = ApplyValueModifier(stat.CritDamageRange, weaponModStats.critDamageRangeModifierValue);
        }

        if (weaponModStats.coolDownRangeModifierValue != 0)
        {
            stat.CoolDownRange = ApplyValueModifier(stat.CoolDownRange, weaponModStats.coolDownRangeModifierValue);
        }

        if (weaponModStats.staminaRangeModifierValue != 0)
        {
            stat.BaseStaminaRange = ApplyValueModifier(stat.BaseStaminaRange, weaponModStats.staminaRangeModifierValue);
        }
    }

    public void RevertRangeWeapon(RangeWeaponStats stat)
    {
        // Range Percentage modifiers
        if (weaponModStats.damageRangeModifierPercentage != 0)
        {
            stat.MinDamageRange = RevertPercentageModifier(stat.MinDamageRange, weaponModStats.damageRangeModifierPercentage);
            stat.MaxDamageRange = RevertPercentageModifier(stat.MaxDamageRange, weaponModStats.damageRangeModifierPercentage);
        }

        if (weaponModStats.accuracyRangeModifierPercentage != 0)
        {
            stat.AccuracyRange = RevertPercentageModifier(stat.AccuracyRange, weaponModStats.accuracyRangeModifierPercentage);
        }

        if (weaponModStats.critChanceRangeModifierPercentage != 0)
        {
            stat.CritChanceRange = RevertPercentageModifier(stat.CritChanceRange, weaponModStats.critChanceRangeModifierPercentage);
        }

        if (weaponModStats.critDamageRangeModifierPercentage != 0)
        {
            stat.CritDamageRange = RevertPercentageModifier(stat.CritDamageRange, weaponModStats.critDamageRangeModifierPercentage);
        }

        if (weaponModStats.coolDownRangeModifierPercentage != 0)
        {
            stat.CoolDownRange = RevertPercentageModifier(stat.CoolDownRange, weaponModStats.coolDownRangeModifierPercentage);
        }

        if (weaponModStats.staminaRangeModifierPercentage != 0)
        {
            stat.BaseStaminaRange = RevertPercentageModifier(stat.BaseStaminaRange, weaponModStats.staminaRangeModifierPercentage);
        }

        // Range Value modifiers
        if (weaponModStats.damageRangeModifierValue != 0)
        {
            stat.MinDamageRange = RevertValueModifier(stat.MinDamageRange, weaponModStats.damageRangeModifierValue);
            stat.MaxDamageRange = RevertValueModifier(stat.MaxDamageRange, weaponModStats.damageRangeModifierValue);
        }

        if (weaponModStats.accuracyRangeModifierValue != 0)
        {
            stat.AccuracyRange = RevertValueModifier(stat.AccuracyRange, weaponModStats.accuracyRangeModifierValue);
        }

        if (weaponModStats.critChanceRangeModifierValue != 0)
        {
            stat.CritChanceRange = RevertValueModifier(stat.CritChanceRange, weaponModStats.critChanceRangeModifierValue);
        }

        if (weaponModStats.critDamageRangeModifierValue != 0)
        {
            stat.CritDamageRange = RevertValueModifier(stat.CritDamageRange, weaponModStats.critDamageRangeModifierValue);
        }

        if (weaponModStats.coolDownRangeModifierValue != 0)
        {
            stat.CoolDownRange = RevertValueModifier(stat.CoolDownRange, weaponModStats.coolDownRangeModifierValue);
        }

        if (weaponModStats.staminaRangeModifierValue != 0)
        {
            stat.BaseStaminaRange = RevertValueModifier(stat.BaseStaminaRange, weaponModStats.staminaRangeModifierValue);
        }
    }
    #endregion
}