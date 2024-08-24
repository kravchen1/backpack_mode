using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EndOfBattle : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;
    [SerializeField] private GameObject okButton;
    [SerializeField] private GameObject newGameButton;
    [SerializeField] private GameObject endOfBattleCanvas;
    [SerializeField] private Slider timeSpeed;
    private PlayerBackpackBattle playerBackpackBattle;
    private PlayerBackpackBattle enemyBackpackBattle;

    private bool awardsReceived = false;
    private void Awake()
    {
        Time.timeScale = 1f;
    }
    void Start()
    {
        playerBackpackBattle = player.GetComponent<PlayerBackpackBattle>();
        enemyBackpackBattle = enemy.GetComponent<PlayerBackpackBattle>();
    }

    public void EndScene()
    {
        if (enemyBackpackBattle.hp <= 0 && !awardsReceived)
        {
            Time.timeScale = 0f;
            timeSpeed.value = 0f;
            timeSpeed.interactable = false;
            if (playerBackpackBattle.characterStats.playerExp + enemyBackpackBattle.enemyExp >= playerBackpackBattle.characterStats.requiredExp)
            {
                playerBackpackBattle.characterStats.playerLvl++;
                playerBackpackBattle.characterStats.lvlText.text = playerBackpackBattle.characterStats.playerLvl.ToString();
                playerBackpackBattle.characterStats.playerExp = playerBackpackBattle.characterStats.playerExp + enemyBackpackBattle.enemyExp - playerBackpackBattle.characterStats.requiredExp;
            }
            else
            {
                playerBackpackBattle.characterStats.playerExp = playerBackpackBattle.characterStats.playerExp + enemyBackpackBattle.enemyExp;
            }
            playerBackpackBattle.characterStats.expText.text = playerBackpackBattle.characterStats.playerExp.ToString() + " / " + playerBackpackBattle.characterStats.requiredExp.ToString();
            playerBackpackBattle.characterStats.playerCoins = playerBackpackBattle.characterStats.playerCoins + enemyBackpackBattle.enemyCoins;
            playerBackpackBattle.characterStats.coinsText.text = playerBackpackBattle.characterStats.playerCoins.ToString();
            playerBackpackBattle.expBar.GetComponent<Image>().fillAmount = (float)((float)playerBackpackBattle.characterStats.playerExp / (float)playerBackpackBattle.characterStats.requiredExp);
            endOfBattleCanvas.SetActive(true);
            okButton.SetActive(true);
            playerBackpackBattle.characterStats.SaveData();

            awardsReceived = true;
        }
        else if (playerBackpackBattle.hp <= 0)
        {
            Time.timeScale = 0f;
            timeSpeed.value = 0f;
            timeSpeed.interactable = false;
            playerBackpackBattle.hp = 0;
            playerBackpackBattle.characterStats.playerHP = 0;
            playerBackpackBattle.characterStats.hpText.text = playerBackpackBattle.characterStats.playerHP.ToString();

            playerBackpackBattle.expBar.GetComponent<Image>().fillAmount = (float)((float)playerBackpackBattle.characterStats.playerExp / (float)playerBackpackBattle.characterStats.requiredExp);
            endOfBattleCanvas.SetActive(true);
            newGameButton.SetActive(true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        EndScene();
    }
}
