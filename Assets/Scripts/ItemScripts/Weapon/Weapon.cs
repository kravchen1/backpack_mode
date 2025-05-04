using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Weapon : Item
{
    public int attackMin;
    public int attackMax;
    public float stamina;
    public int accuracy;
    public int critDamage = 130;
    public int chanceCrit = 5;

    [HideInInspector] public float baseStamina;

    //protected float timer = 0f;
    protected bool timer_locked_out = true;

    protected override void FillStars()
    {
        FillnestedObjectStarsStars(256);
    }

    private void Start()
    {
        FillStars();
        timer_cooldown = baseTimerCooldown;
        timer = timer_cooldown;
        baseStamina = stamina;
        if (SceneManager.GetActiveScene().name == "BackPackBattle" && ObjectInBag())
        {
            animator.speed = 1f / timer_cooldown;
            animator.enabled = true;
        }

    }
    protected bool HaveStamina()
    {
        if (Player.stamina - stamina >= 0)
        {
            Player.stamina -= stamina;
            return true;
        }
        else return false;
    }

    public int BlockDamage()
    {
        var blockItems = this.Enemy.backpack.GetComponentsInChildren<Item>().ToList().Where(e => e.tag.Contains("Block"));
        int resultBlock = 0;
        foreach (var item in blockItems)
        {
            resultBlock += item.BlockActivation();
        }

        return resultBlock;
    }

    public void VampireHP(int resultDamage)
    {
        int vampireHp = Player.menuFightIconData.CalculateVampire(resultDamage);
        if (Player.hp + vampireHp <= Player.maxHP)
        {
            Player.hp += vampireHp;
        }
        else
        {
            Player.hp = Player.maxHP;
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
                StartActivation();
                timer_locked_outStart = false;
                animator.speed = 1f / timer_cooldown;
                animator.Play(originalName + "Activation");

            }
        }
    }


    public override void UpdateForBattle()
    {
        CoolDownStart();
        CoolDown();
        Activation();
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
                            VampireHP(resultDamage);

                            ActivationEffect(resultDamage);

                            CheckNestedObjectActivation("StartBag");
                            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
                        }
                        else
                        {
                            //CreateLogMessage(originalName + " miss", Player.isPlayer);
                            logManager.CreateLogMessageMiss(originalName, Player.isPlayer);
                        }
                    }
                    else
                    {
                        //CreateLogMessage(originalName + " miss", Player.isPlayer);
                        logManager.CreateLogMessageMiss(originalName, Player.isPlayer);
                    }

                }
            }
            else
            {
                //CreateLogMessage(originalName + " no have stamina", Player.isPlayer);
                logManager.CreateLogMessageNoHaveStamina(originalName, Player.isPlayer);
            }
        }
    }

    public virtual void ActivationEffect(int resultDamage)
    {

    }
}
