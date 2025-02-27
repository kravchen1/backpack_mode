using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeSpeed : MonoBehaviour
{
    [SerializeField] private Slider timeSpeed;
    public TextMeshProUGUI TextSpeed;
    public TextMeshProUGUI TextTime;
    // Update is called once per frame
    void Update()
    {
        if (timeSpeed.interactable)
        {
            Time.timeScale = timeSpeed.value;
            TextSpeed.text = "x " + Math.Round(timeSpeed.value, 2).ToString();
        }
        TextTime.text = Math.Round(Time.time,1).ToString() + " sec";
    }

    public void PauseSpeed()
    {
        timeSpeed.value = 0;
    }
}
