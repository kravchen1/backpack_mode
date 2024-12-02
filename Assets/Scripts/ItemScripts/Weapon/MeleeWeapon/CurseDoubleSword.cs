using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurseDoubleSword : Weapon
{
    private float timer1sec = 1f;
    public float burningDamage = 25f;
    private void Start()
    {
        FillnestedObjectStarsStars(256, "gloves");
        timer = timer_cooldown;
        if (SceneManager.GetActiveScene().name == "BackPackBattle" && ObjectInBag())
        {

            animator.speed = 1f / timer_cooldown;
            animator.enabled = true;
        }
    }

    public override void Activation()
    {

        if (timer_locked_out == false)
        {
            timer_locked_out = true;
            if (Player != null)
            {
                // do things
                if (Player.stamina - stamina >= 0)
                {
                    Player.stamina -= stamina;
                    if (UnityEngine.Random.Range(0, 100) <= accuracy)
                    {
                        float armorBefore = Enemy.armor;
                        int attack = UnityEngine.Random.Range(attackMin, attackMax + 1);
                        if (Enemy.armor > 0)
                        {
                            Enemy.armor -= attack;

                            if (Enemy.armor < 0)
                            {
                                Enemy.hp = Enemy.hp + Enemy.armor - attack;
                                Debug.Log("Проклятый одноручный кинжал ломает " + armorBefore + " брони и режет плоть на " + (Enemy.armor - attack) + " здоровья");
                            }
                            else
                            {
                                Debug.Log("Проклятый одноручный кинжал ломает " + armorBefore + " брони");
                            }
                        }
                        else
                        {
                            Enemy.hp -= attack;
                            Debug.Log("Проклятый одноручный кинжал режет плоть на " + attack + " здоровья");
                        }

                    }
                    else
                    {
                        Debug.Log("miss");
                    }
                }
            }

        }
    }

    private void Burning()
    {
        timer1sec -= Time.deltaTime;

        if (timer1sec <= 0)
        {
            timer1sec = 1f;

            if (gameObject.GetComponentsInChildren<Cell>().Where(e => e.nestedObject != null).Count() == 0)
            {
                if (Player != null)
                {
                    Player.hp -= burningDamage;
                    Debug.Log("Персонаж горит из-за проклятого двуручного меча и теряет " + burningDamage + " здоровья");
                }
            }
        }
    }
    public void CoolDown()
    {
        if (timer_locked_out == true)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = timer_cooldown;
                timer_locked_out = false;

                // a delayed action could be called from here
                // once the lock-out period expires
            }
        }




    }

    private void Update()
    {
        FillnestedObjectStarsStars(512, "Gloves");
        if (SceneManager.GetActiveScene().name == "BackPackBattle" && ObjectInBag())
        {
            Burning();
            CoolDown();
            Activation();
        }

        if (SceneManager.GetActiveScene().name == "BackPackShop" || SceneManager.GetActiveScene().name == "BackpackView")
        {
            Rotate();
            SwitchDynamicStatic();
            OnImpulse();
            RotationToStartRotation();

        }
    }

}
