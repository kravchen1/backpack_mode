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
        FillnestedObjectStarsStars(256);
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            //animator.speed = 1f / 0.5f;
            //animator.Play(originalName + "Activation");
        }
    }




    public override void StarActivation(Item item)
    {
        //Активация звёздочек(предмет): накладывает n кровотечения
        if(Enemy != null)
        {
            Enemy.menuFightIconData.AddDebuff(countBleedStack, "ICONBLEED");
            CreateLogMessage("Vampire gloves inflict " + countBleedStack.ToString(), Player.isPlayer);
        }
    }

    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            //CoolDownStart();
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
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            ChangeShowStars(true);
            if (canShowDescription)
            {
                FillnestedObjectStarsStars(256);
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
