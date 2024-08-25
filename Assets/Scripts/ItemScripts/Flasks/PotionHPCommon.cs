using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class PotionHPCommon : Flask
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
            //animator.enabled = true;
        }
    }
    float f_ToPercent(float a, float p)
    {
        return (a / 100 * p); //возвращает значение из процентов, например если передать 200 и 30, то вернёт 60
    }
    public override void Activation()
    {
        if (!isUse)
        {
            if (Player.hp <= f_ToPercent(Player.maxHP, howActivation) )
            {
                isUse = true;
                usable = true;
                Player.hp += f_ToPercent(Player.maxHP, percentHP);
                Debug.Log("выпили фласочку");
                image.color = Color.gray;
            }
        }
    }

    public void TickHeal()
    {
        if (usable && currentTick < maxTimeRegenerate)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = timerRegenerate;
                Player.hp += f_ToPercent(Player.maxHP, percentHP);
                currentTick += 1;
                Debug.Log("а фласочка то действует " + currentTick);
            // a delayed action could be called from here
            // once the lock-out period expires
            }
        }
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            TickHeal();
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
