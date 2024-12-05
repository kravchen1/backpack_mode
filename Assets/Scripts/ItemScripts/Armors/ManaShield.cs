using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class ManaShield : Armor
{
    private bool isUse = false;
    public int countStartResistanceStack = 5;
    public int countNeedManaStack = 1;
    public int blockDamage = 11;
    public int countStealManaStack = 1;

    private int currentHP, currentArmor;
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            animator.speed = 1f / 0.5f;
            animator.Play(originalName + "Activation");
            if (Player != null)
            {
                currentHP = (int)Math.Round(Player.hp, 0);
                currentArmor = (int)Math.Round(Player.armor, 0);
            }
        }
        
    }
 

    public override void StartActivation()
    {
        if (!isUse)
        {
            if (Player != null)
            {
                //Player.armor = Player.armor + startBattleArmorCount;
                //Player.armorMax = Player.armorMax + startBattleArmorCount;
                Player.menuFightIconData.AddBuff(countStartResistanceStack, "ICONRESISTANCE");
                isUse = true;
                //Debug.Log("FireBody give " + startBattleArmorCount + " armor");
                CreateLogMessage("ManaShield give " + countStartResistanceStack.ToString() + " Resistance");
                CheckNestedObjectActivation("StartBag");
            }
        }
    }

    public override void StarActivation(Item item)
    {
        //Активация звёздочек(предмет огня): тратит 1 эффект горения и наносит врагу 5 урона
        if (Player != null && Enemy != null)
        {
            if (Enemy.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
            {
                bool b = false;
                foreach (var icon in Enemy.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
                {
                    if(icon.countStack >= countStealManaStack)
                    {
                        b = true;
                        CreateLogMessage("ManaShield steal " + countStealManaStack.ToString() + " mana");
                    }
                }
                if(b)
                {
                    Enemy.menuFightIconData.DeleteBuff(countStealManaStack, "ICONMANA");
                    Enemy.menuFightIconData.AddBuff(countStealManaStack, "ICONMANA");
                }
            }
        }
    }

    public override void Activation()
    {
        if (Player != null && (Player.hp != currentHP || Player.armor != currentArmor))
        {
            if (Player.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
            {
                bool b = false;
                foreach (var icon in Player.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
                {
                    if (icon.countStack >= countNeedManaStack)
                    {
                        b = true;
                        int resultBlockDamage = 0;
                        if (Player.armor != currentArmor)
                        {
                            if(currentArmor - Player.armor >= blockDamage)
                            {
                                resultBlockDamage = blockDamage;
                                Player.armor += resultBlockDamage;
                                CreateLogMessage("ManaShield remove " + countNeedManaStack.ToString() + " mana and block" + resultBlockDamage.ToString() + "damage");
                            }
                            else
                            {
                                resultBlockDamage = currentArmor;
                                int remain = blockDamage - resultBlockDamage;
                                Player.armor += currentArmor;
                                if (Player.hp != currentHP)
                                {
                                    if(currentHP - Player.hp >= remain)
                                    {
                                        resultBlockDamage += remain;
                                        Player.hp += remain;
                                        CreateLogMessage("ManaShield remove " + countNeedManaStack.ToString() + " mana and block" + resultBlockDamage.ToString() + "damage");
                                    }
                                    else
                                    {
                                        //todo
                                    }
                                }
                            }
                            if(currentArmor - Player.armor > blockDamage)
                            {
                                resultBlockDamage = 10;
                            }
                        }
                        
                        
                    }
                }
                if (b)
                {
                    Player.menuFightIconData.DeleteBuff(countNeedManaStack, "ICONMANA");
                }
            }
            animator.Play(originalName + "Activation2", 0, 0f);

        }
    }

    private void CoolDownStart()
    {
        if (timer_locked_outStart)
        {
            timerStart -= Time.deltaTime;

            if (timerStart <= 0)
            {
                timer_locked_outStart = false;
                //animator.speed = 1f / timer_cooldown;
                StartActivation();
                animator.Play("New State");
            }
        }
    }
    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            //FillnestedObjectStarsStars(512, "RareWeapon");
            CoolDownStart();
        }

        if (SceneManager.GetActiveScene().name == "BackPackShop")
        {
            defaultItemUpdate();
        }
    }

    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSeconds(.1f);
        if (!Exit)
        {
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
                var descr = CanvasDescription.GetComponent<DescriptionItemManaShield>();
                descr.countStartResistanceStack = countStartResistanceStack;
                descr.countNeedManaStack = countNeedManaStack;
                descr.blockDamage = blockDamage;
                descr.countStealManaStack = countStealManaStack;
                descr.SetTextBody();
            }
        }
    }

}
