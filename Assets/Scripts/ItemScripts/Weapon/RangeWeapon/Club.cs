
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Club : Weapon
{
    //private float timer1sec = 1f;
    //public int countIncreasesCritDamage = 10;
    public int blindnessChance;//надо заменить
    public int blindnessStack;//надо заменить

    //public GameObject LogBlindStackCharacter, LogBlindStackEnemy;


    public override void ActivationEffect(int resultDamage)
    {
        int r = UnityEngine.Random.Range(1, 101);
        if (r <= blindnessChance)
        {
            Enemy.menuFightIconData.AddDebuff(blindnessStack, "IconBlind");
            //if (Player.isPlayer)
            //{
            //    CreateLogMessage(LogBlindStackCharacter, "Club inflict " + blindnessStack.ToString());
            //}
            //else
            //{
            //    CreateLogMessage(LogBlindStackEnemy, "Club inflict " + blindnessStack.ToString());
            //}
            logManager.CreateLogMessageInflict(originalName, "blind", blindnessStack, Player.isPlayer);
        }
    }

    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            //FillnestedObjectStarsStars(256);
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemClub>();
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
                descr.blindnessChance = blindnessChance;
                descr.blindnessStack = blindnessStack;

                descr.SetTextStat();
            }
        }
    }


}
