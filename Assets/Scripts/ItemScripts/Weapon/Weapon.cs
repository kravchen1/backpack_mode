//using System;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class Item : MonoBehaviour
//{
//    private ItemStats itemStats;

//    //protected float timer = 0f;
//    protected bool timer_locked_out = true;


//    protected bool HaveStamina()
//    {
//        //todo проверка стамины
//        return true;
//    }

//    public override void UpdateForBattle()
//    {
//        CoolDown();
//        Activation();
//    }


//    public void CoolDown()
//    {
//        if (!timer_locked_outStart && timer_locked_out == true)
//        {
//            timer -= Time.deltaTime;

//            if (timer <= 0)
//            {
//                timer = timer_cooldown;
//                timer_locked_out = false;
//                animator.speed = 1f / timer_cooldown;
//            }
//        }
//    }



//    public override void Activation()
//    {

//        if (!timer_locked_outStart && !timer_locked_out)
//        {
//            timer_locked_out = true;
//            if (HaveStamina())
//            {
//                if (Player != null && Enemy != null)
//                {
//                    int resultDamage = UnityEngine.Random.Range(attackMin, attackMax + 1);
//                    if (Player.menuFightIconData.CalculateMissAccuracy(accuracy))//точность + ослепление
//                    {
//                        if (Enemy.menuFightIconData.CalculateMissAvasion())//уворот
//                        {
//                            resultDamage += Player.menuFightIconData.CalculateAddPower();//увеличение силы
//                            if (Player.menuFightIconData.CalculateChanceCrit(chanceCrit))//крит
//                            {
//                                resultDamage *= (int)(Player.menuFightIconData.CalculateCritDamage(critDamage));
//                            }
//                            int block = BlockDamage();
//                            if (resultDamage >= block)
//                                resultDamage -= block;
//                            else
//                                resultDamage = 0;
//                            Attack(resultDamage, true);
//                            VampireHP(resultDamage);

//                            ActivationEffect(resultDamage);

//                            CheckNestedObjectActivation("StartBag");
//                            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
//                        }
//                        else
//                        {
//                            //CreateLogMessage(originalName + " miss", Player.isPlayer);
//                            logManager.CreateLogMessageMiss(originalName, Player.isPlayer);
//                        }
//                    }
//                    else
//                    {
//                        //CreateLogMessage(originalName + " miss", Player.isPlayer);
//                        logManager.CreateLogMessageMiss(originalName, Player.isPlayer);
//                    }

//                }
//            }
//            else
//            {
//                //CreateLogMessage(originalName + " no have stamina", Player.isPlayer);
//                logManager.CreateLogMessageNoHaveStamina(originalName, Player.isPlayer);
//            }
//        }
//    }

//    public virtual void ActivationEffect(int resultDamage)
//    {

//    }
//}
