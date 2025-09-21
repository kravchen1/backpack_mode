//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class Food : Item
//{
//    protected bool timer_locked_out = true;
//    public float timerEat = 3f;

//    public void Heal(int health)
//    {
//        var stats = GameObject.FindGameObjectWithTag("Stat").GetComponent<CharacterStats>();

//        if (stats.playerHP + health <= stats.playerMaxHp)
//            stats.playerHP += health;
//        else
//            stats.playerHP = stats.playerMaxHp;
//    }

//    public void Eat()
//    {
//        if (isEat)
//        {
//            Activation();
//            Destroy(gameObject);
//        }
//    }

//    public override void StartActivation()
//    {
//        animator.speed = 1f / timerEat;
//        animator.Play(originalName + "Activation");
//        Invoke("Eat", timerEat + 0.1f);
//    }

//    public override void EffectPlaceCorrect()
//    {
//        if (gameObject.transform.parent.name == "backpack")
//        {
//            isEat = true;
//            StartActivation();
//        }
//    }
//    public override void EffectPlaceNoCorrect()
//    {
//        isEat = false;
//    }

//    public override void Update()
//    {
//        if (SceneManager.GetActiveScene().name != "GenerateMap" && SceneManager.GetActiveScene().name != "Cave" && SceneManager.GetActiveScene().name != "SceneShowItems")
//        {
//            defaultItemUpdate();
//        }
//    }
//}
