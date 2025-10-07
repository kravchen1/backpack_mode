using UnityEngine;

public abstract class ActivationItemActionController : ItemActionController
{
    [HideInInspector] protected float cooldownTime = 1f;
    [HideInInspector] protected float currentCooldown = 1f;


    [HideInInspector] protected bool isOnCooldown = true;

    // Properties
    [HideInInspector] public bool IsReady => !isOnCooldown;
    [HideInInspector] public float CooldownProgress => Mathf.Clamp01(currentCooldown / cooldownTime);

    private Transform reloadSprite;

    protected override void Awake()
    {
        if (isFight)
        {
            base.Awake();
            reloadSprite = transform.GetChild(1);
        }
    }

    public override void UpdateForBattle(NPCDataManager attacker, NPCDataManager target)
    {
        if (isFight)
        {
            UpdateCooldown();
            UpdateActivation(attacker, target);
        }
    }

    public override void UpdateForBattle(PlayerDataManager attacker, NPCDataManager target)
    {
        if (isFight)
        {
            UpdateCooldown();
            UpdateActivation(attacker, target);
        }
    }

    public override void UpdateForBattle(NPCDataManager attacker, PlayerDataManager target)
    {
        if (isFight)
        {
            UpdateCooldown();
            UpdateActivation(attacker, target);
        }
    }

    protected virtual void UpdateCooldown()
    {

        if (isOnCooldown && isFight)
        {
            currentCooldown -= Time.deltaTime;
            if (reloadSprite != null)
            {
                Vector3 newScale = reloadSprite.localScale;
                newScale.x = CooldownProgress; // Или newScale.y, если шкала вертикальная
                reloadSprite.localScale = newScale;
            }
            if (currentCooldown <= 0f)
            {
                isOnCooldown = false;
            }
        }
    }

    protected override void UpdateActivation(NPCDataManager attacker, NPCDataManager target)
    {
        if (IsReady && isFight)
        {
            ExecuteAction(attacker, target);
        }
    }
    protected override void UpdateActivation(PlayerDataManager attacker, NPCDataManager target)
    {
        if (IsReady && isFight)
        {
            ExecuteAction(attacker, target);
        }
    }
    protected override void UpdateActivation(NPCDataManager attacker, PlayerDataManager target)
    {
        if (IsReady && isFight)
        {
            ExecuteAction(attacker, target);
        }
    }



    protected override void ExecuteAction(NPCDataManager attacker, NPCDataManager target)
    {
        //Debug.Log(gameObject.name + " activated");
        if (isFight) StartCooldown();
    }

    protected override void ExecuteAction(PlayerDataManager attacker, NPCDataManager target)
    {
        //Debug.Log(gameObject.name + " activated");
        if (isFight) StartCooldown();
    }

    protected override void ExecuteAction(NPCDataManager attacker, PlayerDataManager target)
    {
        //Debug.Log(gameObject.name + " activated");
        if (isFight) StartCooldown();
    }



    protected virtual void StartCooldown()
    {
        isOnCooldown = true;
        currentCooldown = cooldownTime;
    }
}