using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticEnemy : Enemy
{
    private GameObject player;
    private bool isPlayerInTrigger = false;

    private string currentSceneName;
    private GameObject map;

    private GameObject canvasBackpackEnemy;
    private GenerateBackpackOnMap generateBackpackOnMap;
    private bool click = false;
    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        lvlText.text = "lvl. " + lvlEnemy.ToString();
        if(currentSceneName == "GenerateMap")
            map = GameObject.FindGameObjectWithTag("GoMap");
        else
            map = GameObject.FindGameObjectWithTag("Cave");
        JSONBackpackInitialized();
    }
    private void OnTriggerEnter2D()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        isPlayerInTrigger = true;
        if (isShowPressE)
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

    private void OnMouseUp()
    {
        if (canvasBackpackEnemy == null)
        {
            canvasBackpackEnemy = GameObject.FindGameObjectWithTag("backpack");
            if (canvasBackpackEnemy != null)
            {
                canvasBackpackEnemy.transform.GetChild(0).gameObject.SetActive(true);
                canvasBackpackEnemy.transform.GetChild(1).gameObject.SetActive(true);
                generateBackpackOnMap = canvasBackpackEnemy.GetComponent<GenerateBackpackOnMap>();
                generateBackpackOnMap.ClearBackpackObjects();
                generateBackpackOnMap.Generate(enemyJSON);
            }
        }
        else
        {
            if (canvasBackpackEnemy != null)
            {
                canvasBackpackEnemy.transform.GetChild(0).gameObject.SetActive(true);
                canvasBackpackEnemy.transform.GetChild(1).gameObject.SetActive(true);
                generateBackpackOnMap.ClearBackpackObjects();
                generateBackpackOnMap.Generate(enemyJSON);
            }
        }
    }


}