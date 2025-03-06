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
        SceneManager.LoadScene("Main");
    }

    public void ExitFromWorld()
    {
        SaveFromWorld();
        Application.Quit();
    }

    public void ExitCave()
    {
        var doorData = GameObject.FindGameObjectWithTag("DoorEventDistributor");
        doorData.GetComponent<DoorData>().DeleteData();
        PlayerPrefs.SetFloat("PostionMapX", 45f);
        PlayerPrefs.SetFloat("PostionMapY", 383f);
        PlayerPrefs.DeleteKey("isChestClosed");
        PlayerPrefs.DeleteKey("battlePrefabId");
        PlayerPrefs.DeleteKey("isEnemyDied");
        PlayerPrefs.DeleteKey("isEnemyAlive");
        //SceneManager.LoadScene("GenerateMap");
        SceneLoader.Instance.LoadScene("GenerateMap");
    }
    public void BackpackButton()
    {
        SaveFromWorld();
        SceneManager.LoadScene("BackPack");
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