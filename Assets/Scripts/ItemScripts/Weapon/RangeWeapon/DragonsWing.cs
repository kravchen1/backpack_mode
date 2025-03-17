
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DragonsWing : Weapon
{
    //private float timer1sec = 1f;
    //public int countIncreasesCritDamage = 10;
    public int evasionStack;//надо заменить
    public int blindnessStack;//надо заменить

    public GameObject LogEvasionStackCharacter, LogEvasionStackEnemy;
    public GameObject LogBlindStackCharacter, LogBlindStackEnemy;


    public override void ActivationEffect(int resultDamage)
    {
        Player.menuFightIconData.AddBuff(evasionStack, "IconPower");
        Enemy.menuFightIconData.AddDebuff(blindnessStack, "IconBlind");

        if (Player.isPlayer)
        {
            CreateLogMessage(LogBlindStackCharacter, "Dragon`s tail inflict " + blindnessStack.ToString());
            CreateLogMessage(LogEvasionStackCharacter, "Dragon`s tail give " + evasionStack.ToString());
        }
        else
        {
            CreateLogMessage(LogBlindStackCharacter, "Dragon`s tail inflict " + blindnessStack.ToString());
            CreateLogMessage(LogEvasionStackEnemy, "Dragon`s tail give " + evasionStack.ToString());
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

                var descr = CanvasDescription.GetComponent<DescriptionItemDragonsWing>();
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
                descr.blindnessStack = blindnessStack;
                descr.evasionStack = evasionStack;
                descr.SetTextStat();
            }
        }
    }


}
