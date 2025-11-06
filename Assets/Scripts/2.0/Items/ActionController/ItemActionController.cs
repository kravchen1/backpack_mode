using UnityEngine;
using System.Collections;

public abstract class ItemActionController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] protected Animator animator;
    protected ItemStats itemStats;
    protected bool isFight
    {
        get
        {
            if (BattleManager.Instance != null)
                return BattleManager.Instance.isBattleActive;
            return false;
        }
    }

    protected virtual void Awake()
    {
        itemStats = GetComponent<ItemStats>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

}