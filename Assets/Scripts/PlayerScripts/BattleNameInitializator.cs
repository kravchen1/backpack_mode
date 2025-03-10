using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleNameInitializator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI textName;
    public bool isPlayer = true;
    private void Start()
    {
        if(isPlayer)
        {
            textName.text = PlayerPrefs.GetString("characterClass");
        }
        else
        {
            textName.text = PlayerPrefs.GetString("enemyName").Replace("(Clone)", "");
        }
    }
}
