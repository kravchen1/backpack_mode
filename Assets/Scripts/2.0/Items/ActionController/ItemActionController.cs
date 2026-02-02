using UnityEngine;
using System.Collections;

public abstract class ItemActionController : MonoBehaviour
{
    protected ItemStats itemStats;

    protected virtual void Awake()
    {
        itemStats = GetComponent<ItemStats>();
    }

}