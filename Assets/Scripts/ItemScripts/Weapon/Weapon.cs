using System;
using System.Linq;
using UnityEngine;

public class Weapon : Item
{
    public int attackMin;
    public int attackMax;
    public int stamina;
    public int accuracy;
    public int critDamage = 130;
    public int chanceCrit = 5;

    //protected float timer = 0f;
    protected bool timer_locked_out = true;


    protected bool HaveStamina()
    {
        if (Player.stamina - stamina >= 0)
            return true;
        else return false;
    }

    public int BlockDamage()
    {
        var blockItems = this.Enemy.backpack.GetComponentsInChildren<Item>().ToList().Where(e => e.tag.Contains("Block"));
        int resultBlock = 0;
        foreach (var item in blockItems)
        {
            resultBlock += item.BlockActivation();
        }

        return resultBlock;
    }

    
}
