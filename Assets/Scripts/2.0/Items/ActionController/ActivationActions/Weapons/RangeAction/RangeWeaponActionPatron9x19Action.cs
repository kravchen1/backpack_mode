using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RangeWeaponActionPatron9x19Action : RangeWeaponAction
{
    protected override List<ItemType> getPatronsType()
    {
        List<ItemType> patrons = new List<ItemType>();
        patrons.Add(ItemType.Patron9x19);

        return patrons;
    }
}