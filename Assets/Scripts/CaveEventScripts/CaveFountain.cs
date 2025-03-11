using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class CaveFountain : EventParent
{
    private GameObject player;
    private Player classPlayer;
    private CharacterStats characterStats;
    public Sprite emptyFountain;
    private bool isPlayerInTrigger = false;

    public AudioSource waterSaction;
    //public GameObject infoText;
    //public bool isFull = true;


    private void Start()
    {
        if (PlayerPrefs.HasKey("isFountainFull"))
        {
            if(PlayerPrefs.GetInt("isFountainFull") == 0)
                ChangeSprite();
        }
    }

    private void OnTriggerEnter2D()
    {
        isPlayerInTrigger = true;
        player = GameObject.FindGameObjectWithTag("Player");
        characterStats = player.GetComponent<CharacterStats>();
        if(isShowPressE)
        {
            if (PlayerPrefs.HasKey("isFountainFull"))
             {
                if (PlayerPrefs.GetInt("isFountainFull") == 1)
                {
                    GetComponent<AudioSource>().Play();
                    SetActivePressE(isShowPressE);
                }
            }
            else
            {
                GetComponent<AudioSource>().Play();
                SetActivePressE(isShowPressE);
                PlayerPrefs.SetInt("isFountainFull", 1);
            }
        }
    }

    public void ActivateFountain()
    {
        waterSaction.Play();
        var randomHeal = UnityEngine.Random.Range(1, 4);
        switch (randomHeal)
        {
            case 1:
                RestoreHP(30);
                Debug.Log("ActiveFountain Restore 30");
                break;
            case 2:
                RestoreHP(40);
                Debug.Log("ActiveFountain Restore 40");
                break;
            case 3:
                RestoreHP(50);
                Debug.Log("ActiveFountain Restore 30");
                break;
        }
        ChangeSprite();
        isShowPressE = false;
        SetActivePressE(isShowPressE);
        PlayerPrefs.SetInt("isFountainFull", 0);
    }
    //public void SetActivePressE(bool active)
    //{
    //    Debug.Log(active);
    //    infoText.SetActive(active);
    //}

    void RestoreHP(int percentHeal)
    {
        var healValue = characterStats.playerMaxHp / 100 * percentHeal;
        if (characterStats.playerHP + healValue <= characterStats.playerMaxHp)
        {
            characterStats.playerHP += healValue;
        }
        else
        {
            characterStats.playerHP = characterStats.playerMaxHp;
        }
        characterStats.SaveData();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        SetActivePressE(false);
    }

    void ChangeSprite()
    {
        transform.parent.GetComponent<SpriteRenderer>().sprite = emptyFountain;
    }

    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && isShowPressE && PlayerPrefs.GetInt("isFountainFull") == 1)
        {
            ActivateFountain();
        }
    }
}

