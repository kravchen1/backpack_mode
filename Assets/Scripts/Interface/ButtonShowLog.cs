using UnityEngine;
using UnityEngine.UI;

public class ButtonShowLog : MonoBehaviour
{
    public GameObject Log;
    public Image backgroundBlack;
    public void ShowHideLog()
    {
        if (Log.transform.localScale.x == 1)
        {
            Log.transform.localScale = new Vector3(0, 0, 0);
            backgroundBlack.enabled = false;
        }
        else
        {
            Log.transform.localScale = new Vector3(1, 1, 1);
            backgroundBlack.enabled = true;
        }
    }

}
