using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class VampireGloves : Armor
{
    private bool isUse = false;
    public int countBleedStack = 1;
    //public int countArmorStack = 15;
    //protected bool timer_locked_out = true;

    


    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            animator.speed = 1f / 0.5f;
            //animator.Play(originalName + "Activation");
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
                //animator.speed = 1f / timer_cooldown;
                //StartActivation();
                //animator.Play("New State");
            }
        }
    }

    public override void StarActivation(Item item)
    {
        //Активация звёздочек(предмет): накладывает n кровотечения
        if(Enemy != null)
        {
            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
            Enemy.menuFightIconData.AddBuff(countBleedStack, "ICONBLEED");
            CreateLogMessage("VampireGloves apply " + countBleedStack.ToString() + " bleed on enemy");
        }
    }

    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            CoolDownStart();
            //CoolDown();
            //Activation();
        }
        else
        //if (SceneManager.GetActiveScene().name == "BackPackShop")
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
                FillnestedObjectStarsStars(256, "RareWeapon");
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
                var descr = CanvasDescription.GetComponent<DescriptionItemVampireGloves>();
                descr.countBleedStack = countBleedStack;
                //descr.countArmorStack = countArmorStack;
                //descr.countVampireStack = countVampireStack;
                descr.SetTextBody();
            }
        }
    }

}
