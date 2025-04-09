
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MarbleMoth : Weapon
{
    public int evasionStack;

    public GameObject LogEvasionStackCharacter;
    public GameObject LogEvasionStackEnemy;


    public override void ActivationEffect(int resultDamage)
    {
        Player.menuFightIconData.AddBuff(evasionStack, "IconEvasion");
        //if (Player.isPlayer)
        //{
        //    CreateLogMessage(LogEvasionStackCharacter, "MarbleMoth give " + evasionStack.ToString());
        //}
        //else
        //{
        //    CreateLogMessage(LogEvasionStackEnemy, "MarbleMoth give " + evasionStack.ToString());
        //}
        logManager.CreateLogMessageGive(originalName, "evasion", evasionStack, Player.isPlayer);
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

                var descr = CanvasDescription.GetComponent<DescriptionItemMarbleMoth>();
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
                descr.evasionStack =evasionStack;
                descr.cooldown = timer_cooldown;
                descr.SetTextStat();
            }
        }
    }


}
