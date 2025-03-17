using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class RabbitPaw : WitchCraft
{
    private bool isUse = false;
    public int giveCritStack = 2;//надо заменить
    public int giveManaStack = 4;//надо заменить

    public GameObject LogManaStackCharacter, LogManaStackEnemy;
    private void Start()
    {
        timer_cooldown = baseTimerCooldown;
        timer = timer_cooldown;

        FillnestedObjectStarsStars(256, "Mushroom", "Witchcraft");

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
            int countMana = stars.Where(e => e.GetComponent<Cell>().nestedObject != null).Count() * giveManaStack;
            Player.menuFightIconData.AddBuff(countMana, "IconMana");

            if (Player.isPlayer)
            {
                CreateLogMessage(LogManaStackCharacter, "The rabbit`s paw give " + countMana.ToString());
            }
            else
            {
                CreateLogMessage(LogManaStackEnemy, "The rabbit`s paw give " + countMana.ToString());
            }

            CheckNestedObjectActivation("StartBag");
            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
        }
    }

    public override void Activation()
    {
        if (!timer_locked_outStart && !timer_locked_out)
        {
            timer_locked_out = true;
            Player.menuFightIconData.AddBuff(giveCritStack, "IconChanceCrit");
            
            CreateLogMessage("The rabbit`s paw give " + giveCritStack.ToString(), Player.isPlayer);

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
            FillnestedObjectStarsStars(256, "Mushroom", "Witchcraft");
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemRabbitPaw>();
                descr.cooldown = timer_cooldown;
                descr.giveCritStack = giveCritStack;
                descr.giveManaStack = giveManaStack;
                descr.SetTextBody();
            }
        }
    }

}
