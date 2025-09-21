//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using static UnityEngine.Rendering.DebugUI;

//public class BowlOfSoup : Food
//{
//    public int health = 5;//надо заменить
//    public int fireBuff = 2;//надо заменить
//    public override void Activation()
//    {
//        Heal(health);
//        PlayerPrefs.SetInt("BowOfSoupFire", fireBuff);
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

//                var descr = CanvasDescription.GetComponent<DescriptionItemBowlOfSoup>();
//                descr.health = health;
//                descr.fireBuff = fireBuff;
//                descr.weight = weight;
//                descr.SetTextBody();
//            }
//        }
//    }

//}
