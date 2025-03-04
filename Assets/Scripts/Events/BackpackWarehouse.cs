using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BackpackWarehouse : EventParent
{
    private GameObject player;
    private bool isPlayerInTrigger = false;

    private void OnTriggerEnter2D()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        isPlayerInTrigger = true;
        if(isShowPressE)
        {
            GetComponent<AudioSource>().Play();
            SetActivePressE(isShowPressE);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerInTrigger = false;
        SetActivePressE(false);
    }

    public void ActivateShop()
    {
        PlayerPrefs.SetFloat("PostionMapX", player.GetComponent<RectTransform>().anchoredPosition.x);
        PlayerPrefs.SetFloat("PostionMapY", player.GetComponent<RectTransform>().anchoredPosition.y);

        //SceneManager.LoadScene("BackPackWareHouse");
        SceneLoader.Instance.LoadScene("BackPackWareHouse");
    }



    


    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && isShowPressE)
        {
            ActivateShop();
        }
    }
}

