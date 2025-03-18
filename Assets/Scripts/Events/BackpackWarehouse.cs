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
            GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume",1f);
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
        var camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MoveCamera>();
        var rtPlayer = player.GetComponent<RectTransform>().anchoredPosition;

        if (rtPlayer.x > camera.maxX)
        {
            PlayerPrefs.SetFloat("PostionMapX", camera.maxX - 1);
        }
        else
        {
            PlayerPrefs.SetFloat("PostionMapX", rtPlayer.x);
        }

        PlayerPrefs.SetFloat("PostionMapY", rtPlayer.y);

        //SceneManager.LoadScene("BackPackWareHouse");
        PlayerPrefs.SetString("currentLocation", SceneManager.GetActiveScene().name);
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

