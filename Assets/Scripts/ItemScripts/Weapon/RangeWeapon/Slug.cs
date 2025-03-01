
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Slug : Weapon
{
    public int poisonStack;

    //private float timer1sec = 1f;
    //public int countIncreasesCritDamage = 10;

    public GameObject LogPoisonStackCharacter, LogPoisonStackEnemy;

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

    public void RemovePoison()
    {
        var poisons = Player.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.Contains("IconPoison")).ToList();
        if (poisons.Count > 0)
        {
            int countRemoved = 0;
            if (poisons[0].countStack >= poisonStack)
            {
                Player.menuFightIconData.DeleteBuff(poisonStack, "IconPoison");
                countRemoved = poisonStack;
            }
            else
            {
                Player.menuFightIconData.DeleteBuff(poisons[0].countStack, "IconPoison");
                countRemoved = poisons[0].countStack;
            }

            if (Player.isPlayer)
            {
                CreateLogMessage(LogPoisonStackCharacter, "Slug removed " + countRemoved.ToString());
            }
            else
            {
                CreateLogMessage(LogPoisonStackEnemy, "Slug removed " + countRemoved.ToString());
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
                    if (Player.menuFightIconData.CalculateMissAccuracy(accuracy))//�������� + ����������
                    {
                        if (Enemy.menuFightIconData.CalculateMissAvasion())//������
                        {
                            resultDamage += Player.menuFightIconData.CalculateAddPower();//���������� ����
                            if (Player.menuFightIconData.CalculateChanceCrit(chanceCrit))//����
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

                            RemovePoison();

                            CheckNestedObjectActivation("StartBag");
                            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
                        }
                        else
                        {
                            CreateLogMessage("Slug miss", Player.isPlayer);
                        }
                    }
                    else
                    {
                        CreateLogMessage("Slug miss", Player.isPlayer);
                    }

                }
            }
            else
            {
                CreateLogMessage("Slug no have stamina", Player.isPlayer);
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

                var descr = CanvasDescription.GetComponent<DescriptionItemSlug>();
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
                descr.poisonStack = poisonStack;
                descr.cooldown = timer_cooldown;
                descr.SetTextStat();
            }
        }
    }


}
