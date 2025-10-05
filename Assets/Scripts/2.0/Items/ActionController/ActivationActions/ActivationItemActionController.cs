using UnityEngine;

public abstract class ActivationItemActionController : ItemActionController
{
    [HideInInspector] protected float cooldownTime = 1f;
    [HideInInspector] protected float currentCooldown;


    [HideInInspector] protected bool isOnCooldown;

    // Properties
    [HideInInspector] public bool IsReady => !isOnCooldown;
    [HideInInspector] public float CooldownProgress => Mathf.Clamp01(currentCooldown / cooldownTime);

    public override void UpdateForBattle(NPCDataManager attacker, NPCDataManager target)
    {
        UpdateCooldown();
        UpdateActivation(attacker, target);
    }

    public override void UpdateForBattle(PlayerDataManager attacker, NPCDataManager target)
    {
        UpdateCooldown();
        UpdateActivation(attacker, target);
    }

    public override void UpdateForBattle(NPCDataManager attacker, PlayerDataManager target)
    {
        UpdateCooldown();
        UpdateActivation(attacker, target);
    }

    protected virtual void UpdateCooldown()
    {
        if (isOnCooldown)
        {
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0f)
            {
                isOnCooldown = false;
            }
        }
    }

    protected override void UpdateActivation(NPCDataManager attacker, NPCDataManager target)
    {
        if (IsReady)
        {
            ExecuteAction(attacker, target);
        }
    }
    protected override void UpdateActivation(PlayerDataManager attacker, NPCDataManager target)
    {
        if (IsReady)
        {
            ExecuteAction(attacker, target);
        }
    }
    protected override void UpdateActivation(NPCDataManager attacker, PlayerDataManager target)
    {
        if (IsReady)
        {
            ExecuteAction(attacker, target);
        }
    }



    protected override void ExecuteAction(NPCDataManager attacker, NPCDataManager target)
    {
        Debug.Log(gameObject.name + " activated");
        StartCooldown();
    }

    protected override void ExecuteAction(PlayerDataManager attacker, NPCDataManager target)
    {
        Debug.Log(gameObject.name + " activated");
        StartCooldown();
    }

    protected override void ExecuteAction(NPCDataManager attacker, PlayerDataManager target)
    {
        Debug.Log(gameObject.name + " activated");
        StartCooldown();
    }



    protected virtual void StartCooldown()
    {
        isOnCooldown = true;
        currentCooldown = cooldownTime;
    }
}