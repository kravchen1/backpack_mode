
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HiddenDagger : Weapon
{
    public int bleeding;//надо заменить

    //public GameObject LogBleedStackCharacter, LogBleedStackEnemy;

    private bool firstHit = true;
    public override void ActivationEffect(int resultDamage)
    {
        if (firstHit)
        {
            Enemy.menuFightIconData.AddDebuff(bleeding, "IconBleed");
            firstHit = false;
            //if (Player.isPlayer)
            //{
            //    CreateLogMessage(LogBleedStackCharacter, "Hidden dagger inflict " + bleeding.ToString());
            //}
            //else
            //{
            //    CreateLogMessage(LogBleedStackEnemy, "Hidden dagger inflict " + bleeding.ToString());
            //}
            logManager.CreateLogMessageInflict(originalName, "bleed", bleeding, Player.isPlayer);
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

                var descr = CanvasDescription.GetComponent<DescriptionItemHiddenDagger>();
                //descr.countIncreasesCritDamage = countIncreasesCritDamage;

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
                descr.bleeding = bleeding;
                descr.cooldown = timer_cooldown;
                descr.SetTextBody();
                descr.SetTextStat();
            }
        }
    }


}
