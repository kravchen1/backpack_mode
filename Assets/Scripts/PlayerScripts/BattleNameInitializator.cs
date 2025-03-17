using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleNameInitializator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI textName;
    public TextMeshProUGUI textLevel;
    public PlayerBackpackBattle playerBackpackBattle;
    private void Start()
    {
        if(playerBackpackBattle.isPlayer)
        {
            textName.text = PlayerPrefs.GetString("characterClass");
            textLevel.text = playerBackpackBattle.characterStats.playerLvl.ToString();
        }
        else
        {
            textName.text = PlayerPrefs.GetString("enemyName").Replace("(Clone)", "");
            textLevel.text = PlayerPrefs.GetInt("enemyLvl").ToString();
        }
        


    }
}
