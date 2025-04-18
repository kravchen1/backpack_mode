using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class MushroomWhite : Mushroom
{
    public float stamina = 5;
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
            //Player.menuFightIconData.AddBuff(givePowerStack, "IconPower");
            if (Player.stamina + stamina <= Player.staminaMax)
            {
                Player.stamina += stamina;
            }
            else
            {
                Player.stamina = Player.staminaMax;
            }

            //CreateLogMessage("Russula mushroom give " + stamina.ToString(), Player.isPlayer);
            logManager.CreateLogMessageGive(originalName, "stamina", stamina, Player.isPlayer);

            CheckNestedObjectActivation("StartBag");
            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
        }
    }

    public override void ShowDescription()
    {
        //yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillStars();
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemWhiteMushroom>();
                descr.cooldown = timer_cooldown;
                descr.stamina = stamina;
                descr.activationForStar = activationForStar;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }

}
