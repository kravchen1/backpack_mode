using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeWeaponAction : WeaponActionController
{
    public string animationKeyAttackMelee = "AttackMelee";
    public string animationKeyAttackNoStamina = "AttackNoStamina";


    private MeleeWeaponStats meleeWeaponStats;

    protected override void Awake()
    {
        base.Awake();


        //StartCoroutine(Initialize());
        
    }

    //private IEnumerator Initialize()
    //{
    //    yield return null;
    //    yield return null;
    //    yield return null;
    //    yield return null;

    //    InitializeBase();
    //}




    private void InitializeBase()
    {
        cooldownTime    = meleeWeaponStats.CoolDownMelee;
        staminaCost     = meleeWeaponStats.BaseStaminaMelee;
        damageMin       = meleeWeaponStats.MinDamageMelee;
        damageMax       = meleeWeaponStats.MaxDamageMelee;
        critDamageMelee = meleeWeaponStats.CritDamageMelee;
        baseAccuracy    = meleeWeaponStats.AccuracyMelee;
        baseCritChance  = meleeWeaponStats.CritChanceMelee;

        currentCooldown = cooldownTime;
        StartCooldown();
    }

    protected override void Attack()
    {
        
    }
    
}