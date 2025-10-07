using UnityEngine;
using System.Collections;

public abstract class ItemActionController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] protected Animator animator;
    protected ItemStats itemStats;
    protected bool isFight => BattleManager.Instance.isBattleActive;

    protected virtual void Awake()
    {
        if (isFight)
        {
            itemStats = GetComponent<ItemStats>();

            if (animator == null)
                animator = GetComponent<Animator>();
        }
    }

    public virtual void UpdateForBattle(NPCDataManager attacker, NPCDataManager target)
    {
        if (isFight)
        {
            UpdateActivation(attacker, target);
        }
    }
    public virtual void UpdateForBattle(PlayerDataManager attacker, NPCDataManager target)
    {
        if (isFight) UpdateActivation(attacker, target);
    }
    public virtual void UpdateForBattle(NPCDataManager attacker, PlayerDataManager target)
    {
        if (isFight) UpdateActivation(attacker, target);
    }




    protected virtual void UpdateActivation(NPCDataManager attacker, NPCDataManager target)
    {
        // Override in derived classes for activation logic
        if (isFight) ExecuteAction(attacker, target);
    }
    protected virtual void UpdateActivation(PlayerDataManager attacker, NPCDataManager target)
    {
        // Override in derived classes for activation logic
        if (isFight) ExecuteAction(attacker, target);
    }
    protected virtual void UpdateActivation(NPCDataManager attacker, PlayerDataManager target)
    {
        // Override in derived classes for activation logic
        if (isFight) ExecuteAction(attacker, target);
    }




    protected abstract void ExecuteAction(NPCDataManager attacker, NPCDataManager target);
    protected abstract void ExecuteAction(PlayerDataManager attacker, NPCDataManager target);
    protected abstract void ExecuteAction(NPCDataManager attacker, PlayerDataManager target);
}