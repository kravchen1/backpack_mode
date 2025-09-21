//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using static UnityEngine.Rendering.DebugUI;

//public class Magnifire : Stuff
//{
//    public int giveBlindnessStack = 5;

//    public override void Activation()
//    {
//        if (!timer_locked_outStart && !timer_locked_out)
//        {
//            timer_locked_out = true;
//            Enemy.menuFightIconData.AddDebuff(giveBlindnessStack, "IconBlind");

//            //CreateLogMessage("Magnifire inflict " + giveBlindnessStack.ToString(), Player.isPlayer);
//            logManager.CreateLogMessageInflict(originalName, "blind", giveBlindnessStack, Player.isPlayer);

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

//                var descr = CanvasDescription.GetComponent<DescriptionItemMagnifire>();
//                descr.cooldown = timer_cooldown;
//                descr.giveBlindnessStack = giveBlindnessStack;
//                descr.weight = weight;
//                descr.SetTextBody();
//            }
//        }
//    }
//}
