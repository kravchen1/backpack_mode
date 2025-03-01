using UnityEngine;
using UnityEngine.SceneManagement;

public class bag1x1Resistance : Bag
{
    public int countResistanceStack = 1;
    private bool isUse = false;
    public override void StartActivation()
    {
        if (!isUse)
        {
            if (Player != null)
            {
                Player.menuFightIconData.AddBuff(countResistanceStack, "IconResistance");
                CreateLogMessage("bag1x1Resistance start battle player give " + countResistanceStack.ToString(), Player.isPlayer);
                isUse = true;
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

    private void Start()
    {
        //FillnestedObjectStarsStars(256);
        if (SceneManager.GetActiveScene().name == "BackPackBattle" && ObjectInBag())
        {
            animator.speed = 1f / 0.5f;
            animator.Play(originalName + "Activation");
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
            BagDefauldUpdate();
        }
    }
}