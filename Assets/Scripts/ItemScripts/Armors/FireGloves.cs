using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class FireGloves : Armor
{
    //public float timer_cooldown = 2.1f;
    protected bool timer_locked_out = true;
    public int countBurnStack = 2;
    public int coolDown = 50;

    private bool isUse = false;
    //private bool usable = false;
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            //timer_cooldown = baseTimerCooldown;
            //animator.speed = 1f / 0.5f;
            //timer = timer_cooldown;
            //animator.Play(originalName + "Activation");
        }
    }



    public override void StarActivation(Item item)
    {
        //Активация звёздочек(предмет огня): тратит 1 эффект горения и наносит врагу 5 урона
        if (Player != null && Enemy != null)
        {
            if (Enemy.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
            {
                bool b = false;
                foreach (var icon in Enemy.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
                {
                    if (icon.countStack >= countBurnStack)
                    {
                        //Player.menuFightIconData.DeleteBuff(SpendStack, "ICONBURN");
                        b = true;
                        //Enemy.hp -= DamageForStack;
                        //Attack(DamageForStack);
                        Debug.Log("FireGloves removed " + countBurnStack.ToString() + " burn and reset " + coolDown.ToString() + " % cooldown");
                        CreateLogMessage("FireGloves removed " + countBurnStack.ToString() + " burn and reset " + coolDown.ToString() + " % cooldown");
                        item.timer = item.timer_cooldown / 100 * coolDown;
                        //animator.SetTrigger(originalName + "StarActivation");
                        //animator.Play("New State");
                        //animator.Play(originalName + "Activation2", 0, 0f);
                        //animator.StartPlayback
                    }
                }
                if (b)
                {
                    Enemy.menuFightIconData.DeleteBuff(countBurnStack, "ICONBURN");
                    Enemy.menuFightIconData.CalculateFireFrostStats();//true = Player
                    item.animator.Play(item.originalName + "Activation", 0, 1.0f / 100 * coolDown);
                }
                else
                {
                    item.animator.Play(item.originalName + "Activation", 0, 0);
                }
            }
        }
    }



    //public void CoolDown()
    //{
    //    if (!timer_locked_outStart && timer_locked_out == true)
    //    {
    //        timer -= Time.deltaTime;

    //        if (timer <= 0)
    //        {
    //            timer = timer_cooldown;
    //            timer_locked_out = false;
    //            //animator.speed = 1f / timer_cooldown;
    //        }
    //    }
    //}


    //public override void StartActivation()
    //{
    //    if (!isUse)
    //    {
    //        if (Player != null)
    //        {
    //            Player.armor = Player.armor + startBattleArmorCount;
    //            Player.armorMax = Player.armorMax + startBattleArmorCount;
    //            isUse = true;
    //            CreateLogMessage("FireHelmet give " + startBattleArmorCount.ToString() + " armor");
    //            CheckNestedObjectActivation("StartBag");
    //        }
    //    }
    //}

    //public override void Activation()
    //{
    //    if (!timer_locked_outStart && !timer_locked_out)
    //    {
    //        timer_locked_out = true;
    //        if (Player != null)
    //        {
    //            Player.menuFightIconData.AddBuff(countBurnStack, "IconBurn");
    //            //Debug.Log("шлем дал" + countBurnStack.ToString() + " эффектов горения");
    //            CreateLogMessage("FireHelmet give " + countBurnStack.ToString() + " burn");
    //            CheckNestedObjectActivation("StartBag");
    //            CheckNestedObjectStarActivation();
    //            //var calculateFight = GameObject.FindGameObjectWithTag("CalculatedFight").GetComponent<CalculatedFight>();
    //            Player.menuFightIconData.CalculateFireFrostStats();//true = Player
    //        }
    //    }
    //}

    //private void CoolDownStart()
    //{
    //    if (timer_locked_outStart)
    //    {
    //        timerStart -= Time.deltaTime;

    //        if (timerStart <= 0)
    //        {
    //            timer_locked_outStart = false;
    //            animator.speed =  1f / timer_cooldown;
    //            StartActivation();
    //        }
    //    }
    //}
    //public override void Update()
    //{
    //    if (SceneManager.GetActiveScene().name == "BackPackBattle")
    //    {
    //        //CoolDownStart();
    //        //CoolDown();
    //        //Activation();
    //    }

    //    if (SceneManager.GetActiveScene().name == "BackPackShop")
    //    {
    //        defaultItemUpdate();
    //    }
    //}



    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSeconds(.1f);
        if (!Exit)
        {
            FillnestedObjectStarsStars(512, "RareWeapon");
            ChangeShowStars(true);
            if (canShowDescription)
            {
                    DeleteAllDescriptions();
                    CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                    var descr = CanvasDescription.GetComponent<DescriptionItemFireGloves>();
                    //descr.cooldown = timer_cooldown;
                    descr.countStack = countBurnStack;
                    descr.coolDown = coolDown;
                    descr.SetTextBody();
            }
        }
    }
}
