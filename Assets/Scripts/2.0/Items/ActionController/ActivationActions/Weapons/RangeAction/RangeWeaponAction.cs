using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RangeWeaponAction : WeaponActionController
{
    public string animationKeyAttackRange = "AttackRange";
    public string animationKeyNoPatron = "AttackNoPatron";
    public string animationKeyAttackNoStamina = "AttackNoStamina";


    private RangeWeaponStats rangeWeaponStats;
    private bool isInMeleeMode = true;
    private List<ItemStar> itemStarPatrons = new List<ItemStar>();
    private ItemMove currentPatron;

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
        //проставляем звёзды в предметах
        itemStarPatrons = GetComponentsInChildren<ItemStar>().ToList().Where(e => HasMatchingItemType(e.AllowedItemTypes)).ToList();
        rangeWeaponStats = GetComponent<RangeWeaponStats>();
        InitializeBase();
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
        if (CheckPatron())
        {
            Debug.Log(gameObject.name + "стреляет");
            SpendPatron();

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
                text.text = damage.ToString();
                StartCoroutine(Attack(damage, timeAnimation, target));
            }
            else
            {
                Miss();
            }
            animator.Play(animationKeyAttackRange, 0, 0f);
        }
        else
        {
            animator.Play(animationKeyNoPatron, 0, 0f);
        }
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
        if (CheckPatron())
        {
            Debug.Log(gameObject.name + "стреляет");
            SpendPatron();

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
                text.text = damage.ToString();
                StartCoroutine(Attack(damage, timeAnimation, target));
            }
            else
            {
                Miss();
            }
            animator.Play(animationKeyAttackRange, 0, 0f);
        }
        else
        {
            animator.Play(animationKeyNoPatron, 0, 0f);
        }
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
        if (CheckPatron())
        {
            Debug.Log(gameObject.name + "стреляет");
            SpendPatron();

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
                text.text = damage.ToString();
                StartCoroutine(Attack(damage, timeAnimation, target));
            }
            else
            {
                Miss();
            }
            animator.Play(animationKeyAttackRange, 0, 0f);
        }
        else
        {
            animator.Play(animationKeyNoPatron, 0, 0f);
        }
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