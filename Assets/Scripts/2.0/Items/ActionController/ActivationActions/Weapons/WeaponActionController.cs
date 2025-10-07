using System.Linq;
using TMPro;
using UnityEngine;

public abstract class WeaponActionController : ActivationItemActionController
{
    [HideInInspector] protected float staminaCost = 1f;
    [HideInInspector] protected int damageMin = 1;
    [HideInInspector] protected int damageMax = 1;
    [HideInInspector] protected int critDamageMelee = 180;
    [HideInInspector] protected int baseAccuracy = 10;
    [HideInInspector] protected int baseCritChance = 10;

    public TextMeshPro text;
    public float textSize = 500f;
    public Color textColor = Color.black;

    protected float timeAnimation = 1.5f;

    protected override void Awake()
    {
        if (isFight)
        {
            base.Awake();

            isOnCooldown = true;
        }
    }
    protected override void ExecuteAction(NPCDataManager attacker, NPCDataManager target)
    {
        if (isFight)
        {
            Attack(attacker, target);
            StartCooldown();
        }
    }
    protected override void ExecuteAction(PlayerDataManager attacker, NPCDataManager target)
    {
        if (isFight)
        {
            Attack(attacker, target);
            StartCooldown();
        }
    }
    protected override void ExecuteAction(NPCDataManager attacker, PlayerDataManager target)
    {
        if (isFight)
        {
            Attack(attacker, target);
            StartCooldown();
        }
    }


    protected virtual void Attack(NPCDataManager attacker, NPCDataManager target)
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
        text.text = "miss";
    }
    protected void Crit()
    {
        text.fontSize = textSize * 1.2f;
        text.color = Color.red;
    }

    protected void ResetTextDamage()
    {
        text.fontSize = textSize;
        text.color = textColor;
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
        StartCoroutine(ConsumeStamia(staminaCost, 0.1f, target));
    }
    protected virtual void ConsumeStamina(PlayerDataManager target)
    {
        StartCoroutine(ConsumeStamia(staminaCost, 0.1f, target));
    }

    protected System.Collections.IEnumerator ConsumeStamia(float staminaCost, float delay, NPCDataManager target)
    {
        yield return new WaitForSeconds(delay);

        float finalStaminaCost = staminaCost;
        var loadCategory = target.Stats.GetCurrentLoadCategory();
        switch (loadCategory)
        {
            case LoadCategory.Medium:
                finalStaminaCost *= 1.25f;
                break;
            case LoadCategory.Heavy:
                finalStaminaCost *= 1.5f;
                break;
            case LoadCategory.Overloaded:
                finalStaminaCost *= 2f;
                break;
            default:
                break;
        }

        target.Stats.CurrentStamina -= finalStaminaCost;
        yield break;
    }
    protected System.Collections.IEnumerator ConsumeStamia(float staminaCost, float delay, PlayerDataManager target)
    {
        yield return new WaitForSeconds(delay);

        float finalStaminaCost = staminaCost;
        var loadCategory = target.Stats.GetCurrentLoadCategory();
        switch (loadCategory)
        {
            case LoadCategory.Medium:
                finalStaminaCost *= 1.25f;
                break;
            case LoadCategory.Heavy:
                finalStaminaCost *= 1.5f;
                break;
            case LoadCategory.Overloaded:
                finalStaminaCost *= 2f;
                break;
            default:
                break;
        }

        target.Stats.CurrentStamina -= finalStaminaCost;
        yield break;
    }


    protected System.Collections.IEnumerator Attack(int damage, float delay, NPCDataManager target)
    {
        yield return new WaitForSeconds(delay);
        target.TakeDamage(damage);
        yield break;
    }
    protected System.Collections.IEnumerator Attack(int damage, float delay, PlayerDataManager target)
    {
        yield return new WaitForSeconds(delay);
        target.TakeDamage(damage);
        yield break;
    }
}