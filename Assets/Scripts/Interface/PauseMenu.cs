using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    public GameObject pauseCanvas;
    public GameObject doorData;

    public void ExitCave()
    {
        doorData.GetComponent<DoorData>().DeleteData();
        PlayerPrefs.SetFloat("PostionMapX", 45f);
        PlayerPrefs.SetFloat("PostionMapY", 383f);
        //SceneManager.LoadScene("GenerateMap");
        SceneLoader.Instance.LoadScene("GenerateMap");
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            pauseCanvas.SetActive(!pauseCanvas.activeSelf);
        }
    }
}
