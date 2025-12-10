using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeAndRangeWeaponPatron_357MagnumAction : MeleeAndRangeWeaponAction
{
    protected override List<ItemType> getPatronsType()
    {
        List<ItemType> patrons = new List<ItemType>();
        patrons.Add(ItemType.Patron_357Magnum);

        return patrons;
    }
}