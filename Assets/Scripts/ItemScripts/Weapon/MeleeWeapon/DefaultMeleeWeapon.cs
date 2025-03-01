
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DefaultMeleeWeapon : Weapon
{
    //public int countBurnStackOnHit = 1;
    //public int dropFireStack;
    //public int dealDamageDropStack;
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

    public override void Activation()
    {

        if (!timer_locked_outStart && !timer_locked_out)
        {
            timer_locked_out = true;
        }
    }

    public override void StarActivation(Item item)
    {
        
    }


    //private void Burning()
    //{
    //    timer1sec -= Time.deltaTime;

    //    if (timer1sec <= 0)
    //    {
    //        timer1sec = 1f;

    //        if (gameObject.GetComponentsInChildren<Cell>().Where(e => e.nestedObject != null).Count() == 0)
    //        {
    //            if (Player != null)
    //            {
    //                Player.hp -= burningDamage;
    //                Debug.Log("Персонаж горит из-за проклятого кинжала и теряет " + burningDamage + " здоровья");
    //            }
    //        }
    //    }
    //}


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

                var descr = CanvasDescription.GetComponent<DescriptionItemDefaultWeapon>();
                //descr.hitFireStack = countBurnStackOnHit;
                //descr.dropFireStack = dropFireStack;
                //descr.dealDamageDropStack = dealDamageDropStack;
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
