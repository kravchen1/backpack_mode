using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ManaBoots : Armor
{
    protected bool timer_locked_out = true;
    public int countManaStack = 2;

    private bool isUse = false;
    

    private void Start()
    {
        timer_cooldown = baseTimerCooldown;
        timer = timer_cooldown;

        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
        }
    }
 

    




    public void CoolDown()
    {
        if (!timer_locked_outStart && timer_locked_out == true)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = timer_cooldown;
                timer_locked_out = false;
                animator.speed = 1f / timer_cooldown;
            }
        }
    }



    public override void Activation()
    {
        if (!timer_locked_outStart && !timer_locked_out)
        {
            timer_locked_out = true;
            if (Player != null)
            {
                Player.menuFightIconData.AddBuff(countManaStack, "IconMana");
                
                CreateLogMessage("Mana Boots give " + countManaStack.ToString(), Player.isPlayer);
                CheckNestedObjectActivation("StartBag");
                CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
                
            }
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
                animator.speed =  1f / timer_cooldown;
                animator.Play(originalName + "Activation");
                //StartActivation();
            }
        }
    }
    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            CoolDownStart();
            CoolDown();
            Activation();
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
            FillnestedObjectStarsStars(256);
            ChangeShowStars(true);
            if (canShowDescription)
            {
                    DeleteAllDescriptions();
                    CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                    var descr = CanvasDescription.GetComponent<DescriptionItemManaBoots>();
                    descr.cooldown = (float)Math.Round(timer_cooldown,2);
                    descr.countNeedManaStack = countManaStack;
                    descr.SetTextBody();
            }
        }
    }
}
