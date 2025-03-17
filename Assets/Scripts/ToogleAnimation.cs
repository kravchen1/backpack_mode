using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToogleAnimation : MonoBehaviour
{
    [SerializeField] private GameObject anim;
    
    private void OnTriggerEnter2D()
    {
        Debug.Log(gameObject.name + "On");
        anim.gameObject.SetActive(true);
        //gameObject.transform.parent.GetComponent<CircleCollider2D>().enabled = true;
    }
    private void OnTriggerExit2D()
    {
        Debug.Log(gameObject.name + "Out");
        anim.gameObject.SetActive(false);
        //gameObject.transform.parent.GetComponent<CircleCollider2D>().enabled = false;
    }
}
