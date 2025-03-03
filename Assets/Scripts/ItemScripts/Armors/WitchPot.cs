using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class WitchPot : WitchCraft
{
    private bool isUse = false;
    public int giveManaStack = 5;//надо заменить
    public int givePoisonStack = 2;//надо заменить
    public int giveRegenerationStack = 2;//надо заменить
    public int spendManaStack = 1;//надо заменить

    public GameObject LogManaStackCharacter, LogManaStackEnemy;
    public GameObject LogRegenerateStackCharacter, LogRegenerateStackEnemy;
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
            int countMana = stars.Where(e => e.GetComponent<Cell>().nestedObject != null).Count() * giveManaStack;
            Player.menuFightIconData.AddBuff(countMana, "IconMana");

            if (Player.isPlayer)
            {
                CreateLogMessage(LogManaStackCharacter, "big witch pot give " + countMana.ToString());
            }
            else
            {
                CreateLogMessage(LogManaStackEnemy, "big witch pot give " + countMana.ToString());
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
            
            Enemy.menuFightIconData.AddDebuff(givePoisonStack, "IconPoison");
            CreateLogMessage("big witch pot inflict " + givePoisonStack.ToString(), Player.isPlayer);

            if (Player.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
            {
                bool b = false;
                foreach (var icon in Player.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
                {
                    if (icon.countStack >= spendManaStack)
                    {
                        b = true;
                    }
                }
                if (b)
                {
                    Player.menuFightIconData.DeleteBuff(spendManaStack, "ICONMANA");
                    Player.menuFightIconData.AddBuff(giveRegenerationStack, "IconRegenerate");
                    if (Player.isPlayer)
                    {
                        CreateLogMessage(LogManaStackCharacter, "big witch pot used " + spendManaStack.ToString());
                        CreateLogMessage(LogRegenerateStackCharacter, "big witch pot give " + giveRegenerationStack.ToString());
                    }
                    else
                    {
                        CreateLogMessage(LogManaStackEnemy, "big witch pot used " + spendManaStack.ToString());
                        CreateLogMessage(LogRegenerateStackEnemy, "big witch pot give " + giveRegenerationStack.ToString());
                    }
                }
            }

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

                var descr = CanvasDescription.GetComponent<DescriptionItemWitchPot>();
                descr.cooldown = (float)Math.Round(timer_cooldown,2);
                descr.giveManaStack = giveManaStack;
                descr.givePoisonStack = givePoisonStack;
                descr.giveRegenerationStack = giveRegenerationStack;
                descr.spendManaStack = spendManaStack;
                descr.SetTextBody();
            }
        }
    }

}
