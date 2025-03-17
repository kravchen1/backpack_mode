using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyGeneral : StaticEnemy
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

        if (!PlayerPrefs.HasKey("isEnemyGeneralDied"))
        {
            if (PlayerPrefs.GetInt("isEnemyGeneralDefeat") == 1)
            {
                GetComponentInChildren<Animator>().Play("Die");
                GetComponentInChildren<Enemy>().Die();
                PlayerPrefs.SetInt("isEnemyGeneralDied", 1);
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }


}