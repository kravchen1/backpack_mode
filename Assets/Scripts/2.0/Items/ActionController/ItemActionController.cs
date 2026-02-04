using UnityEngine;
using System.Collections;

public abstract class ItemActionController : MonoBehaviour
{
    protected ItemStats itemStats;
    protected bool isEquipped = false;
    protected virtual void Awake()
    {
        itemStats = GetComponent<ItemStats>();
    }
    // Метод вызывается при экипировке
    public virtual void Equip()
    {
        isEquipped = true;
        Debug.Log($"Auto weapon equipped: {gameObject.name}");
    }

    // Метод вызывается при снятии
    public virtual void Unequip()
    {
        isEquipped = false;
        Debug.Log($"Auto weapon unequipped: {gameObject.name}");
    }

}