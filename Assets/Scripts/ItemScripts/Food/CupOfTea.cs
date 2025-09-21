//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using static UnityEngine.Rendering.DebugUI;

//public class CupOfTea : Food
//{
//    public int health = 5;//надо заменить
//    public int regeneration = 2;//надо заменить
//    public override void Activation()
//    {
//        Heal(health);
//        PlayerPrefs.SetInt("CupOfTeaRegeneration", regeneration);
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

//                var descr = CanvasDescription.GetComponent<DescriptionItemCupOfTea>();
//                descr.health = health;
//                descr.regeneration = regeneration;
//                descr.weight = weight;
//                descr.SetTextBody();
//            }
//        }
//    }

//}
