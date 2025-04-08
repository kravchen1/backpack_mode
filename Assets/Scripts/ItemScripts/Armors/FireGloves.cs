using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class FireGloves : Stuff
{
    public int countBurnStack = 2;
    public int coolDown = 50;

    public GameObject LogFireStackCharacter, LogFireStackEnemy;
    public GameObject LogTimerStackCharacter, LogTimerStackEnemy;

    public override void StarActivation(Item item)
    {
        if (Player != null && Enemy != null && item.baseTimerCooldown != 0)
        {
            if (Enemy.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
            {
                bool b = false;
                foreach (var icon in Enemy.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
                {
                    if (icon.countStack >= countBurnStack)
                    {
                        b = true;
                        //CreateLogMessage("FireGloves removed " + countBurnStack.ToString() + " burn and reset " + coolDown.ToString() + " % cooldown");
                        item.timer = item.timer_cooldown / 100 * coolDown;
                        //if (Player.isPlayer)
                        //{
                        //    CreateLogMessage(LogFireStackCharacter, "FireGloves removed " + countBurnStack.ToString() + " from enemy");
                        //    CreateLogMessage(LogTimerStackCharacter, "FireGloves reset on " + coolDown.ToString() + "% " + item.originalName);
                        //}
                        //else
                        //{
                        //    CreateLogMessage(LogFireStackEnemy, "FireGloves removed " + countBurnStack.ToString() + " from enemy");
                        //    CreateLogMessage(LogTimerStackEnemy, "FireGloves reset on " + coolDown.ToString() + "% " + item.originalName);
                        //}
                        logManager.CreateLogMessageUseFromEnemy(originalName, "fire", countBurnStack, Player.isPlayer);
                        logManager.CreateLogMessageReset(originalName, "timer", countBurnStack, item.originalName, Player.isPlayer);


                    }
                }
                if (b)
                {
                    Enemy.menuFightIconData.DeleteBuff(countBurnStack, "ICONBURN");

                    Enemy.menuFightIconData.CalculateFireFrostStats();
                    item.animator.Play(item.originalName + "Activation", 0, 1.0f / 100 * coolDown);
                }
                else
                {
                    item.animator.Play(item.originalName + "Activation", 0, 0);
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

                    var descr = CanvasDescription.GetComponent<DescriptionItemFireGloves>();
                    //descr.cooldown = timer_cooldown;
                    descr.countStack = countBurnStack;
                    descr.coolDown = coolDown;
                    descr.SetTextBody();
            }
        }
    }
}
