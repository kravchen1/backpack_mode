using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DoorEntrance : EventParent
{
    private GameObject player;
    private bool isPlayerInTrigger = false;

    public float positionMapX = -1f, positionMapY = -1f;
    public string loadScene = "-";
    private bool isCoroutineRunning = false;
    private AudioSource audioSource;
    private AudioSource parentAudioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        parentAudioSource = transform.parent.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        player = collision.gameObject;
        isPlayerInTrigger = true;
        if(isShowPressE)
        {
            audioSource.volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
            audioSource.Play();
            SetActivePressE(isShowPressE);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerInTrigger = false;
        SetActivePressE(false);
    }

    private IEnumerator ActivateEntrance()
    {
        isCoroutineRunning = true;
        while (parentAudioSource.isPlaying)
        {
            yield return null; // Ждем один кадр
        }
        if (positionMapX != -1)
        {
            PlayerPrefs.SetFloat("PostionMapX", positionMapX);
        }
        else
        {
            PlayerPrefs.DeleteKey("PostionMapX");
        }
        if (positionMapY != -1)
        {
            PlayerPrefs.SetFloat("PostionMapY", positionMapY);
        }
        else
        {
            PlayerPrefs.DeleteKey("PostionMapX");
        }
        if (loadScene != "-")
        {
            var playerClass = player.GetComponent<Player>();
            if (gameObject.transform.parent.GetComponent<Door>().isLastDoor == false)
            {
                playerClass.distributor.doorData.DoorDataClass.currentDoorId = gameObject.transform.parent.GetComponent<Door>().doorId;
                playerClass.distributor.doorData.DoorDataClass.currentCaveLevel = gameObject.transform.parent.GetComponent<Door>().caveLevel;
                playerClass.distributor.doorData.SaveData();
            }
            else
            {
                playerClass.distributor.doorData.GetComponent<DoorData>().DeleteData();
                PlayerPrefs.DeleteKey("caveEnemyLvl");
                PlayerPrefs.SetFloat("PostionMapX", 45f);
                PlayerPrefs.SetFloat("PostionMapY", 383f);
                loadScene = "GenerateMap";
            }
            //SceneManager.LoadScene(loadScene);
            PlayerPrefs.DeleteKey("isChestClosed");
            PlayerPrefs.DeleteKey("battlePrefabId");
            PlayerPrefs.DeleteKey("isEnemyDied");
            PlayerPrefs.DeleteKey("isEnemyAlive");
            PlayerPrefs.DeleteKey("isFountainFull");
            SceneLoader.Instance.LoadScene(loadScene);

        }
        else
        {
            Debug.Log("бро, заполни параметр loadScene в префабе");
        }
    }



    


    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && isShowPressE && !isCoroutineRunning)
        {
            //PlayerPrefs.SetInt("isEnemyAlive", 0);
            if (PlayerPrefs.HasKey("isEnemyAlive"))
            {
                if (PlayerPrefs.GetInt("isEnemyAlive") == 0)
                {
                    parentAudioSource.volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
                    parentAudioSource.Play();
                    StartCoroutine(ActivateEntrance());
                }
            }
            else
            {
                parentAudioSource.volume = PlayerPrefs.GetFloat("SoundVolume", 1f);
                parentAudioSource.Play();
                StartCoroutine(ActivateEntrance());
            }
        }
    }
}

