using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Entrance : EventParent
{
    private GameObject player;
    private bool isPlayerInTrigger = false;

    public float positionMapX = -1f, positionMapY = -1f;
    public string loadScene = "-";

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

    public void ActivateEntrance()
    {
        if (positionMapX != -1)
        {
            PlayerPrefs.SetFloat("PostionMapX", positionMapX);
        }
        else
        {
            PlayerPrefs.DeleteKey("PostionMapX");
        }
        if (positionMapY != -1)
        {
            PlayerPrefs.SetFloat("PostionMapY", positionMapY);
        }
        else
        {
            PlayerPrefs.DeleteKey("PostionMapX");
        }
        if (loadScene != "-")
        {
            PlayerPrefs.SetInt("NeedSpawnEnemys", 1);
            //SceneManager.LoadScene(loadScene);
            SceneLoader.Instance.LoadScene(loadScene);
        }
        else
        {
            Debug.Log("бро, заполни параметр loadScene в префабе");
        }
    }



    


    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && isShowPressE)
        {
            ActivateEntrance();
        }
    }
}

