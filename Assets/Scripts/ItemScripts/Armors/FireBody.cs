using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class FireBody : Armor
{
    private bool isUse = false;
    //private bool usable = false;
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            animator.enabled = true;
            StartActivation();
        }
        
    }
 

    public void CheckNestedObjectBagsActivation()
    {
        var bags = GameObject.FindGameObjectsWithTag("Bag");
        //var bagCells = GameObject.FindGameObjectsWithTag("BagCell");
        List<Bag> bagsWithFireBody = new List<Bag>();

        foreach (var bag in bags)
        {
            var bagCells = bag.GetComponentsInChildren<Cell>();
            bool find = false;
            foreach (var cell in bagCells)
            {
                if (cell.nestedObject = gameObject)
                {
                    bagsWithFireBody.Add(bag.GetComponent<Bag>());
                    find = true;
                    continue;
                }
            }
            if (find)
                continue;
        }


        foreach(var bag in bagsWithFireBody)
        {

        }
    }


    public override void StartActivation()
    {
        if (!isUse)
        {
            if (Player != null)
            {
                Player.armor = Player.armor + startBattleArmorCount;
                Player.armorMax = Player.armorMax + startBattleArmorCount;
                isUse = true;
                Debug.Log("FireBody give " + startBattleArmorCount + " armor");
            }
        }
    }

    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
           // Activation();
        }

        if (SceneManager.GetActiveScene().name == "BackPackShop")
        {
            defaultItemUpdate();
        }
    }
    
}
