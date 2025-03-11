using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class BlueCrystal : Armor
{
    private bool isUse = false;
    public int manaStackChance = 5;//надо заменить
    public int manaStack = 2;//надо заменить
    private void Start()
    {
        FillnestedObjectStarsStars(256);
    }



    public override void StarActivation(Item item)
    {
        if (Player != null)
        {
            int r = Random.Range(1, 101);
            if (r <= manaStackChance)
            {
                Player.menuFightIconData.AddBuff(manaStack, "IconMana");
                CreateLogMessage("BlueCrystal give " + manaStack.ToString(), Player.isPlayer);
                CheckNestedObjectActivation("StartBag");
                CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
            }
        }
    }

    
    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            //CoolDownStart();
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

                var descr = CanvasDescription.GetComponent<DescriptionItemBlueCrystal>();
                //descr.cooldown = timer_cooldown;
                //descr.countStack = countBurnStack;
                //descr.coolDown = coolDown;
                descr.manaStackChance = manaStackChance;
                descr.manaStack = manaStack;

                descr.SetTextBody();
            }
        }
    }

}
