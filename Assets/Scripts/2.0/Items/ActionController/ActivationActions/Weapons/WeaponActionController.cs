using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class WeaponActionController : ActivationItemActionController
{
    [HideInInspector] protected float staminaCost = 1f;
    [HideInInspector] protected int damageMin = 1;
    [HideInInspector] protected int damageMax = 1;
    [HideInInspector] protected int critDamageMelee = 180;
    [HideInInspector] protected int baseAccuracy = 10;
    [HideInInspector] protected int baseCritChance = 10;

    protected override void Awake()
    {
        base.Awake();

        isOnCooldown = true;
    }
    protected override void ExecuteAction()
    {
        Attack();
        StartCooldown();
    }


    protected virtual void Attack()
    {
        
    }



   



    protected virtual bool HasStamina(PlayerDataManager target)
    {
        float currentStamina = target.Stats.CurrentStamina;

        return currentStamina >= staminaCost;
    }
    
    protected virtual void ConsumeStamina(PlayerDataManager target)
    {
        target.Stats.CurrentStamina -= staminaCost;
    }

}