using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeSpeed : MonoBehaviour
{
    public Slider timeSpeed;
    public TextMeshProUGUI TextSpeed;
    public TextMeshProUGUI TextTime;
    private float startTime;
    [HideInInspector] public double nowTime;
    private void Start()
    {
        if (PlayerPrefs.HasKey("NattleSpeedSlider"))
        {
            timeSpeed.value = PlayerPrefs.GetFloat("NattleSpeedSlider");
        }
        else
        {
            timeSpeed.value = 1;
        }
        startTime = Time.time;
    }
    void Update()
    {
        if (timeSpeed.interactable)
        {
            Time.timeScale = timeSpeed.value;
            TextSpeed.text = "x " + Math.Round(timeSpeed.value, 2).ToString();
            PlayerPrefs.SetFloat("NattleSpeedSlider", timeSpeed.value);
        }
        nowTime = Math.Round(Time.time - startTime, 1);
        TextTime.text = nowTime.ToString() + " sec";
    }

    public void PauseSpeed()
    {
        timeSpeed.value = 0;
    }
}
