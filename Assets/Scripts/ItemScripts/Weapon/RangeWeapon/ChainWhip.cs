
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class ChainWhip : Weapon
//{
//    public int debuffSteal = 2;

//    //public GameObject LogBaseCritStackCharacter, LogBaseCritStackEnemy;
//    //public GameObject LogFireStackCharacter, LogFireStackEnemy;
//    //public GameObject LogChanceCritStackCharacter, LogChanceCritStackEnemy;
//    //public GameObject LogEvasionStackCharacter, LogEvasionStackEnemy;
//    //public GameObject LogManaStackCharacter, LogManaStackEnemy;
//    //public GameObject LogPowerStackCharacter, LogPowerStackEnemy;
//    //public GameObject LogRegenHpStackCharacter, LogRegenHpStackEnemy;
//    //public GameObject LogResistanceStackCharacter, LogResistanceStackEnemy;
//    //public GameObject LogVampireStackCharacter, LogVampireStackEnemy;

    

//    public void CreateMessageLog(string iconName)
//    {
//        switch (iconName)
//        {
//            case "IconBaseCrit":
//                //if (Player.isPlayer)
//                //{
//                //    CreateLogMessage(LogBaseCritStackCharacter, "Black claw steal 1");
//                //}
//                //else
//                //{
//                //    CreateLogMessage(LogBaseCritStackEnemy, "Black claw steal 1");
//                //}
//                logManager.CreateLogMessageSteal(originalName, "basecrit", 1, Player.isPlayer);
//                break;
//            case "IconBurn":
//                //if (Player.isPlayer)
//                //{
//                //    CreateLogMessage(LogFireStackCharacter, "Black claw steal 1");
//                //}
//                //else
//                //{
//                //    CreateLogMessage(LogFireStackEnemy, "Black claw steal 1");
//                //}
//                logManager.CreateLogMessageSteal(originalName, "fire", 1, Player.isPlayer);
//                break;
//            case "IconChanceCrit":
//                //if (Player.isPlayer)
//                //{
//                //    CreateLogMessage(LogChanceCritStackCharacter, "Black claw steal 1");
//                //}
//                //else
//                //{
//                //    CreateLogMessage(LogChanceCritStackEnemy, "Black claw steal 1");
//                //}
//                logManager.CreateLogMessageSteal(originalName, "chancecrit", 1, Player.isPlayer);
//                break;
//            case "IconEvasion":
//                //if (Player.isPlayer)
//                //{
//                //    CreateLogMessage(LogEvasionStackCharacter, "Black claw steal 1");
//                //}
//                //else
//                //{
//                //    CreateLogMessage(LogEvasionStackEnemy, "Black claw steal 1");
//                //}
//                logManager.CreateLogMessageSteal(originalName, "evasion", 1, Player.isPlayer);
//                break;
//            case "IconMana":
//                //if (Player.isPlayer)
//                //{
//                //    CreateLogMessage(LogManaStackCharacter, "Black claw steal 1");
//                //}
//                //else
//                //{
//                //    CreateLogMessage(LogManaStackEnemy, "Black claw steal 1");
//                //}
//                logManager.CreateLogMessageSteal(originalName, "mana", 1, Player.isPlayer);
//                break;
//            case "IconPower":
//                //if (Player.isPlayer)
//                //{
//                //    CreateLogMessage(LogPowerStackCharacter, "Black claw steal 1");
//                //}
//                //else
//                //{
//                //    CreateLogMessage(LogPowerStackEnemy, "Black claw steal 1");
//                //}
//                logManager.CreateLogMessageSteal(originalName, "power", 1, Player.isPlayer);
//                break;
//            case "IconResistance":
//                //if (Player.isPlayer)
//                //{
//                //    CreateLogMessage(LogResistanceStackCharacter, "Black claw steal 1");
//                //}
//                //else
//                //{
//                //    CreateLogMessage(LogResistanceStackEnemy, "Black claw steal 1");
//                //}
//                logManager.CreateLogMessageSteal(originalName, "resist", 1, Player.isPlayer);
//                break;
//            case "IconRegenerate":
//                //if (Player.isPlayer)
//                //{
//                //    CreateLogMessage(LogRegenHpStackCharacter, "Black claw steal 1");
//                //}
//                //else
//                //{
//                //    CreateLogMessage(LogRegenHpStackEnemy, "Black claw steal 1");
//                //}
//                logManager.CreateLogMessageSteal(originalName, "regenerate", 1, Player.isPlayer);
//                break;
//            case "IconVampire":
//                //if (Player.isPlayer)
//                //{
//                //    CreateLogMessage(LogVampireStackCharacter, "Black claw steal 1");
//                //}
//                //else
//                //{
//                //    CreateLogMessage(LogVampireStackEnemy, "Black claw steal 1");
//                //}
//                logManager.CreateLogMessageSteal(originalName, "vampire", 1, Player.isPlayer);
//                break;
//        }
//    }


//    public void stealBuff()
//    {
//        if (Enemy.menuFightIconData.icons.Where(e => e.Buff == true).Count() > 0)
//        {
//            var Buffs = Enemy.menuFightIconData.icons.Where(e => e.Buff == true).ToList();

//            int countEnemyBuff = 0;
//            foreach (var icon in Buffs)
//            {
//                countEnemyBuff += icon.countStack;
//            }

//            if (countEnemyBuff >= debuffSteal)
//            {
//                int stealNow = 0;
//                while (stealNow < debuffSteal)
//                {
//                    int r = UnityEngine.Random.Range(0, Buffs.Count);
//                    //Debug.Log(Buffs[r].sceneGameObjectIcon.name.Replace("(Clone)", ""));
//                    string buff = Buffs[r].sceneGameObjectIcon.name.Replace("(Clone)", "");
//                    Player.menuFightIconData.AddBuff(1, buff);
//                    Enemy.menuFightIconData.DeleteBuff(1, buff);
//                    CreateMessageLog(buff);
//                    stealNow++;
//                }
//            }
//            else
//            {
//                int stealNow = 0;
//                while (stealNow < countEnemyBuff)
//                {
//                    int r = UnityEngine.Random.Range(0, Buffs.Count);
//                    string buff = Buffs[r].sceneGameObjectIcon.name.Replace("(Clone)", "");
//                    Player.menuFightIconData.AddBuff(1, buff);
//                    Enemy.menuFightIconData.DeleteBuff(1, buff);
//                    CreateMessageLog(buff);
//                    stealNow++;
//                }
//            }
//        }
//    }

//    public override void ActivationEffect(int resultDamage)
//    {
//        stealBuff();
//    }
    
//    public override void ShowDescription()
//    {
//        //yield return new WaitForSecondsRealtime(.1f);
//        if (!Exit)
//        {
//            FillStars();
//            ChangeShowStars(true);
//            if (canShowDescription)
//            {
//                DeleteAllDescriptions();
//                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

//                var descr = CanvasDescription.GetComponent<DescriptionItemChainWhip>();
//                descr.weight = weight;
//                descr.SetTextBody();


//                if (Player != null)
//                {
//                    descr.damageMin = attackMin + Player.menuFightIconData.CalculateAddPower();
//                    descr.damageMax = attackMax + Player.menuFightIconData.CalculateAddPower();
//                    descr.accuracyPercent = Player.menuFightIconData.ReturnBlindAndAccuracy(accuracy);
//                    descr.critDamage = (int)(Player.menuFightIconData.CalculateCritDamage(critDamage) * 100);
//                    descr.chanceCrit = chanceCrit + (int)Player.menuFightIconData.CalculateChanceCrit();
//                }
//                else
//                {
//                    descr.damageMin = attackMin;
//                    descr.damageMax = attackMax;
//                    descr.accuracyPercent = accuracy;
//                    descr.critDamage = critDamage;
//                    descr.chanceCrit = chanceCrit;
//                }
//                descr.staminaCost = stamina;
//                descr.cooldown = timer_cooldown;
//                descr.debuffSteal = debuffSteal;
//                descr.SetTextStat();
//            }
//        }
//    }


//}
