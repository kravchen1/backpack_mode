
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwampDragon : Weapon
{
    public int poisonStack;//надо заменить
    public int blindnessStack;//надо заменить
    public int fireStack;//надо заменить

    //private float timer1sec = 1f;
    //public int countIncreasesCritDamage = 10;
    public GameObject LogPoisonStackCharacter, LogPoisonStackEnemy;
    public GameObject LogBlindStackCharacter, LogBlindStackEnemy;
    public GameObject LogFireStackCharacter, LogFireStackEnemy;

    public override void ActivationEffect(int resultDamage)
    {
        Enemy.menuFightIconData.AddDebuff(poisonStack, "IconPoison");
        Enemy.menuFightIconData.AddDebuff(blindnessStack, "IconBlind");

        if (Player.isPlayer)
        {
            CreateLogMessage(LogPoisonStackCharacter, "Swamp dragon inflict " + poisonStack.ToString());
            CreateLogMessage(LogBlindStackCharacter, "Swamp dragon inflict " + blindnessStack.ToString());
        }
        else
        {
            CreateLogMessage(LogPoisonStackEnemy, "Swamp dragon inflict " + poisonStack.ToString());
            CreateLogMessage(LogBlindStackEnemy, "Swamp dragon inflict " + blindnessStack.ToString());
        }
    }

    public override int BlockActivation()
    {
        if (Player != null)
        {
            Player.menuFightIconData.AddBuff(fireStack, "IconBurn");
            if (Player.isPlayer)
            {
                CreateLogMessage(LogFireStackCharacter, "Swamp dragon give  " + fireStack.ToString());
            }
            else
            {
                CreateLogMessage(LogFireStackEnemy, "Swamp dragon give  " + fireStack.ToString());
            }
        }

        return 0;
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

                var descr = CanvasDescription.GetComponent<DescriptionItemSwampDragon>();
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
                descr.poisonStack = poisonStack;
                descr.blindnessStack = blindnessStack;
                descr.fireStack = fireStack;
                descr.cooldown = timer_cooldown;
                descr.SetTextStat();
            }
        }
    }


}
