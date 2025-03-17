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

    public GameObject LogResistanceStackCharacter, LogResistanceStackEnemy;
    public GameObject LogAttackStackCharacter, LogAttackStackEnemy;
    private void Start()
    {
        FillnestedObjectStarsStars(256, "Mana");
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            FillnestedObjectStarsStars(256);
            animator.speed = 1f / 0.5f;
            animator.Play(originalName + "Activation");
        }
        
    }
 

    public override void StartActivation()
    {
        if (!isUse)
        {
            if (Player != null)
            {
                
                Player.menuFightIconData.AddBuff(countStartResistanceStack, "ICONRESISTANCE");
                isUse = true;
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogResistanceStackCharacter, "Mana shield give " + countStartResistanceStack.ToString());
                }
                else
                {
                    CreateLogMessage(LogResistanceStackEnemy, "Mana shield give " + countStartResistanceStack.ToString());
                }

                CheckNestedObjectActivation("StartBag");
                CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
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
                    }
                }
                if(b)
                {
                    Enemy.menuFightIconData.DeleteBuff(countStealManaStack, "ICONMANA");
                    Player.menuFightIconData.AddBuff(countStealManaStack, "ICONMANA");
                    CreateLogMessage("Mana shield steal " + countStealManaStack.ToString(), Player.isPlayer);
                }
            }
        }
    }

    public override int BlockActivation()
    {
        if (Player != null)
        {
            bool b = false;
            foreach (var icon in Player.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
            {
                if (icon.countStack >= countNeedManaStack)
                {
                    b = true;
                }
            }
            if (b)
            {
                Player.menuFightIconData.DeleteBuff(countNeedManaStack, "ICONMANA");
                animator.Play(originalName + "Activation2", 0, 0f);

                CreateLogMessage("Mana shield spend " + countNeedManaStack.ToString(), Player.isPlayer);

                if (Player.isPlayer)
                {
                    CreateLogMessage(LogAttackStackCharacter, "Mana shield block " + blockDamage.ToString());
                }
                else
                {
                    CreateLogMessage(LogAttackStackEnemy, "Mana shield block " + blockDamage.ToString());
                }

                CheckNestedObjectActivation("StartBag");
                CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());

                return blockDamage;
            }
            else
            {
                return 0;
            }
        }
        else return 0;
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
                //animator.Play("New State");
            }
        }
    }
    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            //FillnestedObjectStarsStars(256);
            CoolDownStart();
        }

        //if (SceneManager.GetActiveScene().name == "BackPackShop")
        else if (SceneManager.GetActiveScene().name != "GenerateMap" && SceneManager.GetActiveScene().name != "Cave")
        {
            defaultItemUpdate();
        }
    }

    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillnestedObjectStarsStars(256, "Mana");
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
