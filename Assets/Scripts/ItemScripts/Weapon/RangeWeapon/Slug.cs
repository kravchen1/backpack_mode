
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class Slug : Weapon
//{
//    public int poisonStack;

//    //private float timer1sec = 1f;
//    //public int countIncreasesCritDamage = 10;

//    //public GameObject LogPoisonStackCharacter, LogPoisonStackEnemy;
//    public override void ActivationEffect(int resultDamage)
//    {
//        RemovePoison();
//    }
    

//    public void RemovePoison()
//    {
//        var poisons = Player.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.Contains("IconPoison")).ToList();
//        if (poisons.Count > 0)
//        {
//            int countRemoved = 0;
//            if (poisons[0].countStack >= poisonStack)
//            {
//                Player.menuFightIconData.DeleteBuff(poisonStack, "IconPoison");
//                countRemoved = poisonStack;
//            }
//            else
//            {
//                Player.menuFightIconData.DeleteBuff(poisons[0].countStack, "IconPoison");
//                countRemoved = poisons[0].countStack;
//            }

//            //if (Player.isPlayer)
//            //{
//            //    CreateLogMessage(LogPoisonStackCharacter, "Slug removed " + countRemoved.ToString());
//            //}
//            //else
//            //{
//            //    CreateLogMessage(LogPoisonStackEnemy, "Slug removed " + countRemoved.ToString());
//            //}
//            logManager.CreateLogMessageRemove(originalName, "poison", countRemoved, Player.isPlayer);
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

//                var descr = CanvasDescription.GetComponent<DescriptionItemSlug>();
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
//                descr.poisonStack = poisonStack;
//                descr.cooldown = timer_cooldown;
//                descr.SetTextStat();
//            }
//        }
//    }


//}
