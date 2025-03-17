using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class ChantarelleMushroom : Mushroom
{
    private bool isUse = false;
    public int giveRegenerationStack = 5;//надо заменить
    public int activationForStar = 2;//надо заменить
    private void Start()
    {
        FillnestedObjectStarsStars(256, "Mushroom");

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
            Player.menuFightIconData.AddBuff(giveRegenerationStack, "IconRegenerate");
            CreateLogMessage("Chantarelle mushroom give " + giveRegenerationStack.ToString(), Player.isPlayer);

            CheckNestedObjectActivation("StartBag");
            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());


        }
    }

    public override void StarActivation(Item item)
    {
        
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
        else if (SceneManager.GetActiveScene().name != "GenerateMap" && SceneManager.GetActiveScene().name != "Cave")
        {
            defaultItemUpdate();
        }
    }

    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillnestedObjectStarsStars(256, "Mushroom");
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemChantarelleMushroom>();
                descr.cooldown = timer_cooldown;
                descr.giveRegenerationStack = giveRegenerationStack;
                descr.activationForStar = activationForStar;
                descr.SetTextBody();
            }
        }
    }

}
