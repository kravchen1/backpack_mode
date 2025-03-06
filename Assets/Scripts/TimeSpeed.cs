using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeSpeed : MonoBehaviour
{
    [SerializeField] private Slider timeSpeed;
    public TextMeshProUGUI TextSpeed;
    public TextMeshProUGUI TextTime;
    private float startTime;
    private void Start()
    {
        startTime = Time.time;
    }
    void Update()
    {
        if (timeSpeed.interactable)
        {
            Time.timeScale = timeSpeed.value;
            TextSpeed.text = "x " + Math.Round(timeSpeed.value, 2).ToString();
        }
        TextTime.text = Math.Round(Time.time - startTime, 1).ToString() + " sec";
    }

    public void PauseSpeed()
    {
        timeSpeed.value = 0;
    }
}
