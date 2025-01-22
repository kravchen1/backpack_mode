
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VampireSword1 : Weapon
{
    //private float timer1sec = 1f;
    //public int countBurnStackOnHit = 1;
    //public int dropFireStack;
    //public int dealDamageDropStack;
    private void Start()
    {
        //FillnestedObjectStarsStars(256, "RareWeapon");
        timer_cooldown = baseTimerCooldown;
        timer = timer_cooldown;
        if (SceneManager.GetActiveScene().name == "BackPackBattle" && ObjectInBag())
        {
               animator.speed = 1f / timer_cooldown;
               animator.enabled = true;
        }
    }

    public override void Activation()
    {

        if (!timer_locked_outStart && !timer_locked_out)
        {
            timer_locked_out = true;
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
                            Player.hp += Player.menuFightIconData.CalculateVampire(resultDamage);
                            if (stars.Where(e => e.GetComponent<Cell>().nestedObject != null).Count() == 0)
                                AttackSelf(resultDamage);
                            //Enemy.menuFightIconData.AddBuff(countBurnStackOnHit, "IconBurn");
                            //Debug.Log(gameObject.name + " повесил на врага " + countBurnStackOnHit.ToString() + " эффектов горения");
                            //CreateLogMessage("FireDagger inflict " + countBurnStackOnHit.ToString() + " burn");
                            //Enemy.menuFightIconData.CalculateFireFrostStats();
                            CheckNestedObjectActivation("StartBag");
                            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
                        }
                        else
                        {
                            //Debug.Log(gameObject.name + " уворот");
                            CreateLogMessage(gameObject.name + " miss");
                        }
                    }
                    else
                    {
                        //Debug.Log(gameObject.name + " промах");
                        CreateLogMessage(gameObject.name + " miss");
                    }

                }
            }
            else
            {
                //Debug.Log(gameObject.name + " не хватило стамины");
                CreateLogMessage(gameObject.name + " no have stamina");
            }
        }
    }

   


    public void CoolDown()
    {
        if (!timer_locked_outStart && timer_locked_out == true)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = timer_cooldown;
                timer_locked_out = false;
                animator.speed = 1f / timer_cooldown;
            }
        }
    }



    private void CoolDownStart()
    {
        if (timer_locked_outStart)
        {
            timerStart -= Time.deltaTime;

            if (timerStart <= 0)
            {
                timer_locked_outStart = false;
                animator.speed = 1f / timer_cooldown;
                animator.Play(originalName + "Activation");
            }
        }
    }


    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            CoolDownStart();
            CoolDown();
            Activation();
        }

        if (SceneManager.GetActiveScene().name == "BackPackShop")
        {
            defaultItemUpdate();
        }
    }


    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSeconds(.1f);
        if (!Exit)
        {
            FillnestedObjectStarsStars(256, "RareWeapon");
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemVampireSword>();

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
