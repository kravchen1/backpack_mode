using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mushroom : Item
{
    public int startBattleArmorCount = 0;
    protected bool timer_locked_out = true;
    //protected float timer = 0f;

    private void Start()
    {
        FillStars();

        timer_cooldown = baseTimerCooldown;
        timer = timer_cooldown;

        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            animator.speed = 1f / 0.5f;
            animator.Play(originalName + "Activation");
        }

    }

    public virtual void CoolDownStart()
    {
        if (timer_locked_outStart)
        {
            timerStart -= Time.deltaTime;

            if (timerStart <= 0)
            {
                timer_locked_outStart = false;
                StartActivation();
                animator.speed = 1f / timer_cooldown;
                animator.Play(originalName + "Activation");
            }
        }
    }


    public virtual void CoolDown()
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
        FillnestedObjectStarsStars(256, "Mushroom");
    }

    public override void UpdateForBattle()
    {
        CoolDownStart();
        CoolDown();
        Activation();
    }
}
