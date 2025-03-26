using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class GauntletBoots : Armor
{
    private bool isUse = false;
    public int armor = 2;
    private void Start()
    {
        timer_cooldown = baseTimerCooldown;
        timer = timer_cooldown;

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
                Player.armor = Player.armor + armor;
                Player.armorMax = Player.armorMax + armor;
                isUse = true;
                //Debug.Log("FireBody give " + startBattleArmorCount + " armor");
                CreateLogMessage("GauntletBoots give " + armor.ToString(), Player.isPlayer);
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

                var descr = CanvasDescription.GetComponent<DescriptionItemGauntletBoots>();
                descr.armor = armor;
            }
        }
    }

}
