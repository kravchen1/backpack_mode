
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BlackClaw : Weapon
{
    public int stealsRandomDebuff;
    //private float timer1sec = 1f;
    //public int countIncreasesCritDamage = 10;
    public GameObject LogBaseCritStackCharacter, LogBaseCritStackEnemy;
    public GameObject LogFireStackCharacter, LogFireStackEnemy;
    public GameObject LogChanceCritStackCharacter, LogChanceCritStackEnemy;
    public GameObject LogEvasionStackCharacter, LogEvasionStackEnemy;
    public GameObject LogManaStackCharacter, LogManaStackEnemy;
    public GameObject LogPowerStackCharacter, LogPowerStackEnemy;
    public GameObject LogRegenHpStackCharacter, LogRegenHpStackEnemy;
    public GameObject LogResistanceStackCharacter, LogResistanceStackEnemy;
    public GameObject LogVampireStackCharacter, LogVampireStackEnemy;

    public void CreateMessageLogSteal(string iconName, int count)
    {
        switch(iconName)
        {
            case "IconBaseCrit":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogBaseCritStackCharacter, "Black claw steal + " + count.ToString());
                }
                else
                {
                    CreateLogMessage(LogBaseCritStackEnemy, "Black claw steal + " + count.ToString());
                }
                break;
            case "IconBurn":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogFireStackCharacter, "Black claw steal + " + count.ToString());
                }
                else
                {
                    CreateLogMessage(LogFireStackEnemy, "Black claw steal + " + count.ToString());
                }
                break;
            case "IconChanceCrit":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogChanceCritStackCharacter, "Black claw steal + " + count.ToString());
                }
                else
                {
                    CreateLogMessage(LogChanceCritStackEnemy, "Black claw steal + " + count.ToString());
                }
                break;
            case "IconEvasion":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogEvasionStackCharacter, "Black claw steal + " + count.ToString());
                }
                else
                {
                    CreateLogMessage(LogEvasionStackEnemy, "Black claw steal + " + count.ToString());
                }
                break;
            case "IconMana":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogManaStackCharacter, "Black claw steal + " + count.ToString());
                }
                else
                {
                    CreateLogMessage(LogManaStackEnemy, "Black claw steal + " + count.ToString());
                }
                break;
            case "IconPower":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogPowerStackCharacter, "Black claw steal + " + count.ToString());
                }
                else
                {
                    CreateLogMessage(LogPowerStackEnemy, "Black claw steal + " + count.ToString());
                }
                break;
            case "IconResistance":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogResistanceStackCharacter, "Black claw steal + " + count.ToString());
                }
                else
                {
                    CreateLogMessage(LogResistanceStackEnemy, "Black claw steal + " + count.ToString());
                }
                break;
            case "IconRegenerate":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogRegenHpStackCharacter, "Black claw steal + " + count.ToString());
                }
                else
                {
                    CreateLogMessage(LogRegenHpStackEnemy, "Black claw steal + " + count.ToString());
                }
                break;
            case "IconVampire":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogVampireStackCharacter, "Black claw steal + " + count.ToString());
                }
                else
                {
                    CreateLogMessage(LogVampireStackEnemy, "Black claw steal + " + count.ToString());
                }
                break;
        }
    }

    public void stealBuff()
    {
        if (Enemy.menuFightIconData.icons.Where(e => e.Buff == true).Count() > 0)
        {
            var Buffs = Enemy.menuFightIconData.icons.Where(e => e.Buff == true).ToList();

            int countEnemyBuff = 0;
            foreach(var icon in Buffs)
            {
                countEnemyBuff += icon.countStack;
            }

            if(countEnemyBuff >= stealsRandomDebuff)
            {
                int stealNow = 0;
                while(stealNow < stealsRandomDebuff)
                {
                    int r = UnityEngine.Random.Range(0, Buffs.Count);
                    //Debug.Log(Buffs[r].sceneGameObjectIcon.name.Replace("(Clone)", ""));
                    string buff = Buffs[r].sceneGameObjectIcon.name.Replace("(Clone)", "");
                    Player.menuFightIconData.AddBuff(1, buff);
                    Enemy.menuFightIconData.DeleteBuff(1, buff);
                    CreateMessageLogSteal(buff, 1);
                    stealNow++;
                }
            }
            else
            {
                int stealNow = 0;
                while (stealNow < countEnemyBuff)
                {
                    int r = UnityEngine.Random.Range(0, Buffs.Count);
                    string buff = Buffs[r].sceneGameObjectIcon.name.Replace("(Clone)", "");
                    Player.menuFightIconData.AddBuff(1, buff);
                    Enemy.menuFightIconData.DeleteBuff(1, buff);
                    CreateMessageLogSteal(buff, 1);
                    stealNow++;
                }
            }
            

            
        }
    }

    public override void ActivationEffect(int resultDamage)
    {
        stealBuff();
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

                var descr = CanvasDescription.GetComponent<DescriptionItemBlackClaw>();
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
                descr.stealsRandomDebuff = stealsRandomDebuff;
                descr.cooldown = timer_cooldown;
                descr.SetTextStat();
            }
        }
    }


}
