using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeAndWeaponPatron556x45Action : MeleeAndRangeWeaponAction
{
    protected override List<ItemType> getPatronsType()
    {
        List<ItemType> patrons = new List<ItemType>();
        patrons.Add(ItemType.Patron556x45);

        return patrons;
    }
}