using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class VampireBody : Armor
{
    //public float howActivation = 30; //при 30%(можно менять) или ниже произойдёт активация
    //public float percentHP = 20; //скок процентов сразу восстановит
    //public float percentRegenerate = 5; //скок процентов будет регенерировать
    //public float timerRegenerate = 1; //как часто в секундах будет происходить регенерация
    //public float maxTimeRegenerate = 4; //скольо раз будет происходить регенерация

    public int countBleedStack = 1;
    protected bool timer_locked_out = true;
    
    //public int countArmorStack = 34;
    //public int countResistStack = 10;
    //public int countSpendManaStack = 2;



    private void Start()
    {
        timer_cooldown = baseTimerCooldown;
        timer = timer_cooldown;
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            //animator.speed = 1f / 0.5f;
            //animator.Play(originalName + "Activation");
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
                    //CreateLogMessage("VampireBody add " + countBleedOnEnemy.ToString() + " vampireStack");
                    Player.menuFightIconData.AddBuff(countBleedStack, "ICONVAMPIRE");
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
            Enemy.menuFightIconData.AddBuff(countBleedStack, "ICONBLEED");
            //CreateLogMessage("VampireBody applie " + countBleedStack.ToString() + " bleed");
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
        else
        {
            defaultItemUpdate();
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
                var descr = CanvasDescription.GetComponent<DescriptionItemVampireBody>();
                descr.cooldown = timer_cooldown;
                descr.countBleedStack = countBleedStack;
                descr.SetTextBody();
            }
        }
    }

}
