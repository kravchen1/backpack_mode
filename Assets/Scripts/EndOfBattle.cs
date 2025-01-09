using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
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


    private Map map = new Map();


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

            //удаление врага
            //map.LoadData("Assets/Saves/mapData.json");
            //Tile activeTile = new Tile(null, new Vector2(0, 0));
            //for (int i = 0; i < map.mapData.tiles.Count(); i++)
            //{
            //    if (map.mapData.tiles[i].tileName == player.GetComponent<CharacterStats>().activeTile.tileName && map.mapData.tiles[i].tilePosition == player.GetComponent<CharacterStats>().activeTile.tilePosition)
            //    {
            //        map.mapData.tiles[i].tileName = "roadStandart";
            //        break;
            //    }
            //}
            //map.SaveData("Assets/Saves/mapData.json", map.mapData);



            //if (activeTile.tileName != null)
            //{
            //    map.mapData.tiles.Remove(activeTile);
            //    map.SaveData("Assets/Saves/mapData.json", map.mapData);
            //}
            //if (playerBackpackBattle.characterStats.playerExp + enemyBackpackBattle.enemyExp >= playerBackpackBattle.characterStats.requiredExp)
            //{
            //    //playerBackpackBattle.characterStats.playerLvl++;
            //    //playerBackpackBattle.characterStats.lvlText.text = playerBackpackBattle.characterStats.playerLvl.ToString();
            //    //playerBackpackBattle.characterStats.playerExp = playerBackpackBattle.characterStats.playerExp + enemyBackpackBattle.enemyExp - playerBackpackBattle.characterStats.requiredExp;
            //}
            //else
            //{
            //    playerBackpackBattle.characterStats.playerExp = playerBackpackBattle.characterStats.playerExp + enemyBackpackBattle.enemyExp;
            //}
            //playerBackpackBattle.characterStats.expText.text = playerBackpackBattle.characterStats.playerExp.ToString() + " / " + playerBackpackBattle.characterStats.requiredExp.ToString();
            //playerBackpackBattle.characterStats.playerCoins = playerBackpackBattle.characterStats.playerCoins + enemyBackpackBattle.enemyCoins;
            //playerBackpackBattle.characterStats.coinsText.text = playerBackpackBattle.characterStats.playerCoins.ToString();
            //playerBackpackBattle.expBar.GetComponent<Image>().fillAmount = (float)((float)playerBackpackBattle.characterStats.playerExp / (float)playerBackpackBattle.characterStats.requiredExp);
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
            //playerBackpackBattle.characterStats.hpText.text = playerBackpackBattle.characterStats.playerHP.ToString();

            //playerBackpackBattle.expBar.GetComponent<Image>().fillAmount = (float)((float)playerBackpackBattle.characterStats.playerExp / (float)playerBackpackBattle.characterStats.requiredExp);
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
