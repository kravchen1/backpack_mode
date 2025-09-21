//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using static UnityEngine.Rendering.DebugUI;

//public class RawMeat : Food
//{
//    public int health = 5;//надо заменить

//    public override void Activation()
//    {
//        Heal(health);
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

//                var descr = CanvasDescription.GetComponent<DescriptionItemRawMeat>();
//                descr.health = health;
//                descr.weight = weight;
//                descr.SetTextBody();
//            }
//        }
//    }

//}
