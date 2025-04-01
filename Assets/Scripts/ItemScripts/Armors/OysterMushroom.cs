using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class OysterMushroom : Mushroom
{
    public int giveManaStack = 5;
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
            Player.menuFightIconData.AddBuff(giveManaStack, "IconMana");
            CreateLogMessage("Oyster mushroom give " + giveManaStack.ToString(), Player.isPlayer);

            CheckNestedObjectActivation("StartBag");
            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
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

                var descr = CanvasDescription.GetComponent<DescriptionItemOysterMushroom>();
                descr.cooldown = timer_cooldown;
                descr.giveManaStack = giveManaStack;
                descr.activationForStar = activationForStar;
                descr.SetTextBody();
            }
        }
    }

}
