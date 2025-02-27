using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacterStatsData
{
    public float playerHP, playerMaxHp, playerMaxStamina;
    public int playerCoins, playerLvl, playerExp, requiredExp;



    public CharacterStatsData(float playerHP, float playerMaxHp, int playerExp, int playerCoins, int requiredExp, int playerLvl, float playerMaxStamina)
    {
        this.playerHP = playerHP;
        this.playerMaxHp = playerMaxHp;
        this.playerExp = playerExp;
        this.playerCoins = playerCoins;
        this.requiredExp = requiredExp;
        this.playerLvl = playerLvl;
        this.playerMaxStamina = playerMaxStamina;
    }
}
