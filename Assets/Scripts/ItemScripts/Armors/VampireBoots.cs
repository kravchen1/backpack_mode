//using System.Collections;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using static UnityEngine.Rendering.DebugUI;

//public class VampireBoots : Armor
//{
//    public int countVampireStack = 2;
//    private void Start()
//    {
//        if (SceneManager.GetActiveScene().name == "BackPackBattle")
//        {
//            FillStars();
//            animator.speed = 1f / 0.5f;
//            animator.Play(originalName + "Activation");
//        }
//    }

//    public override void StartActivation()
//    {
//        if (Player != null)
//        {
//            Player.armor = Player.armor + startBattleArmorCount;
//            Player.armorMax = Player.armorMax + startBattleArmorCount;
//            Player.menuFightIconData.AddBuff(countVampireStack, "IconVampire");
//            //CreateLogMessage("Vampire boots give" + countVampireStack.ToString(), Player.isPlayer);
//            logManager.CreateLogMessageGive(originalName, "vampire", countVampireStack, Player.isPlayer);

//            //if (Player.isPlayer)
//            //{
//            //    CreateLogMessage(LogArmorStackCharacter, "Vampire boots give " + startBattleArmorCount.ToString());
//            //}
//            //else
//            //{
//            //    CreateLogMessage(LogArmorStackEnemy, "Vampire boots give " + startBattleArmorCount.ToString());
//            //}
//            logManager.CreateLogMessageGive(originalName, "armor", startBattleArmorCount, Player.isPlayer);

//            CheckNestedObjectActivation("StartBag");
//            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
//        }
        
//    }
//    public override void ShowDescription()
//    {
//        //yield return new WaitForSecondsRealtime(.1f);
//        if (!Exit)
//        {
//            FillStars();
//            ChangeShowStars(true);
//            if (canShowDescription)
//            {
//                DeleteAllDescriptions();
//                CanvasDescription = Instantiate(Description, placeForDescription.GetComponent<RectTransform>().transform);
//                var descr = CanvasDescription.GetComponent<DescriptionItemVampireBoots>();
//                //descr.cooldown = timer_cooldown;
//                descr.armor = startBattleArmorCount;
//                descr.countVampireStack = countVampireStack;
//                descr.weight = weight;
//                descr.SetTextBody();
//            }
//        }
//    }

//}
