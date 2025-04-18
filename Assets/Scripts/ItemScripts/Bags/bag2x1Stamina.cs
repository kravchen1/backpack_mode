using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class bag2x1Stamina : Bag
{
    public int countStaminaPercentLess = 10;
    public override void StartActivation()
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
                            float decrStamina = (float)((item.baseStamina / 100.0) * countStaminaPercentLess);
                            if (item.stamina - decrStamina > 0)
                            {
                                item.stamina -= decrStamina;
                                //CreateLogMessage("bag2x1Stamina decrease " + item.name + " stamina for " + decrStamina.ToString(), Player.isPlayer);

                                logManager.CreateLogMessageDecreaseStamina(originalName, "stamina", decrStamina, item.name, Player.isPlayer);
                            }
                        }
                    }
                }
            }
    }

    public override void CoolDownStart()
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

    public override void ShowDescription()
    {
        //yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillStars();
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemBag2x1Stamina>();
                descr.countLessStamina = countStaminaPercentLess;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }
}