
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AutomaticCrossbow : Weapon
{
    public int bleedingStack;//надо заменить
    public int cooldownSpeedUp;//надо заменить

    //public GameObject LogBleedStackCharacter, LogBleedStackEnemy;
    //public GameObject LogTimerStackCharacter, LogTimerStackEnemy;

    private double speedUp = 0;
    public override void ActivationEffect(int resultDamage)
    {
        if(speedUp == 0)
        {
            speedUp = baseTimerCooldown / 100.0 * cooldownSpeedUp;
        }
        Enemy.menuFightIconData.AddDebuff(bleedingStack, "IconBleed");
        if (timer_cooldown - (float)speedUp >= 0.1f)
        {
            timer_cooldown -= (float)speedUp;
        }
        else
        {
            timer_cooldown = 0.1f;
        }
        baseTimerCooldown = timer_cooldown;

        //if (Player.isPlayer)
        //{
        //    CreateLogMessage(LogBleedStackCharacter, "Automatic Crossbow inflict " + bleedingStack.ToString());
        //    CreateLogMessage(LogTimerStackCharacter, "Automatic Crossbow increased cooldown by " + Math.Round(speedUp, 2).ToString());
        //}
        //else
        //{
        //    CreateLogMessage(LogBleedStackEnemy, "Automatic Crossbow inflict " + bleedingStack.ToString());
        //    CreateLogMessage(LogTimerStackEnemy, "Automatic Crossbow reduced cooldown by " + Math.Round(speedUp, 2).ToString());
        //}
        logManager.CreateLogMessageInflict(originalName, "bleed", bleedingStack, Player.isPlayer);
        logManager.CreateLogMessageReduced(originalName, "timer", Math.Round(speedUp, 2), Player.isPlayer);
    }

    public override void ShowDescription()
    {
        //yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillStars();
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemAutomaticCrossbow>();
                descr.weight = weight;
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
                descr.bleedingStack = bleedingStack;
                descr.cooldownSpeedUp = cooldownSpeedUp;
                descr.cooldown = timer_cooldown;
                descr.SetTextStat();
            }
        }
    }


}
