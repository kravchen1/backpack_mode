using System;
using System.IO;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public float playerHP, playerExp, playerCoins, requiredExp, playerLvl;

    public TextMeshProUGUI hpText, lvlText, coinsText, expText;

    public GameObject hpBar;
    public GameObject expBar;

    private float maxHp = 100;

    private string characterStatsDataFilePath;
    private CharacterStatsData characterStatsData;

    private void Awake()
    {
        LoadData();
        InitializeCharacterStats();
    }
    public void InitializeCharacterStats()
    {
        if (characterStatsData.playerHP == 0)
        {
            Debug.Log("LoadStaticData");
            playerHP = 100;
            playerExp = 0;
            playerCoins = 12;
            playerLvl = 1;
            int x = 500;
            int y = 2;
            requiredExp = (int)(x * Math.Pow(playerLvl, y) - (x * playerLvl)) + 1000;
            hpText.text = playerHP.ToString();
            lvlText.text = playerLvl.ToString();
            coinsText.text = playerCoins.ToString();
            
            expText.text = playerExp.ToString() + " / " + requiredExp.ToString();

            expBar.GetComponent<Image>().fillAmount = playerExp / requiredExp;
            hpBar.GetComponent<Image>().fillAmount = playerHP / maxHp;
        }
        else
        {
            Debug.Log("LoadFromFile");
            playerHP = characterStatsData.playerHP;
            playerExp = characterStatsData.playerExp;
            playerCoins = characterStatsData.playerCoins;
            playerLvl = characterStatsData.playerLvl;
            int x = 500;
            int y = 2;
            requiredExp = (int)(x * Math.Pow(playerLvl, y) - (x * playerLvl)) + 1000;
            //hpText.text = playerHP.ToString();
            lvlText.text = playerLvl.ToString();
            coinsText.text = playerCoins.ToString();
            expText.text = playerExp.ToString() + " / " + requiredExp.ToString();
            //Debug.Log(playerExp.ToString() + " / " + requiredExp.ToString());
            expBar.GetComponent<Image>().fillAmount = playerExp / requiredExp;
            hpBar.GetComponent<Image>().fillAmount = playerHP / maxHp;
        }
    }

    public void SaveData()
    {
        characterStatsDataFilePath = "Assets/Saves/characterStatsData.json";
        characterStatsData = new CharacterStatsData(playerHP, playerExp, playerCoins, requiredExp, playerLvl);

        //var saveData = "[";
        var saveData = JsonUtility.ToJson(characterStatsData);
        //saveData += "]";

        if (File.Exists(characterStatsDataFilePath))
        {
            File.Delete(characterStatsDataFilePath);
        }


        using (FileStream fileStream = new FileStream(characterStatsDataFilePath, FileMode.Create, FileAccess.ReadWrite))
        {
            fileStream.Seek(0, SeekOrigin.End);
            byte[] buffer = Encoding.Default.GetBytes(saveData);
            fileStream.Write(buffer, 0, buffer.Length);
        }
    }
    public CharacterStatsData LoadData()
    {
        characterStatsDataFilePath = "Assets/Saves/characterStatsData.json";
        characterStatsData = new CharacterStatsData(playerHP, playerExp, playerCoins, requiredExp, playerLvl);
        if (File.Exists(characterStatsDataFilePath))
        {
            //foreach (var line in File.ReadLines(mapDataFilePath))
            //{
            //    if (line != "[" && line != "]")
            //        mapData.tiles.Add(JsonUtility.FromJson<Tile>(line.Substring(0, line.Length - 1)));
            //}
            characterStatsData = JsonUtility.FromJson<CharacterStatsData>(File.ReadAllText(characterStatsDataFilePath));
        }
        else
            Debug.LogError("There is no save data!");
        return characterStatsData;
    }
}
