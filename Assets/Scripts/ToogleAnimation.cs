using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToogleAnimation : MonoBehaviour
{
    [SerializeField] private GameObject animation;

    private void OnTriggerEnter2D()
    {
        Debug.Log(gameObject.name + "On");
        animation.gameObject.SetActive(true);
    }
    private void OnTriggerExit2D()
    {
        Debug.Log(gameObject.name + "Out");
        animation.gameObject.SetActive(false);
    }
}
