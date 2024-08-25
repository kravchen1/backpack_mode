using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class DecoratedLates : Armor
{
    public float howActivation = 30; //при 30%(можно менять) или ниже произойдёт активация
    public float percentHP = 20; //скок процентов сразу восстановит
    public float percentRegenerate = 5; //скок процентов будет регенерировать
    public float timerRegenerate = 1; //как часто в секундах будет происходить регенерация
    public float maxTimeRegenerate = 4; //скольо раз будет происходить регенерация
    private bool isUse = false;
    private bool usable = false;
    private int currentTick = 0;
    private void Start()
    {
        timer = timerRegenerate;
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            //animator.speed = 1f / timer_cooldown;
            animator.enabled = true;
            Activation();
        }
        
    }
 
    public override void Activation()
    {
        if (!isUse)
        {
            Player.armor = Player.armor + startBattleArmorCount;
            Player.armorMax = Player.armorMax + startBattleArmorCount;
            isUse = true;
        }
    }

    //public void TickHeal()
    //{
    //    if (usable && currentTick <= maxTimeRegenerate)
    //    {
    //        timer -= Time.deltaTime;

    //        if (timer <= 0)
    //        {
    //            timer = timerRegenerate;
    //            Player.hp += f_ToPercent(Player.maxHP, percentHP);
    //            currentTick += 1;
    //            Debug.Log("а фласочка то действует " + currentTick);
    //        // a delayed action could be called from here
    //        // once the lock-out period expires
    //        }
    //    }
    //}
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
           // Activation();
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
