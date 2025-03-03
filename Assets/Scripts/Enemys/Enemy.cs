using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : EventParent
{
    private GameObject player;
    private bool isPlayerInTrigger = false;

    public TextMeshPro lvlText;

    public int lvlEnemy = 1;
    public int idSpawner = 0;

    private void Start()
    {
        lvlText.text = "lvl. " + lvlEnemy.ToString();
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
        Destroy(gameObject.transform.parent.gameObject);
    }
    public void Die()
    {
        Invoke("EndDieAnimation", 1f);
    }

    public void StartBattle()
    {
        PlayerPrefs.SetString("enemyName", gameObject.transform.parent.gameObject.name);
        PlayerPrefs.SetInt("enemyLvl", lvlEnemy);
        PlayerPrefs.SetInt("enemyIdSpawner", idSpawner);


        SceneManager.LoadScene("BackPackBattle");
    }


}