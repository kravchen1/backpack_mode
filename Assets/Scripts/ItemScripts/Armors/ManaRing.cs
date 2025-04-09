using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ManaRing : Stuff
{
    public int countNeedManaStack = 2;
    public int countBurnStack = 2;


    //public GameObject LogFireStackCharacter, LogFireStackEnemy;

    public override void Activation()
    {
        if (!timer_locked_outStart && !timer_locked_out)
        {
            timer_locked_out = true;
            if (Player != null && Enemy != null)
            {
                if (Player.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
                {
                    bool b = false;
                    foreach (var icon in Player.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
                    {
                        if (icon.countStack >= countNeedManaStack)
                        {
                            b = true;
                        }
                    }
                    if (b)
                    {
                        //CreateLogMessage("Mana ring spend " + countNeedManaStack.ToString(), Player.isPlayer);
                        logManager.CreateLogMessageUse(originalName, "mana", countNeedManaStack, Player.isPlayer);

                        //if (Player.isPlayer)
                        //{
                        //    CreateLogMessage(LogFireStackCharacter, "Mana ring give " + countBurnStack.ToString());
                        //}
                        //else
                        //{
                        //    CreateLogMessage(LogFireStackEnemy, "Mana ring give " + countBurnStack.ToString());
                        //}
                        logManager.CreateLogMessageGive(originalName, "fire", countBurnStack, Player.isPlayer);

                        Player.menuFightIconData.DeleteBuff(countNeedManaStack, "ICONMANA");
                        Player.menuFightIconData.AddBuff(countBurnStack, "ICONBURN");
                        Player.menuFightIconData.CalculateFireFrostStats();//true = Player
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
            FillnestedObjectStarsStars(256);
            ChangeShowStars(true);
            if (canShowDescription)
            {
                    DeleteAllDescriptions();
                    CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                    var descr = CanvasDescription.GetComponent<DescriptionItemManaRing>();
                    descr.cooldown = timer_cooldown;
                    descr.countNeedManaStack = countNeedManaStack;
                    descr.countBurnStack = countBurnStack;
                    descr.SetTextBody();
            }
        }
    }
}
