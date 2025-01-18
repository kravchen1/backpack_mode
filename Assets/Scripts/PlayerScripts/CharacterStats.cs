using System;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public float playerHP, playerExp, playerCoins, requiredExp, playerLvl, playerMaxHp;
    public float playerTime = 0f;

    public TextMeshProUGUI hpText, lvlText, coinsText, expText;
    public GameObject arrowTime;

    public GameObject hpBar;
    public GameObject expBar;

    private string characterStatsDataFilePath;
    private CharacterStatsData characterStatsData;

    public Tile activeTile;


    private void Awake()
    {
        LoadData("Assets/Saves/characterStatsData.json");
        if(SceneManager.GetActiveScene().name == "GenerateMap" || SceneManager.GetActiveScene().name == "GenerateMapTest")
            InitializeObjects();
        InitializeCharacterStats();
    }

    public void InitializeObjects()
    {
        hpBar = GameObject.FindGameObjectWithTag("HPBar");
        expBar = GameObject.FindGameObjectWithTag("ExpBar");
        hpText = GameObject.FindGameObjectWithTag("HPTxt").GetComponent<TextMeshProUGUI>();
        lvlText = GameObject.FindGameObjectWithTag("LvlTxt").GetComponent<TextMeshProUGUI>();
        coinsText = GameObject.FindGameObjectWithTag("CoinTxt").GetComponent<TextMeshProUGUI>();
        expText = GameObject.FindGameObjectWithTag("ExpTxt").GetComponent<TextMeshProUGUI>();
        //arrowTime = GameObject.FindGameObjectWithTag("ArrowTime");
    }
    public void InitializedTime()
    { 
        Vector3 newRotation = new Vector3(0, 0, 90f - (30f * playerTime));
        //arrowTime.transform.rotation = Quaternion.Euler(newRotation);
        //Rotate(newRotation);
    }

    public void InitializeCharacterStats()
    {
        if (characterStatsData.playerHP == 0)
        {
            //Debug.Log("LoadStaticData");
            playerHP = 100;
            playerExp = 0;
            playerCoins = 1000;
            playerLvl = 1;
            playerMaxHp = 100;
            int x = 500;
            int y = 2;
            requiredExp = (int)(x * Math.Pow(playerLvl, y) - (x * playerLvl)) + 1000;
            playerTime = 0f;
        }
        else
        {
            //Debug.Log("LoadFromFile");
            playerHP = characterStatsData.playerHP;
            playerExp = characterStatsData.playerExp;
            playerCoins = characterStatsData.playerCoins;
            playerMaxHp = characterStatsData.playerMaxHp;
            playerLvl = characterStatsData.playerLvl;
            playerTime = characterStatsData.playerTime;
            activeTile = characterStatsData.activeTile;
            int x = 500;
            int y = 2;
            requiredExp = (int)(x * Math.Pow(playerLvl, y) - (x * playerLvl)) + 1000;
        }
        if (SceneManager.GetActiveScene().name == "GenerateMap")
            InitializedTime();
    }

    public void SaveData()
    {
        characterStatsDataFilePath = "Assets/Saves/characterStatsData.json";
        characterStatsData = new CharacterStatsData(playerHP, playerExp, playerCoins, requiredExp, playerLvl, playerTime, activeTile, playerMaxHp);

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
    public CharacterStatsData LoadData(String filePath)
    {
        //characterStatsDataFilePath = "Assets/Saves/characterStatsData.json";
        characterStatsData = new CharacterStatsData(playerHP, playerExp, playerCoins, requiredExp, playerLvl, playerTime, activeTile, playerMaxHp);
        if (File.Exists(filePath))
        {
            //foreach (var line in File.ReadLines(mapDataFilePath))
            //{
            //    if (line != "[" && line != "]")
            //        mapData.tiles.Add(JsonUtility.FromJson<Tile>(line.Substring(0, line.Length - 1)));
            //}
            characterStatsData = JsonUtility.FromJson<CharacterStatsData>(File.ReadAllText(filePath));
        }
        //else
        //    Debug.LogError("There is no save data!");
        return characterStatsData;
    }

    private void Update()
    {
        if (hpText != null) hpText.text = playerHP.ToString();
        if(lvlText != null) lvlText.text = playerLvl.ToString();
        if (coinsText != null) coinsText.text = playerCoins.ToString();
        if (expText != null) expText.text = playerExp.ToString() + " / " + requiredExp.ToString();
        //Debug.Log(playerExp.ToString() + " / " + requiredExp.ToString());
        if(expBar != null) expBar.GetComponent<Image>().fillAmount = playerExp / requiredExp;
        if (hpBar != null) hpBar.GetComponent<Image>().fillAmount = playerHP / playerMaxHp;
    }
}
