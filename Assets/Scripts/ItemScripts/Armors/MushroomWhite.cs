using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class MushroomWhite : Mushroom
{
    private bool isUse = false;
    public float stamina = 5;//надо заменить
    public int activationForStar = 2;//надо заменить
    private void Start()
    {
        FillnestedObjectStarsStars(256, "mushroom");

        timer_cooldown = baseTimerCooldown;
        timer = timer_cooldown;

        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            animator.speed = 1f / 0.5f;
            animator.Play(originalName + "Activation");
        }

    }


    public override void StartActivation()
    {
        if (!isUse)
        {
            isUse = true;
            int countFillStart = stars.Where(e => e.GetComponent<Cell>().nestedObject != null).Count();
            var changeCD = baseTimerCooldown / 100.0f * (activationForStar * countFillStart);

            timer_cooldown = timer_cooldown - changeCD;
            timer = timer_cooldown;

            CheckNestedObjectActivation("StartBag");
            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
        }
    }
    public override void Activation()
    {
        if (!timer_locked_outStart && !timer_locked_out)
        {
            timer_locked_out = true;
            //Player.menuFightIconData.AddBuff(givePowerStack, "IconPower");
            if (Player.stamina + stamina <= Player.staminaMax)
            {
                Player.stamina += stamina;
            }
            else
            {
                Player.stamina = Player.staminaMax;
            }

            CreateLogMessage("Russula mushroom give " + stamina.ToString(), Player.isPlayer);

            CheckNestedObjectActivation("StartBag");
            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
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
                StartActivation();
                animator.speed = 1f / timer_cooldown;
                animator.Play(originalName + "Activation");
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
            FillnestedObjectStarsStars(256, "mushroom");
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemWhiteMushroom>();
                descr.cooldown = timer_cooldown;
                descr.stamina = stamina;
                descr.activationForStar = activationForStar;
                descr.SetTextBody();
            }
        }
    }

}
