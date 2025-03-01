
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
        switch(iconName)
        {
            case "IconBaseCrit":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogBaseCritStackCharacter, "Black claw steal 1");
                }
                else
                {
                    CreateLogMessage(LogBaseCritStackEnemy, "Black claw steal 1");
                }
                break;
            case "IconBurn":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogFireStackCharacter, "Black claw steal 1");
                }
                else
                {
                    CreateLogMessage(LogFireStackEnemy, "Black claw steal 1");
                }
                break;
            case "IconChanceCrit":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogChanceCritStackCharacter, "Black claw steal 1");
                }
                else
                {
                    CreateLogMessage(LogChanceCritStackEnemy, "Black claw steal 1");
                }
                break;
            case "IconEvasion":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogEvasionStackCharacter, "Black claw steal 1");
                }
                else
                {
                    CreateLogMessage(LogEvasionStackEnemy, "Black claw steal 1");
                }
                break;
            case "IconMana":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogManaStackCharacter, "Black claw steal 1");
                }
                else
                {
                    CreateLogMessage(LogManaStackEnemy, "Black claw steal 1");
                }
                break;
            case "IconPower":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogPowerStackCharacter, "Black claw steal 1");
                }
                else
                {
                    CreateLogMessage(LogPowerStackEnemy, "Black claw steal 1");
                }
                break;
            case "IconResistance":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogResistanceStackCharacter, "Black claw steal 1");
                }
                else
                {
                    CreateLogMessage(LogResistanceStackEnemy, "Black claw steal 1");
                }
                break;
            case "IconRegenerate":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogRegenHpStackCharacter, "Black claw steal 1");
                }
                else
                {
                    CreateLogMessage(LogRegenHpStackEnemy, "Black claw steal 1");
                }
                break;
            case "IconVampire":
                if (Player.isPlayer)
                {
                    CreateLogMessage(LogVampireStackCharacter, "Black claw steal 1");
                }
                else
                {
                    CreateLogMessage(LogVampireStackEnemy, "Black claw steal 1");
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
                    CreateMessageLog(buff);
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
                    CreateMessageLog(buff);
                    stealNow++;
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

                            stealBuff();
                            CheckNestedObjectActivation("StartBag");
                            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
                        }
                        else
                        {
                            CreateLogMessage("Black claw miss", Player.isPlayer);
                        }
                    }
                    else
                    {
                        CreateLogMessage("Black claw miss", Player.isPlayer);
                    }

                }
            }
            else
            {
                CreateLogMessage("Black claw no have stamina", Player.isPlayer);
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
