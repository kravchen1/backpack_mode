using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stuff : Item
{
    public int startBattleArmorCount = 0;
    protected bool timer_locked_out = true;

    private void Start()
    {
        timer_cooldown = baseTimerCooldown;
        timer = timer_cooldown;
        FillStars();
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            animator.speed = 1f / 0.5f;
        }
    }

    private void CoolDownStart()
    {
        if (timer_locked_outStart)
        {
            timerStart -= Time.deltaTime;

            if (timerStart <= 0)
            {
                StartActivation();
                timer_locked_outStart = false;
                animator.speed = 1f / timer_cooldown;
                animator.Play(originalName + "Activation");
            }
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
                animator.speed = 1f / timer_cooldown;
            }
        }
    }
    protected override void FillStars()
    {
        FillnestedObjectStarsStars(256);
    }
    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            CoolDownStart();
            CoolDown();
            Activation();
        }

        else if (SceneManager.GetActiveScene().name != "GenerateMap" && SceneManager.GetActiveScene().name != "Cave" && SceneManager.GetActiveScene().name != "SceneShowItems")
        {
            defaultItemUpdate();
        }
    }
}
