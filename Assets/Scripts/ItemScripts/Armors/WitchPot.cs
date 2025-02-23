using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class WitchPot : Armor
{
    private bool isUse = false;
    public int giveManaStack = 5;//надо заменить
    public int givePoisonStack = 2;//надо заменить
    public int giveRegenerationStack = 2;//надо заменить
    public int spendManaStack = 1;//надо заменить
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
        }
    }

    public override void StarActivation(Item item)
    {
        
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

                var descr = CanvasDescription.GetComponent<DescriptionItemWitchPot>();
                descr.cooldown = timer_cooldown;
                descr.giveManaStack = giveManaStack;
                descr.givePoisonStack = givePoisonStack;
                descr.giveRegenerationStack = giveRegenerationStack;
                descr.spendManaStack = spendManaStack;
                descr.SetTextBody();
            }
        }
    }

}
