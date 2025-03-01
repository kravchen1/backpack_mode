using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class VampireAmulet : Armor
{
    //public float howActivation = 30; //при 30%(можно менять) или ниже произойдёт активация
    //public float percentHP = 20; //скок процентов сразу восстановит
    //public float percentRegenerate = 5; //скок процентов будет регенерировать
    //public float timerRegenerate = 1; //как часто в секундах будет происходить регенерация
    //public float maxTimeRegenerate = 4; //скольо раз будет происходить регенерация

    //public int hpDrop = 71;
    //public int countArmorStack = 34;
    //public int countResistStack = 10;
    //public int countSpendManaStack = 2;


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
        //if (!isUse)
        //{
        //    if (Player != null)
        //    {
        //        if(Player.hp < f_ToPercent(Player.maxHP, hpDrop))
        //        {
        //            foreach (var icon in Player.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONMANA")))
        //            {
        //                if (icon.countStack >= countSpendManaStack)
        //                {
        //                    isUse = true;
        //                    CreateLogMessage("ManaHelmet spend <u>" + countSpendManaStack.ToString() + "</u>, give <u>" + countArmorStack.ToString() + " Armor</u> and <u>" + countResistStack.ToString() + "</u> Resistance");
        //                    animator.speed = 5f;
        //                    animator.Play(originalName + "Activation", 0, 0f);
        //                }
        //            }
        //            if(isUse)
        //            {
        //                Player.menuFightIconData.DeleteBuff(countSpendManaStack, "ICONMANA");
        //                Player.armor = Player.armor + countArmorStack;
        //                Player.armorMax = Player.armorMax + countArmorStack;
        //                Player.menuFightIconData.AddBuff(countResistStack, "ICONRESISTANCE");
        //                CheckNestedObjectActivation("StartBag");
        //                CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
        //            }
        //        }
        //    }
        //}
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
            //Activation();
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
                var descr = CanvasDescription.GetComponent<DescriptionItemVampireAmulet>();
                //descr.hpDrop = hpDrop;
                //descr.countArmorStack = countArmorStack;
                //descr.countArmorStack = countResistStack;
                //descr.countArmorStack = countSpendManaStack;
                descr.SetTextBody();
            }
        }
    }

}
