
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BlackCat : Weapon
{
    public int bleedingStack;//надо заменить
    public int resistingStack;//надо заменить
    //private float timer1sec = 1f;
    //public int countIncreasesCritDamage = 10;
    //public GameObject LogBleedStackCharacter, LogBleedStackEnemy;
    //public GameObject LogResistanceStackCharacter, LogResistanceStackEnemy;
    public override void ActivationEffect(int resultDamage)
    {
        Enemy.menuFightIconData.AddDebuff(bleedingStack, "IconBleed");
        //if (Player.isPlayer)
        //{
        //    CreateLogMessage(LogBleedStackCharacter, "Black cat inflict  " + bleedingStack.ToString());
        //}
        //else
        //{
        //    CreateLogMessage(LogBleedStackEnemy, "Black cat inflict  " + bleedingStack.ToString());
        //}
        logManager.CreateLogMessageInflict(originalName, "bleed", bleedingStack, Player.isPlayer);
    }

    public override int BlockActivation()
    {
        if (Player != null)
        {
            Player.menuFightIconData.AddBuff(resistingStack, "IconResistance");
            //if (Player.isPlayer)
            //{
            //    CreateLogMessage(LogResistanceStackCharacter, "Black cat give  " + resistingStack.ToString());
            //}
            //else
            //{
            //    CreateLogMessage(LogResistanceStackEnemy, "Black cat give  " + resistingStack.ToString());
            //}
            logManager.CreateLogMessageGive(originalName, "resist", resistingStack, Player.isPlayer);
        }

        return 0;
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

                var descr = CanvasDescription.GetComponent<DescriptionItemBlackCat>();
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
                descr.resistingStack = resistingStack;
                descr.cooldown = timer_cooldown;
                descr.SetTextStat();
            }
        }
    }


}
