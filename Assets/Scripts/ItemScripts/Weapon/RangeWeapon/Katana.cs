
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class Katana : Weapon
//{
//    public int countStealCritChance;//надо заменить

//   // public GameObject LogChacneCritCharacter, LogChacneCritEnemy;

//    public override void ActivationEffect(int resultDamage)
//    {
//        if (Enemy.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONChanceCrit")))
//        {
//            bool b = false;
//            foreach (var icon in Enemy.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONChanceCrit")))
//            {
//                if (icon.countStack >= countStealCritChance)
//                {
//                    b = true;
//                }
//            }
//            if (b)
//            {

//                Enemy.menuFightIconData.DeleteBuff(countStealCritChance, "ICONChanceCrit");
//                Player.menuFightIconData.AddBuff(countStealCritChance, "ICONChanceCrit");//true = Player

//                //if (Player.isPlayer)
//                //{
//                //    CreateLogMessage(LogChacneCritCharacter, "Katana steal " + countStealCritChance.ToString());
//                //}
//                //else
//                //{
//                //    CreateLogMessage(LogChacneCritEnemy, "Katana steal " + countStealCritChance.ToString());
//                //}
//                logManager.CreateLogMessageSteal(originalName, "chancecrit", countStealCritChance, Player.isPlayer);
//            }
//        }
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

//                var descr = CanvasDescription.GetComponent<DescriptionItemKatana>();
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
//                descr.critChance = countStealCritChance;
//                descr.cooldown = timer_cooldown;
//                descr.SetTextStat();
//            }
//        }
//    }


//}
