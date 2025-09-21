
//using System;
//using System.Collections;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class Broom : Weapon
//{
//    public int activationSpeedUp;//надо заменить

//    // public GameObject LogTimerStackCharacter, LogTimerStackEnemy;

//    private float changeCD = 0;
//    private void Start()
//    {
//        FillStars();
//        timer_cooldown = baseTimerCooldown;
//        timer = timer_cooldown;
//        baseStamina = stamina;
//        if (SceneManager.GetActiveScene().name == "BackPackBattle" && ObjectInBag())
//        {
//            animator.speed = 1f / timer_cooldown;
//            animator.enabled = true;
//        }
//        if (stars[0].GetComponent<Cell>().nestedObject != null)
//        {
//            var starItem = stars[0].GetComponent<Cell>().nestedObject.GetComponent<Item>();
//            changeCD = starItem.baseTimerCooldown / 100.0f * activationSpeedUp;
//        }

//    }
//    public override void StartActivation()
//    {
//        if (stars[0].GetComponent<Cell>().nestedObject != null)
//        {
//            var starItem = stars[0].GetComponent<Cell>().nestedObject.GetComponent<Item>();
//            if (starItem.timer_cooldown >= 0)
//            {
//                if (starItem.timer_cooldown - changeCD > 0.1f)
//                    starItem.timer_cooldown = starItem.timer_cooldown - changeCD;
//                else
//                    starItem.timer_cooldown = 0.1f;
//                starItem.timer = starItem.timer_cooldown;
//                starItem.baseTimerCooldown = starItem.timer_cooldown;



//                logManager.CreateLogMessageReducedForItem(originalName, "timer", Math.Round(changeCD, 2), starItem.name, Player.isPlayer);
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

//                var descr = CanvasDescription.GetComponent<DescriptionItemBroom>();
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
//                descr.activationSpeedUp = activationSpeedUp;
//                descr.cooldown = timer_cooldown;
//                descr.SetTextStat();
//            }
//        }
//    }


//}
