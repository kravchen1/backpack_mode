using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : EventParent
{
    private GameObject player;
    private bool isPlayerInTrigger = false;

    public TextMeshPro lvlText;

    public int lvlEnemy = 1;
    public int idSpawner = 0;

    public int startHP = 100;
    public int stepHPForLevel = 25;

    public float startStamina = 5.0f;
    public float stepStaminaForLevel = 0.5f;

    public List<GameObject> dropItems;
    public List<float> probabilityDropItems;
    private GameObject map;
    private void Start()
    {
        lvlText.text = "lvl. " + lvlEnemy.ToString();
        map = GameObject.FindGameObjectWithTag("GoMap");
    }
    private void OnTriggerEnter2D()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        isPlayerInTrigger = true;
        if (isShowPressE)
        {
            GetComponent<AudioSource>().Play();
            SetActivePressE(isShowPressE);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isPlayerInTrigger = false;
        SetActivePressE(false);
    }

    public void ActivateEnemy()
    {
        PlayerPrefs.SetFloat("PostionMapX", player.GetComponent<RectTransform>().anchoredPosition.x);
        PlayerPrefs.SetFloat("PostionMapY", player.GetComponent<RectTransform>().anchoredPosition.y);
        PlayerPrefs.SetString("currentLocation", SceneManager.GetActiveScene().name);

        StartBattle();
    }



    private void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E) && isShowPressE)
        {
            ActivateEnemy();
        }
    }




    private void EndDieAnimation()
    {
        
        if (dropItems.Count > 0 && dropItems.Count == probabilityDropItems.Count)
        {
            for (int i = 0; i < dropItems.Count; i++)
            {
                float r = Random.Range(1.0f, 100.0f);
                if(r <= probabilityDropItems[i])
                {
                    Instantiate(dropItems[i], gameObject.transform.position, Quaternion.identity, map.GetComponent<RectTransform>().transform);
                }
            }
        }
        Destroy(gameObject.transform.parent.gameObject);
    }
    public void Die()
    {
        Invoke("EndDieAnimation", 1f);
    }

    public void StartBattle()
    {
        PlayerPrefs.SetString("enemyName", gameObject.name);
        PlayerPrefs.SetInt("enemyLvl", lvlEnemy);

        PlayerPrefs.SetInt("enemyHP", startHP + ((lvlEnemy - 1) * stepHPForLevel));
        PlayerPrefs.SetFloat("enemyStamina", startStamina + ((lvlEnemy - 1) * stepStaminaForLevel));

        PlayerPrefs.SetInt("enemyIdSpawner", idSpawner);


        PlayerPrefs.SetString("enemyBackpackJSON", "{\"items\":[{\"name\":\"bagCommon2x2\",\"position\":{\"x\":36.83209228515625,\"y\":-122.88605499267578,\"z\":-1.00030517578125},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Crossbow\",\"position\":{\"x\":39.6099853515625,\"y\":-81.04045104980469,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}},{\"name\":\"Dagger\",\"position\":{\"x\":39.6099853515625,\"y\":-161.37620544433595,\"z\":-2.0006103515625},\"rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0}}]}");



        SceneManager.LoadScene("BackPackBattle");
    }


}