using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public int lvlEnemy = 1;
    private void Start()
    {
        
    }

    public void StartBattle()
    {
        PlayerPrefs.SetString("enemyName", gameObject.name);
        SceneManager.LoadScene("BackPackBattle");
    }


}