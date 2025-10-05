using System.Linq;
using UnityEngine;

public abstract class WeaponActionController : ActivationItemActionController
{
    [HideInInspector] protected float staminaCost = 1f;
    [HideInInspector] protected int damageMin = 1;
    [HideInInspector] protected int damageMax = 1;
    [HideInInspector] protected int critDamageMelee = 180;
    [HideInInspector] protected int baseAccuracy = 10;
    [HideInInspector] protected int baseCritChance = 10;

    protected override void ExecuteAction(NPCDataManager attacker, NPCDataManager target)
    {
        Attack(attacker, target);
        StartCooldown();
    }
    protected override void ExecuteAction(PlayerDataManager attacker, NPCDataManager target)
    {
        Attack(attacker, target);
        StartCooldown();
    }
    protected override void ExecuteAction(NPCDataManager attacker, PlayerDataManager target)
    {
        Attack(attacker, target);
        StartCooldown();
    }


    protected virtual void Attack(NPCDataManager attacker, NPCDataManager target)
    {
        if(!HasStamina(attacker))
        {
            return;
        }

        ConsumeStamina(attacker);
        if (CalculateAccuracy())
        {
            int damage = 0;//тут что угодноб может быть CalculateDamageMelee или CalculateDamageRange или ещё что в будущем
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
    protected virtual void Attack(PlayerDataManager attacker, NPCDataManager target)
    {
        if (!HasStamina(attacker))
        {
            return;
        }

        ConsumeStamina(attacker);
        if (CalculateAccuracy())
        {
            int damage = 0;//тут что угодноб может быть CalculateDamageMelee или CalculateDamageRange или ещё что в будущем
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
    protected virtual void Attack(NPCDataManager attacker, PlayerDataManager target)
    {
        if (!HasStamina(attacker))
        {
            return;
        }

        ConsumeStamina(attacker);
        if (CalculateAccuracy())
        {
            int damage = 0;//тут что угодноб может быть CalculateDamageMelee или CalculateDamageRange или ещё что в будущем
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


    protected int CalculateDamageMelee(NPCDataManager attacker)
    {
        int damageResult = Random.Range(damageMin, damageMax);
        damageResult = (int)(damageResult * (1 + attacker.Attributes.Strength * 0.05f));
        return damageResult;
    }
    protected int CalculateDamageRange(NPCDataManager attacker)
    {
        int damageResult = Random.Range(damageMin, damageMax);
        damageResult = (int)(damageResult * (1 + attacker.Attributes.Agility * 0.05f));
        return damageResult;
    }
    protected int CalculateDamagePSI(NPCDataManager attacker)
    {
        int damageResult = Random.Range(damageMin, damageMax);
        damageResult = (int)(damageResult * (1 + attacker.Attributes.Intellect * 0.05f));
        return damageResult;
    }

    protected int CalculateDamageMelee(PlayerDataManager attacker)
    {
        int damageResult = Random.Range(damageMin, damageMax);
        damageResult = (int)(damageResult * (1 + attacker.Attributes.Strength * 0.05f));
        return damageResult;
    }
    protected int CalculateDamageRange(PlayerDataManager attacker)
    {
        int damageResult = Random.Range(damageMin, damageMax);
        damageResult = (int)(damageResult * (1 + attacker.Attributes.Agility * 0.05f));
        return damageResult;
    }
    protected int CalculateDamagePSI(PlayerDataManager attacker)
    {
        int damageResult = Random.Range(damageMin, damageMax);
        damageResult = (int)(damageResult * (1 + attacker.Attributes.Intellect * 0.05f));
        return damageResult;
    }


    protected void Miss()
    {
        Debug.Log(gameObject.name + " промахнулся");
    }
    protected void Crit()
    {
        Debug.Log(gameObject.name + " кританул");
    }



    protected bool CalculateAccuracy()
    {
        return Random.Range(0, 100) < baseAccuracy;
    }
    protected bool CalculateCriticalHit()
    {
        return Random.Range(0, 100) < baseCritChance;
    }



    protected virtual bool HasStamina(NPCDataManager target)
    {
        float currentStamina = target.Stats.CurrentStamina;

        return currentStamina >= staminaCost;
    }
    protected virtual bool HasStamina(PlayerDataManager target)
    {
        float currentStamina = target.Stats.CurrentStamina;

        return currentStamina >= staminaCost;
    }
    protected virtual void ConsumeStamina(NPCDataManager target)
    {
        target.Stats.CurrentStamina -= staminaCost;
    }
    protected virtual void ConsumeStamina(PlayerDataManager target)
    {
        target.Stats.CurrentStamina -= staminaCost;
    }
}