using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacterStatsData
{
    public float playerHP, playerExp, playerCoins, requiredExp, playerLvl, playerMaxHp;
    public float playerTime;

    public Tile activeTile;
    //public string hpText, lvlText, coinsText;


    public CharacterStatsData(float playerHP, float playerExp, float playerCoins, float requiredExp, float playerLvl, float playerTime, Tile activeTile, float playerMaxHp)
    {
        this.playerHP = playerHP;
        this.playerExp = playerExp;
        this.playerCoins = playerCoins;
        this.requiredExp = requiredExp;
        this.playerLvl = playerLvl;
        this.playerTime = playerTime;
        this.activeTile = activeTile;
        this.playerMaxHp = playerMaxHp;
    }
}
