using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeSpeed : MonoBehaviour
{
    public Slider timeSpeed;
    public TextMeshProUGUI TextSpeed;
    //public TextMeshProUGUI TextTime;
    //private float startTime;
    [HideInInspector] public double nowTime;
    private void Start()
    {
        timeSpeed.value = 1;
    }
    void Update()
    {
        if (timeSpeed.interactable)
        {
            Time.timeScale = timeSpeed.value;
            TextSpeed.text = "x " + Math.Round(timeSpeed.value, 2).ToString();
        }
    }

    public void PauseSpeed()
    {
        timeSpeed.value = 0;
    }
}
