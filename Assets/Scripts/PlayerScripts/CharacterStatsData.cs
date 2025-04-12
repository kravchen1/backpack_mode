using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CharacterStatsData
{
    public float playerMaxStamina;
    public float storageWeight, maxStorageWeigth;
    public int playerHP, playerMaxHp, playerCoins, playerLvl, playerExp, requiredExp;



    public CharacterStatsData(int playerHP, int playerMaxHp, int playerExp, int playerCoins, int requiredExp, int playerLvl, float playerMaxStamina, float storageWeight, float maxStorageWeigth)
    {
        this.playerHP = playerHP;
        this.playerMaxHp = playerMaxHp;
        this.playerExp = playerExp;
        this.playerCoins = playerCoins;
        this.requiredExp = requiredExp;
        this.playerLvl = playerLvl;
        this.playerMaxStamina = playerMaxStamina;
        this.storageWeight = storageWeight;
        this.maxStorageWeigth = maxStorageWeigth;
    }
}
