using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Armor : Item
{
    public int startBattleArmorCount = 0;
    protected float timer = 0f;

    public void CheckNestedObjectActivation(string objectActivation)
    {
        var bags = GameObject.FindGameObjectsWithTag(objectActivation);
        //var bagCells = GameObject.FindGameObjectsWithTag("BagCell");
        List<Bag> bagsWithFireBody = new List<Bag>();

        foreach (var bag in bags)
        {
            var bagCells = bag.GetComponentsInChildren<Cell>();
            bool find = false;
            foreach (var cell in bagCells)
            {
                if (!find)
                {
                    if (cell.nestedObject = gameObject)
                    {
                        bagsWithFireBody.Add(bag.GetComponent<Bag>());
                        find = true;
                        continue;
                    }
                }
            }
            if (find)
            {
                find = false;
                continue;
            }

        }

        foreach (var bag in bagsWithFireBody)
        {
            bag.StarActivation();
        }
    }

    public void CheckNestedObjectStarActivation()
    {
        var stars = GameObject.FindGameObjectsWithTag("StarActivation").Where(e => e.GetComponent<Cell>().nestedObject = gameObject);
        //var bagCells = GameObject.FindGameObjectsWithTag("BagCell");
        List<Bag> bagsWithFireBody = new List<Bag>();
        foreach(var star in stars)
        {
            star.GetComponentInParent<Item>().StarActivation();
        }
    }
}
