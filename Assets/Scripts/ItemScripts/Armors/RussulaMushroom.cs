using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static UnityEditor.Progress;
//using static UnityEngine.Rendering.DebugUI;

public class RussulaMushroom : Mushroom
{
    public int givePowerStack = 5;
    public int activationForStar = 2;
    
    public override void StartActivation()
    {
        int countFillStart = stars.Where(e=>e.GetComponent<Cell>().nestedObject != null).Count();
        var changeCD = baseTimerCooldown / 100.0f * (activationForStar * countFillStart);

        timer_cooldown = timer_cooldown - changeCD;
        timer = timer_cooldown;

        CheckNestedObjectActivation("StartBag");
        CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
    }

    public override void Activation()
    {
        if (!timer_locked_outStart && !timer_locked_out)
        {
            timer_locked_out = true;
            Player.menuFightIconData.AddBuff(givePowerStack, "IconPower");

            //CreateLogMessage("Russula mushroom give " + givePowerStack.ToString(), Player.isPlayer);
            logManager.CreateLogMessageGive(originalName, "power", givePowerStack, Player.isPlayer);

            CheckNestedObjectActivation("StartBag");
            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
        }
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

                var descr = CanvasDescription.GetComponent<DescriptionItemRussulaMushroom>();
                descr.cooldown = timer_cooldown;
                descr.givePowerStack = givePowerStack;
                descr.activationForStar = activationForStar;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }

}
