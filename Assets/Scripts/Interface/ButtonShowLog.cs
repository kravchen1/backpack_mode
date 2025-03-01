using UnityEngine;
using UnityEngine.UI;

public class ButtonShowLog : MonoBehaviour
{
    public GameObject Log;
    public GameObject DescriptionPlace;
    public GameObject DescriptionEnemyPlace;
    public Image backgroundBlack;
    public void ShowHideLog()
    {
        if (Log.transform.localScale.x == 1)
        {
            Log.transform.localScale = new Vector3(0, 0, 0);
            DescriptionPlace.transform.localScale = new Vector3(1, 1, 1);
            DescriptionEnemyPlace.transform.localScale = new Vector3(1, 1, 1);
            backgroundBlack.enabled = false;
        }
        else
        {
            Log.transform.localScale = new Vector3(1, 1, 1);
            DescriptionPlace.transform.localScale = new Vector3(0, 0, 0);
            DescriptionEnemyPlace.transform.localScale = new Vector3(0, 0, 0);
            backgroundBlack.enabled = true;
        }
    }

}
