using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ManaGloves : Stuff
{
   public int countSteelManaStack = 2;
   public override void Activation()
    {
        if (!timer_locked_outStart && !timer_locked_out)
        {
            timer_locked_out = true;
            if (Player != null && Enemy != null)
            {
                if (Enemy.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
                {
                    bool b = false;
                    foreach (var icon in Enemy.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
                    {
                        if (icon.countStack >= countSteelManaStack)
                        {
                            b = true;
                        }
                    }
                    if (b)
                    {

                        Enemy.menuFightIconData.DeleteBuff(countSteelManaStack, "ICONMANA");
                        Player.menuFightIconData.AddBuff(countSteelManaStack, "ICONMANA");//true = Player

                        //CreateLogMessage("Mana gloves steal " + countSteelManaStack.ToString(), Player.isPlayer);
                        logManager.CreateLogMessageSteal(originalName, "mana", countSteelManaStack, Player.isPlayer);
                        
                        CheckNestedObjectActivation("StartBag");
                        CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
                    }
                }
            }
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

                var descr = CanvasDescription.GetComponent<DescriptionItemManaGloves>();
                descr.cooldown = timer_cooldown;
                descr.countSteelManaStack = countSteelManaStack;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }
}
