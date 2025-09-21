//using System.Collections;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class bag1x1Resistance : Bag
//{
//    public int countResistanceStack = 1;
//    private bool isUse = false;
//    public override void StartActivation()
//    {
//        if (Player != null)
//        {
//            Player.menuFightIconData.AddBuff(countResistanceStack, "IconResistance");
//            //CreateLogMessage("bag1x1Resistance give " + countResistanceStack.ToString(), Player.isPlayer);
//            logManager.CreateLogMessageGive(originalName, "resist", countResistanceStack, Player.isPlayer);
//            isUse = true;
//        }
//    }

//    public override void CoolDownStart()
//    {
//        if (timer_locked_outStart)
//        {
//            timerStart -= Time.deltaTime;

//            if (timerStart <= 0)
//            {
//                timer_locked_outStart = false;
//                //animator.speed = 1f / timer_cooldown;
//                StartActivation();
//                animator.Play("New State");
//            }
//        }
//    }

//    private void Start()
//    {
//        //FillnestedObjectStarsStars(256);
//        if (SceneManager.GetActiveScene().name == "BackPackBattle" && ObjectInBag())
//        {
//            animator.speed = 1f / 0.5f;
//            animator.Play(originalName + "Activation");
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

//                var descr = CanvasDescription.GetComponent<DescriptionItemBag1x1Resistance>();
//                descr.countStackResistance = countResistanceStack;
//                descr.weight = weight;
//                descr.SetTextBody();
//            }
//        }
//    }
//}