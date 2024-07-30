using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class MainScriptMap : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject carePoint;
    private Color startColorCarePoint;
    //private LineRenderer2D lineRenderer;
    void Start()
    {
        startColorCarePoint = carePoint.gameObject.GetComponent<Image>().color;

    }

    // Update is called once per frame
    void Update()
    {
        carePoint.gameObject.GetComponent<Image>().color = Color.Lerp(Color.green, Color.red, Mathf.PingPong(Time.time, 1));
        //Cube.GetComponent<Renderer>().material.color = lerpedColor;
    }


}
