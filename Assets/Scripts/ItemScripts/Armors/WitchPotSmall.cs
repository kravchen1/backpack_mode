using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class WitchPotSmall : WitchCraft
{
    public int givePoisonStack = 5;
    private void Start()
    {
        timer_cooldown = baseTimerCooldown;
        timer = timer_cooldown;

        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            animator.speed = 1f / 0.5f;
        }

    }

    public override void Activation()
    {
        if (!timer_locked_outStart && !timer_locked_out)
        {
            timer_locked_out = true;
            Enemy.menuFightIconData.AddDebuff(givePoisonStack, "IconPoison");

            //CreateLogMessage("Small witch pot inflict " + givePoisonStack.ToString(), Player.isPlayer);
            logManager.CreateLogMessageInflict(originalName, "poison", givePoisonStack, Player.isPlayer);

            CheckNestedObjectActivation("StartBag");
            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
        }
    }

    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemWitchPotSmall>();
                descr.cooldown = timer_cooldown;
                descr.givePoisonStack = givePoisonStack;
               
                descr.SetTextBody();
            }
        }
    }

}
