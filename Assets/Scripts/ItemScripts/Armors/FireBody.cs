using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class FireBody : Armor
{
    private bool isUse = false;
    public int DamageForStack = 5;
    public int SpendStack = 1;
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            animator.speed = 1f / 0.5f;
            animator.Play(originalName + "Activation");
        }
        
    }
 

    public override void StartActivation()
    {
        if (!isUse)
        {
            if (Player != null)
            {
                Player.armor = Player.armor + startBattleArmorCount;
                Player.armorMax = Player.armorMax + startBattleArmorCount;
                isUse = true;
                Debug.Log("FireBody give " + startBattleArmorCount + " armor");
                CheckNestedObjectActivation("StartBag");
            }
        }
    }

    public override void StarActivation()
    {
        //Активация звёздочек(предмет огня): тратит 1 эффект горения и наносит врагу 5 урона
        if (Player != null && Enemy != null)
        {
            if (Player.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
            {
                foreach (var icon in Player.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
                {
                    if(icon.countStack >= SpendStack)
                    {
                        Player.menuFightIconData.DeleteBuff(SpendStack, "ICONBURN");
                        Enemy.hp -= DamageForStack;
                        Debug.Log("FiryBody сняла 1 ожёг и нанесла 5 урона");
                    }
                }
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
    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            FillnestedObjectStarsStars(512, "RareWeapon");
            CoolDownStart();
        }

        if (SceneManager.GetActiveScene().name == "BackPackShop")
        {
            defaultItemUpdate();
        }
    }
    
}
