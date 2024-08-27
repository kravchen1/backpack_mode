using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class GlovesBase : Armor
{
    public float acceleration = 0.1f;
    private bool isUse = false;
    private bool usable = false;
    private int currentTick = 0;
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle" && ObjectInBag())
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
            FillnestedObjectStarsStars(512, "Weapon");
            foreach (var go in gameObject.GetComponentsInChildren<Cell>())
            {
                if (go.nestedObject != null)
                {
                    var weaponNested = go.nestedObject.GetComponent<Weapon>();
                    weaponNested.timer_cooldown = weaponNested.timer_cooldown - (weaponNested.timer_cooldown * acceleration);
                    weaponNested.animator.speed = 1f/weaponNested.timer_cooldown;
                    Debug.Log(weaponNested.gameObject.name + " ускорен на " + 1 / acceleration + "%");
                }
            }
           
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
        FillnestedObjectStarsStars(512, "Weapon");
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
