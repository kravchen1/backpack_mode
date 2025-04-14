
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DragonsTail : Weapon
{
    public int powerStack;//надо заменить
    public int blindnessStack;//надо заменить

    //public GameObject LogPowerStackCharacter, LogPowerStackEnemy;
    //public GameObject LogBlindStackCharacter, LogBlindStackEnemy;

    public override void ActivationEffect(int resultDamage)
    {
        Player.menuFightIconData.AddBuff(powerStack, "IconPower");
        Enemy.menuFightIconData.AddDebuff(blindnessStack, "IconBlind");

        //if (Player.isPlayer)
        //{
        //    CreateLogMessage(LogBlindStackCharacter, "Dragon`s tail inflict " + blindnessStack.ToString());
        //    CreateLogMessage(LogPowerStackCharacter, "Dragon`s tail give " + powerStack.ToString());
        //}
        //else
        //{
        //    CreateLogMessage(LogBlindStackCharacter, "Dragon`s tail inflict " + blindnessStack.ToString());
        //    CreateLogMessage(LogPowerStackEnemy, "Dragon`s tail give " + powerStack.ToString());
        //}
        logManager.CreateLogMessageInflict(originalName, "blind", blindnessStack, Player.isPlayer);
        logManager.CreateLogMessageGive(originalName, "power", powerStack, Player.isPlayer);
    }
    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillStars();
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemDragonsTail>();
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
                descr.cooldown = timer_cooldown;
                descr.blindnessStack = blindnessStack;
                descr.powerStack = powerStack;
                descr.SetTextStat();
            }
        }
    }


}
