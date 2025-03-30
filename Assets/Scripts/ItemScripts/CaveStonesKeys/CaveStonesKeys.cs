using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CaveStonesKeys : Item
{
    protected bool timer_locked_out = true;
    public int stoneLevel;

    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            //CoolDownStart();
        }

        //if (SceneManager.GetActiveScene().name == "BackPackShop")
        else if (SceneManager.GetActiveScene().name != "GenerateMap" && SceneManager.GetActiveScene().name != "Cave" && SceneManager.GetActiveScene().name != "SceneShowItems")
        {
            defaultItemUpdate();
        }
    }
}
