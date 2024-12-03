
using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FireDagger1 : Weapon
{
    //private float timer1sec = 1f;
    public int countBurnStackOnHit = 1;
    public int dropFireStack;
    public int dealDamageDropStack;
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
                            if (Player.menuFightIconData.CalculateChanceCrit())//крит
                            {
                                resultDamage *= (int)(Player.menuFightIconData.CalculatePercentBaseCrit(baseDamageCrit));
                            }
                            Attack(resultDamage);
                            Player.menuFightIconData.CalculateVampire(resultDamage);
                            Enemy.menuFightIconData.AddBuff(countBurnStackOnHit, "IconBurn");
                            //Debug.Log(gameObject.name + " повесил на врага " + countBurnStackOnHit.ToString() + " эффектов горения");
                            CreateLogMessage("FireDagger inflict " + countBurnStackOnHit.ToString() + " burn");
                            Enemy.menuFightIconData.CalculateFireFrostStats();
                            CheckNestedObjectActivation("StartBag");
                            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
                        }
                        else
                        {
                            //Debug.Log(gameObject.name + " уворот");
                            CreateLogMessage("FireDagger miss");
                        }
                    }
                    else
                    {
                        //Debug.Log(gameObject.name + " промах");
                        CreateLogMessage("FireDagger miss");
                    }

                }
            }
            else
            {
                //Debug.Log(gameObject.name + " не хватило стамины");
                CreateLogMessage("FireDagger no have stamina");
            }
        }
    }

    public override void StarActivation(Item item)
    {
        //Активация звёздочек(предмет огня): снимает 2 эффекта горения с врага и наносит врагу 5 урона
        if (Player != null && Enemy != null)
        {
            if (Enemy.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
            {
                bool b = false;
                foreach (var icon in Enemy.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
                {
                    if (icon.countStack >= dropFireStack)
                    {
                        //Player.menuFightIconData.DeleteBuff(SpendStack, "ICONBURN");
                        b = true;
                        //Enemy.hp -= dealDamageDropStack;
                        //Debug.Log(gameObject.name + " снял" + dropFireStack.ToString() + " 'эффекта огня' и нанесла 5 урона");
                        Attack(dealDamageDropStack);
                        CreateLogMessage("FireDagger removed " + dropFireStack.ToString() + " burn");
                        //animator.Play(originalName + "Activation2", 0, 0f);
                    }
                }
                if (b)
                {
                    Enemy.menuFightIconData.DeleteBuff(dropFireStack, "ICONBURN");
                    Enemy.menuFightIconData.CalculateFireFrostStats();//true = Player
                }
            }
        }
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
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemFireDagger>();
                descr.hitFireStack = countBurnStackOnHit;
                descr.dropFireStack = dropFireStack;
                descr.dealDamageDropStack = dealDamageDropStack;
                descr.SetTextBody();

                descr.damageMin = attackMin + Player.menuFightIconData.CalculateAddPower();
                descr.damageMax = attackMax + Player.menuFightIconData.CalculateAddPower();
                descr.staminaCost = stamina;
                descr.accuracyPercent = Player.menuFightIconData.ReturnBlindAndAccuracy(accuracy);
                descr.baseDamageCrit = (int)((Player.menuFightIconData.CalculatePercentBaseCrit(baseDamageCrit)) * 100);
                descr.cooldown = timer_cooldown;
                descr.SetTextStat();
            }
        }
    }


}
