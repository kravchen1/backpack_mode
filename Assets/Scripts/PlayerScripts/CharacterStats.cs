using System;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public float playerHP, playerExp, playerCoins, requiredExp, playerLvl, playerMaxHp, playerMaxStamina;
    public float playerTime = 0f;

    public TextMeshProUGUI hpText, lvlText, coinsText, expText;

    public GameObject hpBar;
    public GameObject expBar;

    private string characterStatsDataFilePath;
    private CharacterStatsData characterStatsData;



    private void Awake()
    {
        LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "characterStatsData.json"));
        if (SceneManager.GetActiveScene().name == "GenerateMap" || SceneManager.GetActiveScene().name == "Cave"
            || SceneManager.GetActiveScene().name == "GenerateMapFortress1" || SceneManager.GetActiveScene().name == "GenerateMapInternumFortress1")
        {
            InitializeObjects();
        }
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
    public void InitializeCharacterStats()
    {
        if (characterStatsData.playerHP == 0)
        {
            playerHP = 31;
            playerExp = 1;
            playerCoins = 50;
            playerLvl = 1;
            playerMaxHp = 1000;
            int x = 500;
            int y = 2;
            requiredExp = (int)(x * Math.Pow(playerLvl, y) - (x * playerLvl)) + 1000;
            playerTime = 0f;
            playerMaxStamina = 5f;
        }
        else
        {
            playerHP = characterStatsData.playerHP;
            playerExp = characterStatsData.playerExp;
            playerCoins = characterStatsData.playerCoins;
            playerMaxHp = characterStatsData.playerMaxHp;
            playerLvl = characterStatsData.playerLvl;
            playerMaxStamina = characterStatsData.playerMaxStamina;
            int x = 500;
            int y = 2;
            requiredExp = (int)(x * Math.Pow(playerLvl, y) - (x * playerLvl)) + 1000;
        }
    }

    public void SaveData()
    {
        characterStatsDataFilePath = Path.Combine(PlayerPrefs.GetString("savePath"), "characterStatsData.json");
        characterStatsData = new CharacterStatsData(playerHP, playerMaxHp, playerExp, playerCoins, requiredExp, playerLvl, playerMaxStamina);

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
        characterStatsData = new CharacterStatsData(playerHP, playerMaxHp, playerExp, playerCoins, requiredExp, playerLvl, playerMaxStamina);
        if (File.Exists(filePath))
        {
            characterStatsData = JsonUtility.FromJson<CharacterStatsData>(File.ReadAllText(filePath));
        }
        return characterStatsData;
    }
    public CharacterStatsData LoadDataEnemy(String jsonData)
    {
        characterStatsData = new CharacterStatsData(playerHP, playerMaxHp, playerExp, playerCoins, requiredExp, playerLvl, playerMaxStamina);
        characterStatsData = JsonUtility.FromJson<CharacterStatsData>(jsonData);
        return characterStatsData;
    }

    private void Update()
    {
        if (hpText != null) hpText.text = playerHP.ToString() + " / " + playerMaxHp.ToString();
        if(lvlText != null) lvlText.text = playerLvl.ToString();
        if (coinsText != null) coinsText.text = playerCoins.ToString();
        if (expText != null) expText.text = playerExp.ToString() + " / " + requiredExp.ToString();
        if(expBar != null) expBar.GetComponent<Image>().fillAmount = playerExp / requiredExp;
        if (hpBar != null) hpBar.GetComponent<Image>().fillAmount = playerHP / playerMaxHp;
    }
}
