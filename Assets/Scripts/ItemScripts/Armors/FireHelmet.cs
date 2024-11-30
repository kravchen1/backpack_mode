using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class FireHelmet : Armor
{
    //public float timer_cooldown = 2.1f;
    protected bool timer_locked_out = true;
    public int countBurnStack = 2;

    private bool isUse = false;
    //private bool usable = false;
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            animator.speed = 1f / 0.5f;
            timer = timer_cooldown;
            animator.Play(originalName + "Activation");
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
            }
        }
    }



    public override void StartActivation()
    {
        if (!isUse)
        {
            if (Player != null)
            {
                Player.armor = Player.armor + startBattleArmorCount;
                Player.armorMax = Player.armorMax + startBattleArmorCount;
                isUse = true;
                Debug.Log("FireHelmet give " + startBattleArmorCount + " armor");
                CheckNestedObjectActivation("StartBag");
            }
        }
    }

    public override void Activation()
    {
        if (!timer_locked_outStart && !timer_locked_out)
        {
            timer_locked_out = true;
            if (Player != null)
            {
                Player.menuFightIconData.AddBuff(countBurnStack, "IconBurn");
                Debug.Log("шлем дал" + countBurnStack.ToString() + " эффектов горения");
                CheckNestedObjectActivation("StartBag");
                CheckNestedObjectStarActivation();
                var calculateFight = GameObject.FindGameObjectWithTag("CalculatedFight").GetComponent<CalculatedFight>();
                calculateFight.calculateFireFrostStats(true);//true = Player
                animator.speed = 1f / timer_cooldown;
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
                animator.speed =  1f / timer_cooldown;
                StartActivation();
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
                    CanvasDescription.GetComponent<DescriptionItemFireHelmet>().cooldown = timer_cooldown;
                    CanvasDescription.GetComponent<DescriptionItemFireHelmet>().countStack = countBurnStack;
                    CanvasDescription.GetComponent<DescriptionItemFireHelmet>().SetTextBody();
            }
        }
    }
}
