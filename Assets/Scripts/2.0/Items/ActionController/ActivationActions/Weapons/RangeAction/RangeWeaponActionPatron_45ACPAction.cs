using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RangeWeaponActionPatron_45ACPAction : RangeWeaponAction
{
    protected override List<ItemType> getPatronsType()
    {
        List<ItemType> patrons = new List<ItemType>();
        patrons.Add(ItemType.Patron_45ACP);

        return patrons;
    }
}