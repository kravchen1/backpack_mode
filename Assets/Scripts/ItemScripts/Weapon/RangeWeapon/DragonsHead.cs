
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DragonsHead : Weapon
{
    public int fireStack  = 2;//
    public int starActivation = 10;//

    private bool isUse = false;

    public GameObject LogFireStackCharacter, LogFireStackEnemy;
    protected override void FillStarts()
    {
        FillnestedObjectStarsStars(256, "Fire", "Dragon");
    }
    public override void StartActivation()
    {
        if (!isUse)
        {
            isUse = true;
            int countFillStart = stars.Where(e => e.GetComponent<Cell>().nestedObject != null).Count();
            var changeCD = baseTimerCooldown / 100.0f * (starActivation * countFillStart);

            timer_cooldown = timer_cooldown - changeCD;
            timer = timer_cooldown;

            CheckNestedObjectActivation("StartBag");
            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
        }
    }

    public override void ActivationEffect(int resultDamage)
    {
        Player.menuFightIconData.AddBuff(fireStack, "IconBurn");
        //if (Player.isPlayer)
        //{
        //    CreateLogMessage(LogFireStackCharacter, "Dragon`s head give " + fireStack.ToString());
        //}
        //else
        //{
        //    CreateLogMessage(LogFireStackEnemy, "Dragon`s head give " + fireStack.ToString());
        //}
        logManager.CreateLogMessageGive(originalName, "fire", fireStack, Player.isPlayer);
    }

    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillnestedObjectStarsStars(256, "Fire", "Dragon");
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemDragonsHead>();
                //descr.countIncreasesCritDamage = countIncreasesCritDamage;
                descr.SetTextBody();

                
                if (Player != null)
                {
                    descr.damageMin = attackMin + Player.menuFightIconData.CalculateAddPower();
                    descr.damageMax = attackMax + Player.menuFightIconData.CalculateAddPower();
                    descr.accuracyPercent = Player.menuFightIconData.ReturnBlindAndAccuracy(accuracy);
                    descr.critDamage = (int)(Player.menuFightIconData.CalculateCritDamage(critDamage) * 100);
                    descr.chanceCrit = chanceCrit + (int)Player.menuFightIconData.CalculateChanceCrit();
                }
                else
                {
                    descr.damageMin = attackMin;
                    descr.damageMax = attackMax;
                    descr.accuracyPercent = accuracy;
                    descr.critDamage = critDamage;
                    descr.chanceCrit = chanceCrit;
                }
                descr.staminaCost = stamina;
                descr.cooldown = timer_cooldown;
                descr.starActivation = starActivation;
                descr.fireStack = fireStack;
                descr.SetTextStat();
            }
        }
    }


}
