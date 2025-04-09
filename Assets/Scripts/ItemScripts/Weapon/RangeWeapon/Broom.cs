
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Broom : Weapon
{
    public int activationSpeedUp;//���� ��������

   // public GameObject LogTimerStackCharacter, LogTimerStackEnemy;


    public override void StartActivation()
    {
        if (stars[0].GetComponent<Cell>().nestedObject != null)
        {
            var starItem = stars[0].GetComponent<Cell>().nestedObject.GetComponent<Item>();
            if (starItem.timer_cooldown >= 0)
            {
                var changeCD = starItem.baseTimerCooldown / 100.0f * activationSpeedUp;
                starItem.timer_cooldown = starItem.timer_cooldown - changeCD;
                starItem.timer = starItem.timer_cooldown;

                //if (Player.isPlayer)
                //{
                //    CreateLogMessage(LogTimerStackCharacter, "Broom increased cooldown for " + starItem.name + " by " + Math.Round(changeCD, 2).ToString());
                //}
                //else
                //{
                //    CreateLogMessage(LogTimerStackCharacter, "Broom increased cooldown for " + starItem.name + " by " + Math.Round(changeCD, 2).ToString());
                //}
                logManager.CreateLogMessageReducedForItem(originalName, "timer", Math.Round(changeCD, 2), starItem.name, Player.isPlayer);
            }
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

                var descr = CanvasDescription.GetComponent<DescriptionItemBroom>();
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
                descr.activationSpeedUp = activationSpeedUp;
                descr.cooldown = timer_cooldown;
                descr.SetTextStat();
            }
        }
    }


}
