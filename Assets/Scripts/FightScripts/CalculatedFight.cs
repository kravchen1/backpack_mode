using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CalculatedFight : MonoBehaviour
{
    public float countPercentFire = 0.01f;
    public float countPercentFrost = 0.01f;

    public void calculateFireFrostStats(bool Player)
    {
        FightMenuBuffAndDebuffs MenuFightIcons;
        List<Item> allItems = new List<Item>();
        GameObject backpack;
        if (Player)
        {
            backpack = GameObject.FindGameObjectWithTag("backpack");
            allItems = backpack.GetComponentsInChildren<Item>().ToList();
            MenuFightIcons = GameObject.FindGameObjectWithTag("MenuFightPlayer").GetComponent<FightMenuBuffAndDebuffs>();
        }
        else
        {
            backpack = GameObject.FindGameObjectWithTag("backpackEnemy");
            allItems = backpack.GetComponentsInChildren<Item>().ToList();
            MenuFightIcons = GameObject.FindGameObjectWithTag("MenuFightEnemy").GetComponent<FightMenuBuffAndDebuffs>();
        }

        if (MenuFightIcons.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")) || MenuFightIcons.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONFROST")))
        {
            int countBurn = 0, countFrost = 0;
            foreach (var icon in MenuFightIcons.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
            {
                countBurn = icon.countStack;
            }
            foreach (var icon in MenuFightIcons.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONFROST")))
            {
                countFrost = icon.countStack;
            }

            foreach (var item in allItems)
            {
                var changeCD = item.baseTimerCooldown * (countPercentFire * (countBurn - countFrost));
                if (item.baseTimerCooldown - changeCD > 0.1f)
                    item.timer_cooldown = item.baseTimerCooldown - changeCD;
                else
                    item.timer_cooldown = 0.1f;
            }
        }
    }

    public void calculateTakeDamage()
    {

    }

    public int calculateInflictDamage()
    {
        return 1;
    }

    public void calculateHeal()
    {

    }

}