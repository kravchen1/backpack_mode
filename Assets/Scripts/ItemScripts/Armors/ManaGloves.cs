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
                            b = true;
                        }
                    }
                    if (b)
                    {

                        Enemy.menuFightIconData.DeleteBuff(countSteelManaStack, "ICONMANA");
                        Player.menuFightIconData.AddBuff(countSteelManaStack, "ICONMANA");//true = Player

                        CreateLogMessage("Mana gloves steal " + countSteelManaStack.ToString(), Player.isPlayer);
                        CheckNestedObjectActivation("StartBag");
                        CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
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

                    var descr = CanvasDescription.GetComponent<DescriptionItemManaGloves>();
                    descr.cooldown = timer_cooldown;
                    descr.countSteelManaStack = countSteelManaStack;
                    descr.SetTextBody();
            }
        }
    }
}
