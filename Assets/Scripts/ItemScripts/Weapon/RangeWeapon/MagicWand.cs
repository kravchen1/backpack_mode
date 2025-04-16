
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MagicWand : Weapon
{
    public int giveManaStack;//надо заменить
    //public GameObject LogManaStackCharacter, LogManaStackEnemy;

    public override void ActivationEffect(int resultDamage)
    {
        Player.menuFightIconData.AddBuff(giveManaStack, "IconMana");

        //if (Player.isPlayer)
        //{
        //    CreateLogMessage(LogManaStackCharacter, "Magic wand give " + giveManaStack.ToString());
        //}
        //else
        //{
        //    CreateLogMessage(LogManaStackEnemy, "Magic wand give " + giveManaStack.ToString());
        //}
        logManager.CreateLogMessageGive(originalName, "mana", giveManaStack, Player.isPlayer);
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

                var descr = CanvasDescription.GetComponent<DescriptionItemMagicWand>();
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
                descr.giveManaStack = giveManaStack;
                descr.cooldown = timer_cooldown;
                descr.SetTextStat();
            }
        }
    }


}
