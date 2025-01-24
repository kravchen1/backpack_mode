using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class ManaFlask : Flask
{
    private bool isUse = false;
    public int giveStack = 27;
    //public int countStarManaStack = 3;
   // public int countManaStack = 1;
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
                isUse = true;
                Player.menuFightIconData.AddBuff(giveStack, "IconMana");
                //Debug.Log("FireBody give " + startBattleArmorCount + " armor");
                CreateLogMessage("ManaFlask give " + giveStack.ToString() + " mana");
                CheckNestedObjectActivation("StartBag");
            }
        }
    }

    //public override void StarActivation(Item item)
    //{
    //    //Активация звёздочек(предмет огня): тратит 1 эффект горения и наносит врагу 5 урона
    //    if (Player != null && Enemy != null)
    //    {
    //        if (Player.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
    //        {
    //            bool b = false;
    //            foreach (var icon in Player.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
    //            {
    //                if(icon.countStack >= SpendStack)
    //                {
    //                    //Player.menuFightIconData.DeleteBuff(SpendStack, "ICONBURN");
    //                    b = true;
    //                    //Enemy.hp -= DamageForStack;
    //                    Attack(DamageForStack);
    //                    //Debug.Log("FiryBody сняла" + SpendStack.ToString() + " ожёг и нанесла 5 урона");
    //                    CreateLogMessage("FireBody removed " + SpendStack.ToString() + " burn and apply " + DamageForStack.ToString() + " damage");
    //                    //animator.SetTrigger(originalName + "StarActivation");
    //                    //animator.Play("New State");
    //                    animator.Play(originalName + "Activation2", 0, 0f);
    //                    //animator.StartPlayback
    //                }
    //            }
    //            if(b)
    //            {
    //                Player.menuFightIconData.DeleteBuff(SpendStack, "ICONBURN");
    //                Player.menuFightIconData.CalculateFireFrostStats();//true = Player
    //            }
    //        }
    //    }
    //}

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
                //animator.Play("New State");
            }
        }
    }
    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            //FillnestedObjectStarsStars(256, "RareWeapon");
            CoolDownStart();
        }

        //if (SceneManager.GetActiveScene().name == "BackPackShop")
        else
        {
            defaultItemUpdate();
        }
    }

    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSeconds(.1f);
        if (!Exit)
        {
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
                CanvasDescription.GetComponent<DescriptionItemManaFlask>().giveStack = giveStack;
                CanvasDescription.GetComponent<DescriptionItemManaFlask>().SetTextBody();
            }
        }
    }

}
