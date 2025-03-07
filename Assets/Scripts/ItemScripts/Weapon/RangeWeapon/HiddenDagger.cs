
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HiddenDagger : Weapon
{
    public int bleeding;//надо заменить

    public GameObject LogBleedStackCharacter, LogBleedStackEnemy;

    private void Start()
    {
        timer_cooldown = baseTimerCooldown;
        timer = timer_cooldown;
        baseStamina = stamina;
        if (SceneManager.GetActiveScene().name == "BackPackBattle" && ObjectInBag())
        {
               animator.speed = 1f / timer_cooldown;
               animator.enabled = true;
        }
    }
    private bool firstHit = true;
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
                            if(firstHit)
                            {
                                Enemy.menuFightIconData.AddDebuff(bleeding, "IconBleed");
                                firstHit = false;
                                if (Player.isPlayer)
                                {
                                    CreateLogMessage(LogBleedStackCharacter, "Hidden dagger inflict " + bleeding.ToString());
                                }
                                else
                                {
                                    CreateLogMessage(LogBleedStackEnemy, "Hidden dagger inflict " + bleeding.ToString());
                                }
                            }
                            Attack(resultDamage, true);
                            VampireHP(resultDamage);
                            //Debug.Log(gameObject.name + " повесил на врага " + countBurnStackOnHit.ToString() + " эффектов горения");
                            CheckNestedObjectActivation("StartBag");
                            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
                        }
                        else
                        {
                            CreateLogMessage("Hidden dagger miss", Player.isPlayer);
                        }
                    }
                    else
                    {
                        CreateLogMessage("Hidden dagger miss", Player.isPlayer);
                    }
                }
            }
            else
            {
                CreateLogMessage("Hidden dagger no have stamina", Player.isPlayer);
            }
        }
    }

    public override void StarActivation(Item item)
    {
        //if(item.GetComponent<Weapon>() != null)
        //    item.GetComponent<Weapon>().critDamage += critDamage / 100 * countIncreasesCritDamage;
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
        else if (SceneManager.GetActiveScene().name == "GenerateMap")
        {

        }
        else
        {
            defaultItemUpdate();
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

                var descr = CanvasDescription.GetComponent<DescriptionItemHiddenDagger>();
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
                descr.bleeding = bleeding;
                descr.cooldown = timer_cooldown;
                descr.SetTextStat();
            }
        }
    }


}
