using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ToogleVisible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log(other.gameObject.layer);
        if (other.gameObject.layer != 11) return;
        //Debug.Log("2");
        if (!other.gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            other.gameObject.GetComponent<NPCAnimationController>().enabled = true;
            other.gameObject.GetComponent<Animator>().enabled = true;
            other.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            other.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            other.gameObject.transform.GetChild(2).gameObject.SetActive(true);
            other.gameObject.transform.GetChild(3).gameObject.SetActive(true);
            other.gameObject.transform.GetChild(4).gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != 11) return;

        if (other.gameObject.transform.GetChild(0).gameObject.activeSelf)
        {
            other.gameObject.GetComponent<NPCAnimationController>().enabled = false;
            other.gameObject.GetComponent<Animator>().enabled = false;
            other.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            other.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            other.gameObject.transform.GetChild(2).gameObject.SetActive(false);
            other.gameObject.transform.GetChild(3).gameObject.SetActive(false);
            other.gameObject.transform.GetChild(4).gameObject.SetActive(false);
        }
    }
}
