using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class RedCrystal : Armor
{
    private bool isUse = false;
    public int powerStack = 5;
    public int powerStackChance = 5;
    private void Start()
    {
        FillnestedObjectStarsStars(256);
    }


    public override void StartActivation()
    {
        if (!isUse)
        {
        }
    }

    public override void StarActivation(Item item)
    {
        if (Player != null)
        {
            int r = Random.Range(1, 101);
            if (r <= powerStackChance)
            {
                Player.menuFightIconData.AddBuff(powerStack, "IconPower");
                CreateLogMessage("RedCrystal give " + powerStack.ToString(), Player.isPlayer);
                CheckNestedObjectActivation("StartBag");
                CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
            }
        }
    }

    private void CoolDownStart()
    {
    }
    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            CoolDownStart();
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
            FillnestedObjectStarsStars(256);
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemRedCrystal>();
                //descr.cooldown = timer_cooldown;
                descr.powerStack = powerStack;
                descr.powerStackChance = powerStackChance;
                descr.SetTextBody();
            }
        }
    }

}
