using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeAndWeaponActionController : WeaponActionController
{
    private MeleeAndRangeWeaponStats meleeAndRangeWeaponStats;
    private bool isInMeleeMode = true;
    private List<ItemStar> itemStarPatrons = new List<ItemStar>();
    private ItemMove currentPatron;

    protected override void Awake()
    {
        base.Awake();

        
        Initialize();
    }
    
    private void Initialize()
    {
        itemStarPatrons = GetComponentsInChildren<ItemStar>().ToList().Where(e => HasMatchingItemType(e.AllowedItemTypes)).ToList();
        meleeAndRangeWeaponStats = GetComponent<MeleeAndRangeWeaponStats>();
        SwitchMode();
        InitializeBase();
    }
    private void SwitchMode()
    {
        if (CheckPatron())
        {
            isInMeleeMode = false;
        }
        else
        {
            isInMeleeMode = true;
        }
    }
    private bool CheckPatron()
    {
        foreach(var item in itemStarPatrons)
        {
            if(item.CurrentItem != null && item.CurrentItem.GetComponent<ItemMove>() != null && item.CurrentItem.GetComponent<ItemMove>().StackCount>=1)
            {
                currentPatron = item.CurrentItem.GetComponent<ItemMove>();
                return true;
            }
        }
        currentPatron = null;
        return false;
    }
    private bool HasMatchingItemType(List<ItemType> itemTypesToCheck)
    {
        List<ItemType> patrons = new List<ItemType>();
        patrons.AddRange(getPatronsType());

        foreach (var itemType in itemTypesToCheck)
        {
            if (patrons.Contains(itemType))
                return true;
        }
        return false;
    }
    protected virtual List<ItemType> getPatronsType()
    {
        List<ItemType> patrons = new List<ItemType>();
        patrons.Add(ItemType.Patron5x45);

        return patrons;
    }
    private void InitializeBase()
    {
        if (isInMeleeMode)
        {
            cooldownTime    = meleeAndRangeWeaponStats.CoolDownMelee;
            staminaCost     = meleeAndRangeWeaponStats.BaseStaminaMelee;
            damageMin       = meleeAndRangeWeaponStats.MinDamageMelee;
            damageMax       = meleeAndRangeWeaponStats.MaxDamageMelee;
            critDamageMelee = meleeAndRangeWeaponStats.CritDamageMelee;
            baseAccuracy    = meleeAndRangeWeaponStats.AccuracyMelee;
            baseCritChance  = meleeAndRangeWeaponStats.CritChanceMelee;
        }
        else
        {
            cooldownTime = meleeAndRangeWeaponStats.CoolDownRange;
            staminaCost = meleeAndRangeWeaponStats.BaseStaminaRange;
            damageMin = meleeAndRangeWeaponStats.MinDamageRange;
            damageMax = meleeAndRangeWeaponStats.MaxDamageRange;
            critDamageMelee = meleeAndRangeWeaponStats.CritDamageRange;
            baseAccuracy = meleeAndRangeWeaponStats.AccuracyRange;
            baseCritChance = meleeAndRangeWeaponStats.CritChanceRange;
        }
    }
    private void SpendPatron()
    {
        currentPatron.StackCount--;
        SwitchMode();
        InitializeBase();
    }

    protected override void Attack(NPCDataManager attacker, NPCDataManager target)
    {
        if (!HasStamina(attacker))
        {
            return;
        }

        ConsumeStamina(attacker);
        if (!isInMeleeMode)
        {
            Debug.Log(gameObject.name + "стреляет");
            SpendPatron();
        }
        else
        {
            Debug.Log(gameObject.name + "бьёт в рукопашную");
        }
        if (CalculateAccuracy())
        {
            int damage = 0;
            if (isInMeleeMode)
            {
                damage = CalculateDamageMelee(attacker);
            }
            else
            {
                damage = CalculateDamageRange(attacker);
            }

            bool isCritical = CalculateCriticalHit();
            if (isCritical)
            {
                damage = (int)(damage * critDamageMelee / 100f);
                Crit();
            }

            target.TakeDamage(damage);
        }
        else
        {
            Miss();
        }
    }
    protected override void Attack(PlayerDataManager attacker, NPCDataManager target)
    {
        if (!HasStamina(attacker))
        {
            return;
        }

        ConsumeStamina(attacker);
        if (!isInMeleeMode)
        {
            SpendPatron();
        }
        if (CalculateAccuracy())
        {
            int damage = 0;
            if (isInMeleeMode)
            {
                damage = CalculateDamageMelee(attacker);
            }
            else
            {
                damage = CalculateDamageRange(attacker);
            }

            bool isCritical = CalculateCriticalHit();
            if (isCritical)
            {
                damage = (int)(damage * critDamageMelee / 100f);
                Crit();
            }

            target.TakeDamage(damage);
        }
        else
        {
            Miss();
        }
    }
    protected override void Attack(NPCDataManager attacker, PlayerDataManager target)
    {
        if (!HasStamina(attacker))
        {
            return;
        }

        ConsumeStamina(attacker);
        if (!isInMeleeMode)
        {
            SpendPatron();
        }
        if (CalculateAccuracy())
        {
            int damage = 0;
            if (isInMeleeMode)
            {
                damage = CalculateDamageMelee(attacker);
            }
            else
            {
                damage = CalculateDamageRange(attacker);
            }

            bool isCritical = CalculateCriticalHit();
            if (isCritical)
            {
                damage = (int)(damage * critDamageMelee / 100f);
                Crit();
            }

            target.TakeDamage(damage);
        }
        else
        {
            Miss();
        }
    }


}