using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticEnemy : Enemy
{

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


    public override void Move()
    {
    }
}