using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacterStatsData
{
    public float playerHP, playerExp, playerCoins, requiredExp, playerLvl;
    public float playerTime;
    //public string hpText, lvlText, coinsText;


    public CharacterStatsData(float playerHP, float playerExp, float playerCoins, float requiredExp, float playerLvl, float playerTime)
    {
        this.playerHP = playerHP;
        this.playerExp = playerExp;
        this.playerCoins = playerCoins;
        this.requiredExp = requiredExp;
        this.playerLvl = playerLvl;
        this.playerTime = playerTime;
    }
}
