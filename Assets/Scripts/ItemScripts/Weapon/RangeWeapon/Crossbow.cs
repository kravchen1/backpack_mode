
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Crossbow : Weapon

{
    public int bleedingChance;//надо заменить
    public int bleedingStack;//надо заменить

    //public GameObject LogBleedStackCharacter, LogBleedStackEnemy;

    public override void ActivationEffect(int resultDamage)
    {
        int r = UnityEngine.Random.Range(0, 100);
        if (r <= bleedingChance)
        {
            Enemy.menuFightIconData.AddDebuff(bleedingStack, "IconBleed");
            //if (Player.isPlayer)
            //{
            //    CreateLogMessage(LogBleedStackCharacter, "Crossbow inflict " + bleedingStack.ToString());
            //}
            //else
            //{
            //    CreateLogMessage(LogBleedStackEnemy, "Crossbow inflict " + bleedingStack.ToString());
            //}
            logManager.CreateLogMessageInflict(originalName, "bleed", bleedingStack, Player.isPlayer);
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

                var descr = CanvasDescription.GetComponent<DescriptionItemCrossbow>();
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
                descr.bleedingChance = bleedingChance;
                descr.bleedingStack = bleedingStack;
                descr.SetTextStat();
            }
        }
    }


}
