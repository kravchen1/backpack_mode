using TMPro;
using UnityEngine;

public class TimeAndDate : MonoBehaviour
{
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;

    private void Start()
    {
        if(PlayerPrefs.HasKey("timeOn"))
        {
            if(PlayerPrefs.GetInt("timeOn") == 1)
            {
                TimeOn();
            }
        }

        if (PlayerPrefs.HasKey("dateOn"))
        {
            if (PlayerPrefs.GetInt("dateOn") == 1)
            {
                dateOn();
            }
        }
    }

    public void TimeOn()
    {
        timeText.gameObject.SetActive(true);
        //PlayerPrefsMigrationManager.Instance.RegisterIntPref("timeOn");
        PlayerPrefs.SetInt("timeOn", 1);
        
    }

    public void dateOn()
    {
        dateText.gameObject.SetActive(true);
       // PlayerPrefsMigrationManager.Instance.RegisterIntPref("dateOn");
        PlayerPrefs.SetInt("dateOn", 1);
    }

    public void TimeOff()
    {
        timeText.gameObject.SetActive(false);
        PlayerPrefs.SetInt("timeOn", 0);
    }

    public void dateOff()
    {
        dateText.gameObject.SetActive(false);
        PlayerPrefs.SetInt("dateOn", 0);
    }

}