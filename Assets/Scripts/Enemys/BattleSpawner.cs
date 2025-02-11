using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleSpawner : MonoBehaviour
{
    public List<GameObject> battles;
    public string Biom = "1";
    private void Start()
    {
        if (PlayerPrefs.GetInt("NeedSpawnEnemys") == 1)
        {
            
        }
        else
        {
            if (File.Exists(Path.Combine(PlayerPrefs.GetString("savePath"), "battlesIn" + Biom + ".json")))
            {
               
            }
        }
    }

    //public void StartBattle()
    //{
    //    PlayerPrefs.SetString("enemyName", gameObject.name);
    //    SceneManager.LoadScene("BackPackBattle");
    //}


}