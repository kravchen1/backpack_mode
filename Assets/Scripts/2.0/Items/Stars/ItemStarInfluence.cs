using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemStarInfluence : ItemStar
{
    protected override void StarActionEnable()
    {
        _currentItem.GetComponent<ItemActionModifyController>().ModifyEnableItem(gameObject.transform.parent.parent.gameObject);
    }

    protected override void StarActionDisable()
    {
        _currentItem.GetComponent<ItemActionModifyController>().ModifyDisableItem(gameObject.transform.parent.parent.gameObject);
    }
}