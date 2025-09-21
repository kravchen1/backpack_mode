//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class FuckSword : Weapon
//{
//    private void Start()
//    {
//        timer = timer_cooldown;
//        if (SceneManager.GetActiveScene().name == "BackPackBattle" && ObjectInBag())
//        {

//               animator.speed = 1f / timer_cooldown;
//               animator.enabled = true;
//        }
//    }

//    public override void Activation()
//    {
        
//        if (timer_locked_out == false)
//        {
//            timer_locked_out = true;

//            // do things
//            if (Player != null)
//            {
//                if (Player.stamina - stamina >= 0)
//                {
//                    Player.stamina -= stamina;
//                    if (Random.Range(0, 100) <= accuracy)
//                    {
//                        float armorBefore = Enemy.armor;
//                        Debug.Log("Иди на хуй");
//                        int attack = Random.Range(attackMin, attackMax + 1);
//                        if (Enemy.armor > 0)
//                        {
//                            Enemy.armor -= attack;

//                            if (Enemy.armor < 0)
//                            {
//                                Enemy.hp = Enemy.hp + Enemy.armor - attack;
//                                Debug.Log("Меч ломает " + armorBefore + " брони и режет плоть на " + (Enemy.armor - attack) + " здоровья");
//                            }
//                            else
//                            {
//                                Debug.Log("Меч ломает " + armorBefore + " брони");
//                            }
//                        }
//                        else
//                        {
//                            Enemy.hp -= attack;
//                            Debug.Log("Меч режет плоть на " + attack + " здоровья");
//                        }

//                    }
//                    else
//                    {
//                        Debug.Log("miss");
//                    }
//                }
//            }
            
//        }
//    }

//    public void CoolDown()
//    {
//        if (timer_locked_out == true)
//        {
//            timer -= Time.deltaTime;

//            if (timer <= 0)
//            {
//                timer = timer_cooldown;
//                timer_locked_out = false;

//                // a delayed action could be called from here
//                // once the lock-out period expires
//            }
//        }
//    }

//    private void Update()
//    {
//        if (SceneManager.GetActiveScene().name == "BackPackBattle" && ObjectInBag())
//        {

//                CoolDown();
//                Activation();
//        }

//        if (SceneManager.GetActiveScene().name == "BackPackShop" || SceneManager.GetActiveScene().name == "BackpackView")
//        {
//            Rotate();
//            SwitchDynamicStatic();
//            OnImpulse();
//            RotationToStartRotation();
            
//        }
//    }
    
//}
