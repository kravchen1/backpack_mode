using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class VampireBody : Armor
{
    

    public int countBleedStack = 1;
    protected bool timer_locked_out = true;



    public GameObject LogBleedStackCharacter, LogBleedStackEnemy;

    private void Start()
    {
        FillnestedObjectStarsStars(256, "Vampire");
        timer_cooldown = baseTimerCooldown;
        timer = timer_cooldown;
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
        }
    }
    public void CoolDown()
    {
        if (!timer_locked_outStart && timer_locked_out == true)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = timer_cooldown;
                timer_locked_out = false;
                animator.speed = 1f / timer_cooldown;
            }
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

                    CreateLogMessage("Vampire body give" + countBleedOnEnemy.ToString(), Player.isPlayer);

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

            if (Player.isPlayer)
            {
                CreateLogMessage(LogBleedStackCharacter, "Vampire body inflict " + countBleedStack.ToString());
            }
            else
            {
                CreateLogMessage(LogBleedStackEnemy, "Vampire body inflict " + countBleedStack.ToString());
            }
        }
    }


    private void CoolDownStart()
    {
        if (timer_locked_outStart)
        {
            timerStart -= Time.deltaTime;

            if (timerStart <= 0)
            {
                timer_locked_outStart = false;
                animator.speed = 1f / timer_cooldown;
                animator.Play(originalName + "Activation");
                //StartActivation();
            }
        }
    }

    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            CoolDownStart();
            CoolDown();
            Activation();
        }

        //if (SceneManager.GetActiveScene().name == "BackPackShop")
        else if (SceneManager.GetActiveScene().name != "GenerateMap" && SceneManager.GetActiveScene().name != "Cave")
        {
            defaultItemUpdate();
        }
    }

    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillnestedObjectStarsStars(256, "Vampire");
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
                var descr = CanvasDescription.GetComponent<DescriptionItemVampireBody>();
                descr.cooldown = timer_cooldown;
                descr.countBleedStack = countBleedStack;
                descr.SetTextBody();
            }
        }
    }

}
