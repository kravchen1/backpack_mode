
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Nunchucks : Weapon
{
    public int activationSpeedUp;//надо заменить
    public int giveCritStack;//надо заменить

   // public GameObject LogChanceCritStackCharacter, LogChanceCritStackEnemy;
   // public GameObject LogTimerStackCharacter, LogTimerStackEnemy;

    public override void ActivationEffect(int resultDamage)
    {
        Player.menuFightIconData.AddBuff(giveCritStack, "IconChanceCrit");
        double speedUp = baseTimerCooldown / 100.0 * activationSpeedUp;
        timer_cooldown -= (float)speedUp;

        //if (Player.isPlayer)
        //{
        //    CreateLogMessage(LogChanceCritStackCharacter, "Nunchucks give " + giveCritStack.ToString());
        //    CreateLogMessage(LogTimerStackCharacter, "Nunchucks increased cooldown by " + Math.Round(speedUp, 2).ToString());
        //}
        //else
        //{
        //    CreateLogMessage(LogChanceCritStackEnemy, "Nunchucks give " + giveCritStack.ToString());
        //    CreateLogMessage(LogTimerStackEnemy, "Nunchucks increased cooldown by " + Math.Round(speedUp, 2).ToString());
        //}

        logManager.CreateLogMessageGive(originalName, "chancecrit", giveCritStack, Player.isPlayer);
        logManager.CreateLogMessageReduced(originalName, "timer", Math.Round(speedUp, 2), Player.isPlayer);
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

                var descr = CanvasDescription.GetComponent<DescriptionItemNunchucks>();
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
                descr.giveCritStack = giveCritStack;
                descr.cooldown = timer_cooldown;
                descr.SetTextStat();
            }
        }
    }


}
