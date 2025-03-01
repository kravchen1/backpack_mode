using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class bag2x1Stamina : Bag
{
    public int countStaminaPercentLess = 10;
    private bool isUse = false;
    public override void StartActivation()
    {
        if (!isUse)
        {
            var cellList = gameObject.GetComponentsInChildren<Cell>();
            List<Weapon> distinctItem = new List<Weapon>();
            foreach (var cell in cellList)
            {
                if (cell.nestedObject != null)
                {
                    var item = cell.nestedObject.GetComponent<Weapon>();
                    if (item != null)
                    {
                        if (!distinctItem.Contains(item))
                        {
                            distinctItem.Add(item);
                            float decrStamina = (float)((item.stamina / 100.0) * countStaminaPercentLess);
                            if (item.stamina - decrStamina > 0)
                            {
                                item.stamina -= decrStamina;
                                CreateLogMessage("bag2x1Stamina decrease " + item.name + " stamina for " + decrStamina.ToString(), Player.isPlayer);
                            }
                        }
                    }
                }
            }
            isUse = true;
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