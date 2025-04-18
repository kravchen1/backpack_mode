
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VampireBow1 : Weapon
{
    //private float timer1sec = 1f;
    public int countBaseCritStack = 1;

    public GameObject LogBaseCritStackCharacter, LogBaseCritStackEnemy;
    public GameObject LogChanceCritStackCharacter, LogChanceCritStackEnemy;
    public override void ActivationEffect(int resultDamage)
    {
        //добавление шанса крита
        if (Enemy.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBLEED")))
        {
            foreach (var icon in Enemy.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBLEED")))
            {
                Player.menuFightIconData.AddBuff(icon.countStack, "IconChanceCrit");
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogChanceCritStackCharacter, "Vampire bow give " + icon.countStack.ToString());
                }
                else
                {
                    CreateLogMessage(LogChanceCritStackEnemy, "Vampire bow give " + icon.countStack.ToString());
                }

            }
        }
    }

    public override void StarActivation(Item item)
    {
        if (Player != null)
        {
            Player.menuFightIconData.AddBuff(countBaseCritStack, "IconBaseCrit");
            if (Player.isPlayer)
            {
                CreateLogMessage(LogBaseCritStackCharacter, "Vampire bow give " + countBaseCritStack.ToString());
            }
            else
            {
                CreateLogMessage(LogBaseCritStackEnemy, "Vampire bow give " + countBaseCritStack.ToString());
            }
        }
    }

    protected override void FillStars()
    {
        FillnestedObjectStarsStars(256, "Weapon");
    }

    public override void ShowDescription()
    {
        //yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillStars();
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemVampireBow>();
                descr.countBaseCritStack = countBaseCritStack;
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
                descr.SetTextStat();
            }
        }
    }


}
