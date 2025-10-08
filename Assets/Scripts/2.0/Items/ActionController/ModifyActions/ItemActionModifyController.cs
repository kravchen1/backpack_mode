using UnityEngine;
using System.Collections;

public abstract class ItemActionModifyController : ItemActionController
{
    protected override void Awake()
    {
        base.Awake();
    }

    public virtual void ModifyEnableItem(GameObject item)
    {

    }

    public virtual void ModifyDisableItem(GameObject item)
    {

    }
}