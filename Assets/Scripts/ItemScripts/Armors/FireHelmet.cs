using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class FireHelmet : Armor
{
    public float timer_cooldown = 2.1f;
    protected bool timer_locked_out = true;
    public int countBurnStack = 2;

    private bool isUse = false;
    //private bool usable = false;
    private void Start()
    {
        StartActivation();
    }
 

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

        foreach(var bag in bagsWithFireBody)
        {
            bag.Activation();
        }
    }




    public void CoolDown()
    {
        if (timer_locked_out == true)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = timer_cooldown;
                timer_locked_out = false;
            }
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
                Debug.Log("FireHelmet give " + startBattleArmorCount + " armor");
                CheckNestedObjectActivation("StartBag");
            }
        }
    }

    public override void Activation()
    {
        if (timer_locked_out == false)
        {
            timer_locked_out = true;
            if (Enemy != null)
            {
                Enemy.menuFightIconData.AddBuff(countBurnStack, "IconBurn");
                Debug.Log("шлем наложил 2 ожёга на врага");
                CheckNestedObjectActivation("StartBag");
            }
        }
    }

    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            CoolDown();
            Activation();
        }

        if (SceneManager.GetActiveScene().name == "BackPackShop")
        {
            defaultItemUpdate();
        }
    }
    
}
