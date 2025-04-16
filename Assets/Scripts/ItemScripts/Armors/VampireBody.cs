using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class VampireBody : Stuff
{
    public int countBleedStack = 1;

    //public GameObject LogBleedStackCharacter, LogBleedStackEnemy;

    private void Start()
    {
        FillStars();
        timer_cooldown = baseTimerCooldown;
        timer = timer_cooldown;
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
        }
    }

    public override void Activation()
    {
        if (!timer_locked_outStart && !timer_locked_out)
        {
            timer_locked_out = true;
            if (Player != null && Enemy != null)
            {
                bool b = false;
                int countBleedOnEnemy = 0;
                if (Enemy.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBLEED")))
                {
                    foreach (var icon in Enemy.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBLEED")))
                    {
                        countBleedOnEnemy = icon.countStack;
                        b = true;
                    }
                }
                if (b)
                {
                    Player.menuFightIconData.AddBuff(countBleedOnEnemy, "ICONVAMPIRE");

                    //CreateLogMessage("Vampire body give" + countBleedOnEnemy.ToString(), Player.isPlayer);
                    logManager.CreateLogMessageGive(originalName, "vampire", countBleedOnEnemy, Player.isPlayer);

                    CheckNestedObjectActivation("StartBag");
                    CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
                }
            }
        }
    }

    public override void StarActivation(Item item)
    {
        //Активация звёздочек(предмет вампира): накладывает на врага стаки кровотечения
        if (Player != null && Enemy != null)
        {
            Enemy.menuFightIconData.AddDebuff(countBleedStack, "ICONBLEED");

            //if (Player.isPlayer)
            //{
            //    CreateLogMessage(LogBleedStackCharacter, "Vampire body inflict " + countBleedStack.ToString());
            //}
            //else
            //{
            //    CreateLogMessage(LogBleedStackEnemy, "Vampire body inflict " + countBleedStack.ToString());
            //}

            logManager.CreateLogMessageInflict(originalName, "bleed", countBleedStack, Player.isPlayer);
        }
    }


    protected override void FillStars()
    {
        FillnestedObjectStarsStars(256, "Vampire");
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
                var descr = CanvasDescription.GetComponent<DescriptionItemVampireBody>();
                descr.cooldown = timer_cooldown;
                descr.countBleedStack = countBleedStack;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }

}
