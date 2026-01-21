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
        if (isFight)
        {
            base.Awake();

            StartCoroutine(Initialize());
        }
    }

    private IEnumerator Initialize()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        InitializeBase();
    }




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

    protected override void Attack(NPCDataManager attacker, NPCDataManager target)
    {
        ResetTextDamage();
        CalculateAnimationTime();
        if (!HasStamina(attacker))
        {
            text.text = "No Stamina";
            animator.Play(animationKeyAttackNoStamina, 0, 0f);
            return;
        }
        itemStats.durability--;
        ConsumeStamina(attacker);

        if (CalculateAccuracy())
        {
            int damage = 0;
            damage = CalculateDamageMelee(attacker);


            bool isCritical = CalculateCriticalHit();
            if (isCritical)
            {
                damage = (int)(damage * critDamageMelee / 100f);
                Crit();
            }
            text.text = damage.ToString();
            StartCoroutine(Attack(damage, timeAnimation, target));
        }
        else
        {
            Miss();
        }

        animator.Play(animationKeyAttackMelee, 0, 0f);
    }
    protected override void Attack(PlayerDataManager attacker, NPCDataManager target)
    {
        ResetTextDamage();
        CalculateAnimationTime();
        if (!HasStamina(attacker))
        {
            text.text = "No Stamina";
            animator.Play(animationKeyAttackNoStamina, 0, 0f);
            return;
        }
        itemStats.durability--;
        ConsumeStamina(attacker);

        if (CalculateAccuracy())
        {
            int damage = 0;
            damage = CalculateDamageMelee(attacker);


            bool isCritical = CalculateCriticalHit();
            if (isCritical)
            {
                damage = (int)(damage * critDamageMelee / 100f);
                Crit();
            }
            text.text = damage.ToString();
            StartCoroutine(Attack(damage, timeAnimation, target));
        }
        else
        {
            Miss();
        }

        animator.Play(animationKeyAttackMelee, 0, 0f);
    }
    protected override void Attack(NPCDataManager attacker, PlayerDataManager target)
    {
        ResetTextDamage();
        CalculateAnimationTime();
        if (!HasStamina(attacker))
        {
            text.text = "No Stamina";
            animator.Play(animationKeyAttackNoStamina, 0, 0f);
            return;
        }
        itemStats.durability--;
        ConsumeStamina(attacker);

        if (CalculateAccuracy())
        {
            int damage = 0;
            damage = CalculateDamageMelee(attacker);


            bool isCritical = CalculateCriticalHit();
            if (isCritical)
            {
                damage = (int)(damage * critDamageMelee / 100f);
                Crit();
            }
            text.text = damage.ToString();
            StartCoroutine(Attack(damage, timeAnimation, target));
        }
        else
        {
            Miss();
        }

        animator.Play(animationKeyAttackMelee, 0, 0f);
    }

    private void CalculateAnimationTime()
    {
        if (cooldownTime < timeAnimation)
        {
            animator.speed = timeAnimation / cooldownTime;
            timeAnimation = cooldownTime;
        }
        else
        {
            timeAnimation = 1.5f;
            animator.speed = timeAnimation;
        }
    }
}