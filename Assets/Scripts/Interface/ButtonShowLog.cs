using System.IO;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonShowLog : MonoBehaviour
{
    public GameObject Log;
    public void ShowHideLog()
    {
        if (Log.transform.localScale.x == 1)
        {
            Log.transform.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            Log.transform.localScale = new Vector3(1, 1, 1);
        }
    }

}
