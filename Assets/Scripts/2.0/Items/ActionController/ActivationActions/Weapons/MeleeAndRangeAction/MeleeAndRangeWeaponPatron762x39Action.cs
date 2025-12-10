using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeAndRangeWeaponPatron762x39Action : MeleeAndRangeWeaponAction
{
    protected override List<ItemType> getPatronsType()
    {
        List<ItemType> patrons = new List<ItemType>();
        patrons.Add(ItemType.Patron762x39);

        return patrons;
    }
}