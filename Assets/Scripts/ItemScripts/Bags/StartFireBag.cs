//using System.Collections;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class StartFireBag : Bag
//{
//    public int countBurnStack = 1;
//    public override void StarActivation(Item item)
//    {
//        if (Player != null)
//        {
//            //Player.menuFightIconData.AddBuff(countBurnStack, "IconBurn");
//            logManager.CreateLogMessageGive(originalName, "fire", countBurnStack, Player.isPlayer);
//            //Debug.Log("сумка огня наложила 1 ожёг");
//            //CreateLogMessage("FireBag give " + countBurnStack.ToString() + " burn");
//            Player.menuFightIconData.CalculateFireFrostStats();//true = Player
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

//                var descr = CanvasDescription.GetComponent<DescriptionItemFireBag>();
//                descr.countFireStack = countBurnStack;
//                descr.weight = weight;
//                descr.SetTextBody();
//            }
//        }
//    }
//}