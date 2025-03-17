
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

    public GameObject LogBleedStackCharacter, LogBleedStackEnemy;
    public GameObject LogTimerStackCharacter, LogTimerStackEnemy;

    public override void ActivationEffect(int resultDamage)
    {
        Enemy.menuFightIconData.AddDebuff(bleedingStack, "IconBleed");
        double speedUp = baseTimerCooldown / 100.0 * cooldownSpeedUp;
        timer_cooldown -= (float)speedUp;

        if (Player.isPlayer)
        {
            CreateLogMessage(LogBleedStackCharacter, "Automatic Crossbow inflict " + bleedingStack.ToString());
            CreateLogMessage(LogTimerStackCharacter, "Automatic Crossbow increased cooldown by " + Math.Round(speedUp, 2).ToString());
        }
        else
        {
            CreateLogMessage(LogBleedStackEnemy, "Automatic Crossbow inflict " + bleedingStack.ToString());
            CreateLogMessage(LogTimerStackEnemy, "Automatic Crossbow increased cooldown by " + Math.Round(speedUp, 2).ToString());
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

                var descr = CanvasDescription.GetComponent<DescriptionItemAutomaticCrossbow>();
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
                descr.bleedingStack = bleedingStack;
                descr.cooldownSpeedUp = cooldownSpeedUp;
                descr.cooldown = timer_cooldown;
                descr.SetTextStat();
            }
        }
    }


}
