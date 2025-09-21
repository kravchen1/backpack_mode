//using System.Collections;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using static UnityEngine.Rendering.DebugUI;

//public class ManaFlask : Flask
//{
//    public int giveStack = 27;
    
//    public override void StartActivation()
//    {
//        if (Player != null)
//        {
//            Player.menuFightIconData.AddBuff(giveStack, "IconMana");
//            //CreateLogMessage("Mana flask give " + giveStack.ToString(), Player.isPlayer);
//            logManager.CreateLogMessageGive(originalName, "mana", giveStack, Player.isPlayer);
//            CheckNestedObjectActivation("StartBag");
//            CheckNestedObjectStarActivation(gameObject.GetComponent<Item>());
//            animator.Play(originalName + "Activation", 0, 0f);
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

//                var descr = CanvasDescription.GetComponent<DescriptionItemManaFlask>();
//                descr.giveStack = giveStack;
//                descr.weight = weight;
//                descr.SetTextBody();
//            }
//        }
//    }

//}
