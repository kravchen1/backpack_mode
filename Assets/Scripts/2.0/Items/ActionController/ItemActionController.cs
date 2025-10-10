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
        itemStats = GetComponent<ItemStats>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

}