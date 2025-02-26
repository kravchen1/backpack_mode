using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacterStatsData
{
    public float playerHP, playerMaxHp, playerExp, playerCoins, requiredExp, playerLvl, playerMaxStamina;



    public CharacterStatsData(float playerHP, float playerMaxHp, float playerExp, float playerCoins, float requiredExp, float playerLvl, float playerMaxStamina)
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
