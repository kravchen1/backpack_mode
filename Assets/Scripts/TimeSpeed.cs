using UnityEngine;
using UnityEngine.UI;

public class TimeSpeed : MonoBehaviour
{
    [SerializeField] private Slider timeSpeed;

    // Update is called once per frame
    void Update()
    {
        if(timeSpeed.interactable)
            Time.timeScale = timeSpeed.value;
    }
}
