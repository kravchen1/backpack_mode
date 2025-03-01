
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AngryFluff : Weapon
{
    public int removesRandomDebuff;

    public GameObject LogPoisonStackCharacter, LogPoisonStackEnemy;
    public GameObject LogFrostStackCharacter, LogFrostStackEnemy;
    public GameObject LogBlindStackCharacter, LogBlindStackEnemy;
    public GameObject LogBleedStackCharacter, LogBleedStackEnemy;

    private void Start()
    {
        //FillnestedObjectStarsStars(256);
        timer_cooldown = baseTimerCooldown;
        timer = timer_cooldown;
        if (SceneManager.GetActiveScene().name == "BackPackBattle" && ObjectInBag())
        {
               animator.speed = 1f / timer_cooldown;
               animator.enabled = true;
        }
    }

    public void CreateMessageLog(string iconName)
    {
        switch (iconName)
        {
            case "IconPoison":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogPoisonStackCharacter, "Mud worm remove 1");
                }
                else
                {
                    CreateLogMessage(LogPoisonStackEnemy, "Mud worm remove 1");
                }
                break;
            case "IconFrost":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogFrostStackCharacter, "Mud worm remove 1");
                }
                else
                {
                    CreateLogMessage(LogFrostStackEnemy, "Mud worm remove 1");
                }
                break;
            case "IconBlind":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogBlindStackCharacter, "Mud worm remove 1");
                }
                else
                {
                    CreateLogMessage(LogBleedStackEnemy, "Mud worm remove 1");
                }
                break;
            case "IconBleed":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogBleedStackCharacter, "Mud worm remove 1");
                }
                else
                {
                    CreateLogMessage(LogBleedStackEnemy, "Mud worm remove 1");
                }
                break;
        }
    }

    public void RemovedDebuff()
    {
        if (Player.menuFightIconData.icons.Where(e => e.Buff == false).Count() > 0)
        {
            var debuffs = Player.menuFightIconData.icons.Where(e => e.Buff == false).ToList();

            int countPlayerDebuff = 0;
            foreach (var icon in debuffs)
            {
                countPlayerDebuff += icon.countStack;
            }

            if (countPlayerDebuff >= removesRandomDebuff)
            {
                int removeNow = 0;
                while (removeNow < removesRandomDebuff)
                {
                    int r = UnityEngine.Random.Range(0, debuffs.Count);
                    //Debug.Log(Buffs[r].sceneGameObjectIcon.name.Replace("(Clone)", ""));
                    string debuff = debuffs[r].sceneGameObjectIcon.name.Replace("(Clone)", "");
                    Player.menuFightIconData.DeleteBuff(1, debuff);
                    CreateMessageLog(debuff);
                    removeNow++;
                }
            }
            else
            {
                int removeNow = 0;
                while (removeNow < countPlayerDebuff)
                {
                    int r = UnityEngine.Random.Range(0, debuffs.Count);
                    string debuff = debuffs[r].sceneGameObjectIcon.name.Replace("(Clone)", "");
                    Enemy.menuFightIconData.DeleteBuff(1, debuff);
                    CreateMessageLog(debuff);
                    removeNow++;
                }
            }
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

                            RemovedDebuff();
                            CheckNestedObjectActivation("StartBag");
                            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
                        }
                        else
                        {
                            CreateLogMessage("Dinosaur miss", Player.isPlayer);
                        }
                    }
                    else
                    {
                        CreateLogMessage("Dinosaur miss", Player.isPlayer);
                    }

                }
            }
            else
            {
                CreateLogMessage("Dinosaur no have stamina", Player.isPlayer);
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
            FillnestedObjectStarsStars(256);
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemAngryFluff>();
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
                descr.removesRandomDebuff = removesRandomDebuff;
                descr.cooldown = timer_cooldown;
                descr.SetTextStat();
            }
        }
    }


}
