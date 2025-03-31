using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToogleAnimation : MonoBehaviour
{
    [SerializeField] private GameObject anim;
    private bool moveAnim = false;
    private void OnTriggerEnter2D()
    {
        //Debug.Log(gameObject.name + "On");
        anim.gameObject.SetActive(true);
        if (!moveAnim)
        {
            gameObject.transform.parent.GetComponent<Enemy>().Move();
            moveAnim = true;
        }
        //gameObject.transform.parent.GetComponent<CircleCollider2D>().enabled = true;
    }
    private void OnTriggerExit2D()
    {
        //Debug.Log(gameObject.name + "Out");
        anim.gameObject.SetActive(false);
        //gameObject.transform.parent.GetComponent<CircleCollider2D>().enabled = false;
    }
}
