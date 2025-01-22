using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ManaGloves : Armor
{
    //public float timer_cooldown = 2.1f;
    protected bool timer_locked_out = true;
    public int countSteelManaStack = 2;

    private bool isUse = false;
    //private bool usable = false;
    private void Start()
    {
            timer_cooldown = baseTimerCooldown;
            //animator.speed = 1f / 0.5f;
            timer = timer_cooldown;
            //animator.Play(originalName + "Activation");
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

    public override void Activation()
    {
        if (!timer_locked_outStart && !timer_locked_out)
        {
            timer_locked_out = true;
            if (Player != null && Enemy != null)
            {
                if (Enemy.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
                {
                    bool b = false;
                    foreach (var icon in Enemy.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
                    {
                        if (icon.countStack >= countSteelManaStack)
                        {
                            //Player.menuFightIconData.DeleteBuff(SpendStack, "ICONBURN");
                            b = true;
                            //Enemy.hp -= dealDamageDropStack;
                            //Debug.Log(gameObject.name + " ����" + dropFireStack.ToString() + " '������� ����' � ������� 5 �����");
                            //Attack(dealDamageDropStack);
                            CreateLogMessage("ManaGloves steal " + countSteelManaStack.ToString() + " mana");
                            //animator.Play(originalName + "Activation2", 0, 0f);
                        }
                    }
                    if (b)
                    {
                        Enemy.menuFightIconData.DeleteBuff(countSteelManaStack, "ICONMANA");
                        Player.menuFightIconData.AddBuff(countSteelManaStack, "ICONMANA");//true = Player
                    }
                }
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
                animator.speed =  1f / timer_cooldown;
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

                    var descr = CanvasDescription.GetComponent<DescriptionItemManaGloves>();
                    descr.cooldown = timer_cooldown;
                    descr.countSteelManaStack = countSteelManaStack;
                    descr.SetTextBody();
            }
        }
    }
}
