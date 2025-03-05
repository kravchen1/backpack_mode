using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsEscController : MonoBehaviour
{
    
    public void BackToMainMenuFromWorld()
    {
        SaveFromWorld();
        //SceneManager.LoadScene("Main");
        SceneLoader.Instance.LoadScene("Main");
    }

    public void ExitFromWorld()
    {
        SaveFromWorld();
        Application.Quit();
    }

    public void BackpackButton()
    {
        SaveFromWorld();
        //SceneManager.LoadScene("BackPack");
        SceneLoader.Instance.LoadScene("BackPack");
    }

    private void SaveFromWorld()
    {
        GameObject player;
        RectTransform playerRectTransform;

        player = GameObject.FindGameObjectWithTag("Player");
        playerRectTransform = player.GetComponent<RectTransform>();


        PlayerPrefs.SetFloat("PostionMapX", playerRectTransform.anchoredPosition.x);
        PlayerPrefs.SetFloat("PostionMapY", playerRectTransform.anchoredPosition.y);
        PlayerPrefs.SetString("currentLocation", SceneManager.GetActiveScene().name);


        player.GetComponent<Player>().characterStats.SaveData();

    }


}