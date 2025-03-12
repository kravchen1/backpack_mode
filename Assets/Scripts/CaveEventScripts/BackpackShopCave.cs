using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BackpackShopCave : EventParent
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
        PlayerPrefs.SetString("currentLocation", SceneManager.GetActiveScene().name);


        //SceneManager.LoadScene("BackPackShop");
        SceneLoader.Instance.LoadScene("BackPackShopCave1");
    }



    


    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && isShowPressE)
        {
            ActivateShop();
        }
    }
}

