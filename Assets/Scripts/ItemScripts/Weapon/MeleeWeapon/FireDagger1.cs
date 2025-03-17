
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FireDagger1 : Weapon
{
    //private float timer1sec = 1f;
    public int countBurnStackOnHit = 1;
    public int dropFireStack;
    public int dealDamageDropStack;

    public GameObject DebugFireLogCharacter, DebugFireLogEnemy;
    protected override void FillStarts()
    {
        FillnestedObjectStarsStars(256);
    }
    public override void ActivationEffect(int attack)
    {
        Enemy.menuFightIconData.AddBuff(countBurnStackOnHit, "IconBurn");
        if (Player.isPlayer)
        {
            CreateLogMessage(DebugFireLogCharacter, "FireDagger inflict " + countBurnStackOnHit.ToString());
        }
        else
        {
            CreateLogMessage(DebugFireLogEnemy, "FireDagger inflict " + countBurnStackOnHit.ToString());
        }

        Enemy.menuFightIconData.CalculateFireFrostStats();
    }
    public override void StarActivation(Item item)
    {
        if (Player != null && Enemy != null)
        {
            if (Enemy.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
            {
                bool b = false;
                foreach (var icon in Enemy.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
                {
                    if (icon.countStack >= dropFireStack)
                    {
                        b = true;
                    }
                }
                if (b)
                {
                    Enemy.menuFightIconData.DeleteBuff(dropFireStack, "ICONBURN");
                    CreateLogMessage(DebugFireLogCharacter, "FireDagger removed " + dropFireStack.ToString());
                    Enemy.menuFightIconData.CalculateFireFrostStats();//true = Player
                    Attack(dealDamageDropStack, false);
                }
            }
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

                var descr = CanvasDescription.GetComponent<DescriptionItemFireDagger>();
                descr.hitFireStack = countBurnStackOnHit;
                descr.dropFireStack = dropFireStack;
                descr.dealDamageDropStack = dealDamageDropStack;
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
