using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBackpackBattle : MonoBehaviour
{
    public GameObject backpack;
    public GameObject hpBar;
    public GameObject staminaBar;
    public GameObject expBar;

    public float hp = 74f;
    public float maxHP = 100f;

    public float stamina = 74f;
    public float staminaMax = 100f;
    public float staminaRegenerate = 1f;

    public int enemyExp;
    public int enemyCoins;

    public CharacterStats characterStats;

    void Start()
    {
        InitializeData();
    }

    void InitializeData()
    {
        if (gameObject.name == "Character")
        {
            characterStats = GetComponent<CharacterStats>();
            characterStats.LoadData();
            characterStats.InitializeCharacterStats();
            hp = characterStats.playerHP;
        }
        else
        {
            enemyExp = 100;
            enemyCoins = 10;
        }
    }


    // Update is called once per frame
    void Update()
    {
        hpBar.GetComponent<Image>().fillAmount = hp / maxHP;
        if (gameObject.name == "Character")
        {
            characterStats.playerHP = Convert.ToInt32(hp);
            characterStats.hpText.text = characterStats.playerHP.ToString();
        }
        staminaBar.GetComponent<Image>().fillAmount = stamina / staminaMax;
        if (stamina < staminaMax)
        {
            stamina += staminaRegenerate * Time.deltaTime;
        }

    }
}
