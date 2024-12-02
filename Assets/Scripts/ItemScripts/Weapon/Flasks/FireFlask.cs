using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class FireFlask : Flask
{
    //public float howActivation = 30; //при 30%(можно менять) или ниже произойдёт активация
    //public float percentHP = 20; //скок процентов сразу восстановит
    //public float percentRegenerate = 5; //скок процентов будет регенерировать
    //public float timerRegenerate = 1; //как часто в секундах будет происходить регенерация
    //public float maxTimeRegenerate = 4; //скольо раз будет происходить регенерация

    public int countStack = 5;
    public int giveStack = 15;


    private bool isUse = false;
    private bool usable = false;
    private int currentTick = 0;
    private void Start()
    {
        //timer = timerRegenerate;
        //if (SceneManager.GetActiveScene().name == "BackPackBattle")
        //{
        //    //animator.speed = 1f / 0.5f;
        //    //timer = timer_cooldown;
        //    //animator.Play(originalName + "Activation");
        //}
    }
    float f_ToPercent(float a, float p)
    {
        return (a / 100 * p); //возвращает значение из процентов, например если передать 200 и 30, то вернёт 60
    }
    public override void Activation()
    {
        if (!isUse)
        {
            if (Player != null)
            {
                foreach (var icon in Player.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
                {
                    if (icon.countStack >= countStack)
                    {
                        isUse = true;
                        Debug.Log("FiryFlask дала " + giveStack.ToString() + " эффектов горения");
                        //animator.SetTrigger(originalName + "StarActivation");
                        //animator.Play("New State");

                        animator.speed = 5f;
                        animator.Play(originalName + "Activation", 0, 0f);
                    }
                }
                if (isUse)
                {
                    Player.menuFightIconData.AddBuff(giveStack, "ICONBURN");
                    Player.menuFightIconData.CalculateFireFrostStats();//true = Player
                    CheckNestedObjectActivation("StartBag");
                    CheckNestedObjectStarActivation();

                }
            }
        }
    }

    //public void TickHeal()
    //{
    //    if (usable && currentTick < maxTimeRegenerate)
    //    {
    //        timer -= Time.deltaTime;

    //        if (timer <= 0)
    //        {
    //            timer = timerRegenerate;
    //            Player.hp += f_ToPercent(Player.maxHP, percentHP);
    //            currentTick += 1;
    //            Debug.Log("а фласочка то действует " + currentTick);
    //        // a delayed action could be called from here
    //        // once the lock-out period expires
    //        }
    //    }
    //}
    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            //CoolDownStart();
            //CoolDown();
            Activation();
        }

        if (SceneManager.GetActiveScene().name == "BackPackShop")
        {
            defaultItemUpdate();
        }
    }

    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSeconds(.1f);
        if (!Exit)
        {
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
                var descr = CanvasDescription.GetComponent<DescriptionItemFireFlask>();
                descr.giveStack = giveStack;
                descr.countStack = countStack;
                descr.SetTextBody();
            }
        }
    }

}
