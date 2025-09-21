//using UnityEngine;
//using UnityEngine.UI;

//public class ButtonShowLog : MonoBehaviour
//{
//    public GameObject Log;
//    public GameObject DescriptionPlace;
//    public GameObject DescriptionEnemyPlace;
//    public GameObject animationsPlace;
//    public Image backgroundBlack;

//    private EndOfBattle endOfBattle;

//    public TimeSpeed timeSpeed;

//    private float lastTimeSpeed;

//    public GameObject content;
//    public void ShowHideLog()
//    {
//        if(endOfBattle == null)
//        {
//            endOfBattle = GameObject.FindFirstObjectByType<EndOfBattle>();
//        }
//        if (Log.transform.localScale.x == 1)
//        {
//            Log.transform.localScale = new Vector3(0, 0, 0);
//            DescriptionPlace.transform.localScale = new Vector3(1, 1, 1);
//            DescriptionEnemyPlace.transform.localScale = new Vector3(1, 1, 1);
//            animationsPlace.transform.localScale = new Vector3(1, 1, 1);
//            backgroundBlack.enabled = false;
//            content.SetActive(false);
//            if (!endOfBattle.isEndOfBattle)
//            {
//                timeSpeed.timeSpeed.value = lastTimeSpeed;
//                timeSpeed.timeSpeed.interactable = true;
//            }
//        }
//        else
//        {
//            Log.transform.localScale = new Vector3(1, 1, 1);
//            DescriptionPlace.transform.localScale = new Vector3(0, 0, 0);
//            DescriptionEnemyPlace.transform.localScale = new Vector3(0, 0, 0);
//            animationsPlace.transform.localScale = new Vector3(0, 0, 0);
//            backgroundBlack.enabled = true;
//            content.SetActive(true);
//            if (!endOfBattle.isEndOfBattle)
//            {
//                lastTimeSpeed = timeSpeed.timeSpeed.value;
//                Time.timeScale = 0f;
//                timeSpeed.timeSpeed.value = 0;
//                timeSpeed.timeSpeed.interactable = false;
//            }
//        }
//    }

//}
