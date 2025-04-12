using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyPodpol : StaticEnemy
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

        if (!PlayerPrefs.HasKey("isEnemyPodpolDied"))
        {
            if (PlayerPrefs.GetInt("isEnemyPodpolDefeat") == 1)
            {
                GetComponentInChildren<Animator>().Play("Die");
                GetComponentInChildren<Enemy>().Die();
                PlayerPrefs.SetInt("isEnemyPodpolDied", 1);
            }
        }
        else
        {
            Destroy(gameObject);
        }

    }


}