using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RangeWeaponAction : WeaponActionController
{


    private RangeWeaponStats rangeWeaponStats;
    private List<ItemStar> itemStarPatrons = new List<ItemStar>();
    private ItemMove currentPatron;

    protected override void Awake()
    {
        base.Awake();
    }

    //private IEnumerator Initialize()
    //{
    //    yield return null;
    //    yield return null;
    //    yield return null;
    //    yield return null;
    //    //проставляем звёзды в предметах
    //    itemStarPatrons = GetComponentsInChildren<ItemStar>().ToList().Where(e => HasMatchingItemType(e.AllowedItemTypes)).ToList();
    //    rangeWeaponStats = GetComponent<RangeWeaponStats>();
    //    InitializeBase();
    //}

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
        patrons.Add(ItemType.Patron556x45);

        return patrons;
    }
    private void InitializeBase()
    {

        cooldownTime = rangeWeaponStats.CoolDownRange;
        staminaCost = rangeWeaponStats.BaseStaminaRange;
        damageMin = rangeWeaponStats.MinDamageRange;
        damageMax = rangeWeaponStats.MaxDamageRange;
        critDamageMelee = rangeWeaponStats.CritDamageRange;
        baseAccuracy = rangeWeaponStats.AccuracyRange;
        baseCritChance = rangeWeaponStats.CritChanceRange;

        currentCooldown = cooldownTime;
        StartCooldown();
    }
    private void SpendPatron()
    {
        currentPatron.StackCount--;
        if(currentPatron.StackCount==0)
        {
            Destroy(currentPatron.gameObject);
        }
        InitializeBase();
    }


    protected override void Attack()
    {
       
      
    }


}