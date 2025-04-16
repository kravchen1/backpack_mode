using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class ChantarelleMushroom : Mushroom
{
    public int giveRegenerationStack = 5;
    public int activationForStar = 2;

    public override void StartActivation()
    {
        int countFillStart = stars.Where(e => e.GetComponent<Cell>().nestedObject != null).Count();
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
            Player.menuFightIconData.AddBuff(giveRegenerationStack, "IconRegenerate");
            //CreateLogMessage("Chantarelle mushroom give " + giveRegenerationStack.ToString(), Player.isPlayer);
            logManager.CreateLogMessageGive(originalName, "regenerate", giveRegenerationStack, Player.isPlayer);

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

                var descr = CanvasDescription.GetComponent<DescriptionItemChantarelleMushroom>();
                descr.cooldown = timer_cooldown;
                descr.giveRegenerationStack = giveRegenerationStack;
                descr.activationForStar = activationForStar;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }

}
