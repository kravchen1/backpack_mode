using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AxeCommon2Hand : MeleeWeapon
{
    private void Start()
    {
        timer = timer_cooldown;
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
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

            // do things
            if (Player.stamina - stamina >= 0)
            {
                Player.stamina -= stamina;
                if (Random.Range(0, 100) <= Accuracy)
                {
                    Debug.Log("Топор ёбнул");
                    Enemy.hp -= Random.Range(attackMin, attackMax + 1);
                }
                else
                {
                    Debug.Log("топор miss");
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
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            CoolDown();
            Activation();
        }

        if (SceneManager.GetActiveScene().name == "BackPackShop")
        {
            Rotate();
            SwitchDynamicStatic();
            OnImpulse();
            RotationToStartRotation();
            
        }
    }
    
}
