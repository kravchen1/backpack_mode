
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndBattleButtonController : MonoBehaviour
{
    public GameObject canvas1, canvas2, canvas3, canvas4;
    public GameObject DescriptionPlace;
    public GameObject DescriptionEnemyPlace;
    public GameObject animationsPlace;

    public void ToogleHide()
   {
        canvas1.SetActive(!canvas1.activeSelf);
        if (PlayerPrefs.GetInt("WinLose") == 1)
        {
            canvas2.SetActive(!canvas2.activeSelf);
        }
        if (PlayerPrefs.GetInt("WinLose") == 2)
        {
            canvas3.SetActive(!canvas3.activeSelf);
        }
        canvas4.SetActive(!canvas4.activeSelf);

        if (canvas1.activeSelf)
        {
            DescriptionPlace.transform.localScale = new Vector3(0, 0, 0);
            DescriptionEnemyPlace.transform.localScale = new Vector3(0, 0, 0);
            animationsPlace.transform.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            DescriptionPlace.transform.localScale = new Vector3(1, 1, 1);
            DescriptionEnemyPlace.transform.localScale = new Vector3(1, 1, 1);
            animationsPlace.transform.localScale = new Vector3(1, 1, 1);
        }

    }

    
}