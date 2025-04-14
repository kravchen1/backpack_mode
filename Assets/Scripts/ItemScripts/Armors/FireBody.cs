using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class FireBody : Armor
{
    public int DamageForStack = 5;
    public int SpendStack = 2;

    public GameObject DebugFireLogCharacter, DebugFireLogEnemy, DebugArmorLogCharacter, DebugArmorLogEnemy;
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            FillnestedObjectStarsStars(256, "Fire");
            animator.speed = 1f / 0.5f;
            animator.Play(originalName + "Activation");
        }
    }


    public override void StartActivation()
    {
        if (Player != null)
        {
            Player.armor = Player.armor + startBattleArmorCount;
            Player.armorMax = Player.armorMax + startBattleArmorCount;
            //if (Player.isPlayer)
            //{
            //    //CreateLogMessage(DebugArmorLogCharacter, "FireBody give " + startBattleArmorCount.ToString());
            //}
            //else
            //{
            //    CreateLogMessage(DebugArmorLogEnemy, "FireBody give " + startBattleArmorCount.ToString());
            //}
            logManager.CreateLogMessageGive(originalName, "armor", startBattleArmorCount, Player.isPlayer);

            CheckNestedObjectActivation("StartBag");
            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
        }
    }
    public override void StarActivation(Item item)
    {
        //Активация звёздочек(предмет огня): тратит 1 эффект горения и наносит врагу 5 урона
        if (Player != null && Enemy != null)
        {
            if (Player.menuFightIconData.icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
            {
                bool b = false;
                foreach (var icon in Player.menuFightIconData.icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains("ICONBURN")))
                {
                    if (icon.countStack >= SpendStack)
                    {
                        b = true;
                    }
                }
                if (b)
                {
                    Player.menuFightIconData.DeleteBuff(SpendStack, "ICONBURN");
                    Player.menuFightIconData.CalculateFireFrostStats();
                    //if (Player.isPlayer)
                    //{
                    //    CreateLogMessage(DebugFireLogCharacter, "FireBody removed " + SpendStack.ToString());
                    //}
                    //else
                    //{
                    //    CreateLogMessage(DebugFireLogEnemy, "FireBody removed " + SpendStack.ToString());
                    //}
                    logManager.CreateLogMessageUse(originalName, "fire", SpendStack, Player.isPlayer);


                    Attack(DamageForStack, false);
                    animator.Play(originalName + "Activation2", 0, 0f);
                }
            }
        }
    }

    protected override void FillStars()
    {
        FillnestedObjectStarsStars(256, "Fire");
    }
    public override IEnumerator ShowDescription()
    {
        yield return new WaitForSecondsRealtime(.1f);
        if (!Exit)
        {
            FillStars();
            ChangeShowStars(true);
            if (canShowDescription)
            {
                DeleteAllDescriptions();
                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);

                var descr = CanvasDescription.GetComponent<DescriptionItemFireBody>();
                descr.SpendStack = SpendStack;
                descr.DamageForStack = DamageForStack;
                descr.weight = weight;
                descr.SetTextBody();
            }
        }
    }

}
