using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class VampireBoots : Armor
{
    private bool isUse = false;
    public int countVampireStack = 2;
    public int countArmorStack = 15;
    //protected bool timer_locked_out = true;
    



    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            animator.speed = 1f / 0.5f;
            animator.Play(originalName + "Activation");
        }
    }

    public override void StartActivation()
    {
        if (!isUse)
        {
            if (Player != null)
            {
                Player.armor = Player.armor + countArmorStack;
                Player.armorMax = Player.armorMax + countArmorStack;
                isUse = true;
                Player.menuFightIconData.AddBuff(countVampireStack, "IconVampire");
                //Debug.Log("FireBody give " + startBattleArmorCount + " armor");
                CreateLogMessage("VampireBoots give " + countArmorStack.ToString() + " armor and " + countVampireStack.ToString() + " VampireStack");
                CheckNestedObjectActivation("StartBag");
                CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
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
                //animator.speed = 1f / timer_cooldown;
                StartActivation();
                animator.Play("New State");
            }
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
            FillnestedObjectStarsStars(256, "RareWeapon");
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
                var descr = CanvasDescription.GetComponent<DescriptionItemVampireBoots>();
                //descr.cooldown = timer_cooldown;
                descr.countArmorStack = countArmorStack;
                descr.countVampireStack = countVampireStack;
                descr.SetTextBody();
            }
        }
    }

}
