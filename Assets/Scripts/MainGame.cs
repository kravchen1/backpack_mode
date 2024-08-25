using UnityEngine;
using UnityEngine.UI;

public class MainGame : MonoBehaviour
{
    public Text globaltest;
    private void Start()
    {
        if (!PlayerPrefs.HasKey("level"))
        {
            PlayerPrefs.SetInt("level", 0);
        }
       // globaltest.text = PlayerPrefs.GetInt("level").ToString();
    }

}
