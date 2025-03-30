using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BackpackShopItem : EventParent
{
    private GameObject player;
    private bool isPlayerInTrigger = false;

    private void OnTriggerEnter2D()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        isPlayerInTrigger = true;
        if(isShowPressE)
        {
            GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume",1f);
            GetComponent<AudioSource>().Play();
            SetActivePressE(isShowPressE);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerInTrigger = false;
        SetActivePressE(false);
    }



    public void ActivateShop()
    {
        PlayerPrefs.SetInt("id2ShopItem", 1);
        if (PlayerPrefs.GetInt("id2ShopEat") == 1)
        {
            var qm = FindFirstObjectByType<QuestManager>();
            if (qm.CompleteQuest(2, false))
            {
                Quest quest = new Quest("talkToTheKing2", "talk to the king", -1, 3);

                PlayerPrefs.SetInt("NPC_King", 2);
                qm.questData.questData.quests.Add(quest);
                qm.questData.SaveData();
            }
        }

        checkCameraPositionAndSavePlayerPosition(player);
        PlayerPrefs.SetString("currentLocation", SceneManager.GetActiveScene().name);

        //SceneManager.LoadScene("BackPackShop");
        SceneLoader.Instance.LoadScene("BackPackShop");
    }



    


    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && isShowPressE)
        {
            ActivateShop();
        }
    }
}

