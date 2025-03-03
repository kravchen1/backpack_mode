
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VampireBow1 : Weapon
{
    //private float timer1sec = 1f;
    public int countBaseCritStack = 1;

    public GameObject LogBaseCritStackCharacter, LogBaseCritStackEnemy;
    public GameObject LogChanceCritStackCharacter, LogChanceCritStackEnemy;

    private void Start()
    {
        FillnestedObjectStarsStars(256, "weapon");
        timer_cooldown = baseTimerCooldown;
        timer = timer_cooldown;
        baseStamina = stamina;
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
                                float flDmg = (float)resultDamage * Player.menuFightIconData.CalculateCritDamage(critDamage);
                                resultDamage = (int)flDmg;
                            }
                            int block = BlockDamage();
                            if (resultDamage >= block)
                                resultDamage -= block;
                            else
                                resultDamage = 0;
                            Attack(resultDamage, true);
                            VampireHP(resultDamage);
                            //добавление шанса крита
                            if (Enemy.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBLEED")))
                            {
                                foreach (var icon in Enemy.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBLEED")))
                                {
                                    Player.menuFightIconData.AddBuff(icon.countStack, "IconChanceCrit");
                                    if (Player.isPlayer)
                                    {
                                        CreateLogMessage(LogChanceCritStackCharacter, "Vampire bow give " + icon.countStack.ToString());
                                    }
                                    else
                                    {
                                        CreateLogMessage(LogChanceCritStackEnemy, "Vampire bow give " + icon.countStack.ToString());
                                    }

                                }
                            }
                            CheckNestedObjectActivation("StartBag");
                            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
                        }
                        else
                        {
                            CreateLogMessage("Vampire bow miss", Player.isPlayer);
                        }
                    }
                    else
                    {
                        CreateLogMessage("Vampire bow  miss", Player.isPlayer);
                    }

                }
            }
            else
            {
                CreateLogMessage("Vampire bow no have stamina", Player.isPlayer);
            }
        }
    }

    public override void StarActivation(Item item)
    {
        //if(item.GetComponent<Weapon>() != null)
        //item.GetComponent<Weapon>().critDamage += critDamage / 100 * countIncreasesCritDamage;
        if (Player != null)
        {
            Player.menuFightIconData.AddBuff(countBaseCritStack, "IconBaseCrit");
            if (Player.isPlayer)
            {
                CreateLogMessage(LogBaseCritStackCharacter, "Vampire bow give " + countBaseCritStack.ToString());
            }
            else
            {
                CreateLogMessage(LogBaseCritStackEnemy, "Vampire bow give " + countBaseCritStack.ToString());
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

        //if (SceneManager.GetActiveScene().name == "BackPackShop")
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
            FillnestedObjectStarsStars(256, "weapon");
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemVampireBow>();
                descr.countBaseCritStack = countBaseCritStack;
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
