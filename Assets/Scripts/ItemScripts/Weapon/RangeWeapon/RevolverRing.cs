
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RevolverRing : Weapon
{

    public void ActiovationNow()
    {
        if (HaveStamina())
        {
            if (Player != null && Enemy != null)
            {
                int resultDamage = UnityEngine.Random.Range(attackMin, attackMax + 1);
                if (Player.menuFightIconData.CalculateMissAccuracy(accuracy))//точность + ослепление
                {
                    if (Enemy.menuFightIconData.CalculateMissAvasion())//уворот
                    {
                        resultDamage += Player.menuFightIconData.CalculateAddPower();//увеличение силы
                        if (Player.menuFightIconData.CalculateChanceCrit(chanceCrit))//крит
                        {
                            resultDamage *= (int)(Player.menuFightIconData.CalculateCritDamage(critDamage));
                        }
                        int block = BlockDamage();
                        if (resultDamage >= block)
                            resultDamage -= block;
                        else
                            resultDamage = 0;
                        Attack(resultDamage, true);
                        VampireHP(resultDamage);

                        CheckNestedObjectActivation("StartBag");
                        CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
                    }
                    else
                    {
                        CreateLogMessage("Revolver ring miss", Player.isPlayer);
                    }
                }
                else
                {
                    CreateLogMessage("Revolver ring miss", Player.isPlayer);
                }
            }
        }
        else
        {
            CreateLogMessage("Revolver ring no have stamina", Player.isPlayer);
        }
    }
    public override void StarActivation(Item item)
    {
        ActiovationNow();
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

                var descr = CanvasDescription.GetComponent<DescriptionItemRevolverRing>();
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
                descr.SetTextStat();
            }
        }
    }


}
