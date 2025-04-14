using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class RabbitPaw : WitchCraft
{
    public int giveCritStack = 2;
    public int giveManaStack = 4;

    //public GameObject LogManaStackCharacter, LogManaStackEnemy;
    private void Start()
    {
        timer_cooldown = baseTimerCooldown;
        timer = timer_cooldown;

        FillStars();

        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            animator.speed = 1f / 0.5f;
            animator.Play(originalName + "Activation");
        }

    }

    public override void StartActivation()
    {
        int countMana = stars.Where(e => e.GetComponent<Cell>().nestedObject != null).Count() * giveManaStack;
        Player.menuFightIconData.AddBuff(countMana, "IconMana");

        //if (Player.isPlayer)
        //{
        //    CreateLogMessage(LogManaStackCharacter, "The rabbit`s paw give " + countMana.ToString());
        //}
        //else
        //{
        //    CreateLogMessage(LogManaStackEnemy, "The rabbit`s paw give " + countMana.ToString());
        //}

        logManager.CreateLogMessageGive(originalName, "mana", countMana, Player.isPlayer);

        CheckNestedObjectActivation("StartBag");
        CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
    }

    public override void Activation()
    {
        if (!timer_locked_outStart && !timer_locked_out)
        {
            timer_locked_out = true;
            Player.menuFightIconData.AddBuff(giveCritStack, "IconChanceCrit");

            //CreateLogMessage("The rabbit`s paw give " + giveCritStack.ToString(), Player.isPlayer);

            logManager.CreateLogMessageGive(originalName, "chanceCrit", giveCritStack, Player.isPlayer);

            CheckNestedObjectActivation("StartBag");
            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
        }
    }


    protected override void FillStars()
    {
        FillnestedObjectStarsStars(256, "Mushroom", "Witchcraft");
    }
    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillStars();
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemRabbitPaw>();
                descr.cooldown = timer_cooldown;
                descr.giveCritStack = giveCritStack;
                descr.giveManaStack = giveManaStack;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }

}
