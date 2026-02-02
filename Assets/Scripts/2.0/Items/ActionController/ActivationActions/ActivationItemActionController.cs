using UnityEngine;

public abstract class ActivationItemActionController : ItemActionController
{
    [HideInInspector] protected float cooldownTime = 1f;
    [HideInInspector] protected float currentCooldown = 1f;


    [HideInInspector] protected bool isOnCooldown = true;

    // Properties
    [HideInInspector] public bool IsReady => !isOnCooldown;


    protected override void Awake()
    {
        base.Awake();
    }

    public virtual void UpdateForBattle()
    {
        if (itemStats.durability > 0)
        {
            UpdateCooldown();
            UpdateActivation();
        }
    }


    protected virtual void UpdateCooldown()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f)
        {
            isOnCooldown = false;
        }
    }

    protected virtual void UpdateActivation()
    {
        if (IsReady)
        {
            ExecuteAction();
        }
    }



    protected virtual void ExecuteAction()
    {
       StartCooldown();
    }



    protected virtual void StartCooldown()
    {
        isOnCooldown = true;
        currentCooldown = cooldownTime;
    }
}