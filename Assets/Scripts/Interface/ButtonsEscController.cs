using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonsEscController : MonoBehaviour
{
    public List<BattleSpawn> battleSpawns;


    public Vector3 PointForStack;
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
        PlayerPrefs.DeleteKey("battlePrefabJSON");
        //SceneManager.LoadScene("GenerateMap");
        SceneLoader.Instance.LoadScene("GenerateMap");
    }
    public void BackpackButton()
    {
        SaveFromWorld();
        //SceneManager.LoadScene("BackPack");
        //CameraManager.DisableMainCamera();
        //SceneManager.LoadScene("BackPack", LoadSceneMode.Additive);
        SceneLoader.Instance.LoadScene("BackPack");
    }

    public void BackpackButtonWithoutCheck()
    {
        SaveFromWorldWithoutCheck();
        //SceneManager.LoadScene("BackPack");
        SceneLoader.Instance.LoadScene("BackPack");
    }

    private void SaveFromWorld()
    {
        GameObject player;
        RectTransform playerRectTransform;

        player = GameObject.FindGameObjectWithTag("Player");
        playerRectTransform = player.GetComponent<RectTransform>();


        checkCameraPositionAndSavePlayerPosition(player);


        if (battleSpawns != null && battleSpawns.Count > 0)
        {
            string Biom = "1";

            BattlesSpawnerData battlesSpawnerData = new BattlesSpawnerData();
            battlesSpawnerData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "battlesIn" + Biom + ".json"));

            foreach (var battleSpawn in battleSpawns)
            {
                var battleSpawnerData = battlesSpawnerData.battlesSpawnerDataClass.battleData.Where(e => e.id == battleSpawn.id).ToList();
                if (battleSpawnerData.Count > 0)
                {
                    battleSpawnerData[0].position = battleSpawn.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition;
                }
            }

            battlesSpawnerData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "battlesIn" + Biom + ".json"));

        }

        PlayerPrefs.SetString("currentLocation", SceneManager.GetActiveScene().name);

        //Debug.Log(PlayerPrefs.GetFloat("PostionMapX"));
        //Debug.Log(PlayerPrefs.GetFloat("PostionMapY"));

        player.GetComponent<Player>().characterStats.SaveData();

        //Debug.Log("X: " + PlayerPrefs.GetFloat("PostionMapX"));
        //Debug.Log("Y: " + PlayerPrefs.GetFloat("PostionMapY"));


        if (Time.timeScale == 0) Time.timeScale = 1;

    }

    private void SaveFromWorldWithoutCheck()
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


    public void checkCameraPositionAndSavePlayerPosition(GameObject player)
    {
        var camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MoveCamera>();
        var rtPlayer = player.GetComponent<RectTransform>().anchoredPosition;

        if (camera != null)
        {
            if (rtPlayer.x <= camera.minX)
            {
                PlayerPrefs.SetFloat("PostionMapX", camera.minX + 1);
            }
            else if (rtPlayer.x >= camera.maxX)
            {
                PlayerPrefs.SetFloat("PostionMapX", camera.maxX - 1);
            }
            else
            {
                PlayerPrefs.SetFloat("PostionMapX", rtPlayer.x);
            }

            if (rtPlayer.y <= camera.minY)
            {
                PlayerPrefs.SetFloat("PostionMapY", camera.minY + 1);
            }
            else if (rtPlayer.y >= camera.maxY)
            {
                PlayerPrefs.SetFloat("PostionMapY", camera.maxY - 1);
            }
            else
            {
                PlayerPrefs.SetFloat("PostionMapY", rtPlayer.y);
            }
        }
        else
        {
            PlayerPrefs.SetFloat("PostionMapX", rtPlayer.x);
            PlayerPrefs.SetFloat("PostionMapY", rtPlayer.y);
        }
    }


    public void ButtonStack()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        player.GetComponent<RectTransform>().anchoredPosition = PointForStack;
    }

}