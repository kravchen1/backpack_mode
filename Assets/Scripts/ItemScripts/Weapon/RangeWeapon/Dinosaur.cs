
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Dinosaur : Weapon
{
    public int powerStack;
    public int powerStackChance;

    public GameObject LogPowerStackCharacter, LogPowerStackEnemy;
    public override void ActivationEffect(int resultDamage)
    {
        RandomAddBuff(powerStackChance);
    }
    public void RandomAddBuff(int randomChance)
    {
        int r = UnityEngine.Random.Range(1, 101);
        if (r <= randomChance)
        {
            Player.menuFightIconData.AddBuff(powerStack, "IconPower");
            //if (Player.isPlayer)
            //{
            //    CreateLogMessage(LogPowerStackCharacter, "Dinosaur give " + powerStack.ToString());
            //}
            //else
            //{
            //    CreateLogMessage(LogPowerStackEnemy, "Dinosaur give " + powerStack.ToString());
            //}
            logManager.CreateLogMessageGive(originalName, "power", powerStack, Player.isPlayer);
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

                var descr = CanvasDescription.GetComponent<DescriptionItemDinosaur>();
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
                descr.powerStackChance = powerStackChance;
                descr.powerStack = powerStack;
                descr.cooldown = timer_cooldown;
                descr.SetTextStat();
            }
        }
    }


}
