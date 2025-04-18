using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ManaBoots : Stuff
{
    public int countManaStack = 2;

    public override void Activation()
    {
        if (!timer_locked_outStart && !timer_locked_out)
        {
            timer_locked_out = true;
            if (Player != null)
            {
                Player.menuFightIconData.AddBuff(countManaStack, "IconMana");

                //CreateLogMessage("Mana Boots give " + countManaStack.ToString(), Player.isPlayer);
                logManager.CreateLogMessageGive(originalName, "mana", countManaStack, Player.isPlayer);
                CheckNestedObjectActivation("StartBag");
                CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
                
            }
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

                var descr = CanvasDescription.GetComponent<DescriptionItemManaBoots>();
                descr.cooldown = (float)Math.Round(timer_cooldown, 2);
                descr.countManaStack = countManaStack;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }
}
