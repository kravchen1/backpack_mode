using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticEnemy : Enemy
{
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
}