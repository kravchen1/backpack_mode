using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToogleAnimation : MonoBehaviour
{
    [SerializeField] private GameObject anim;

    private void OnTriggerEnter2D()
    {
        anim.gameObject.SetActive(true);
        gameObject.transform.parent.GetComponent<Enemy>().Move(true);
    }

    private void OnTriggerExit2D()
    {
        gameObject.transform.parent.GetComponent<Enemy>().StopMove();
        anim.gameObject.SetActive(false);
    }
}
