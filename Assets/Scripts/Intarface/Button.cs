using System.IO;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    private Color buttonColor;
    //[SerializeField] private GameObject goMap;
    private GameObject player;
    private Map map;

    private void Awake()
    {
        
    }

    public void LoadBackpack()
    {
        //player = GameObject.Find("Player");
        player = GameObject.FindGameObjectWithTag("Player");
        buttonColor = GetComponent<Image>().color;
        map = player.GetComponent<Player>().goMap.GetComponent<generateMapScript>();
        player.GetComponent<Player>().GetComponent<CharacterStats>().SaveData();
        map.startPlayerPosition = player.GetComponent<RectTransform>().anchoredPosition;
        map.SaveData("Assets/Saves/mapData.json");
        SceneManager.LoadScene("BackpackView");
    }

    private void StartBackPack()
    {
        BackpackData backpackData = new BackpackData();
        backpackData.itemData = new ItemData();
        Data data = null; 
        string character = PlayerPrefs.GetString("characterClass");

        int rX = Random.Range(0, 5);
        int rY = Random.Range(0, 5);
        float x = 84.49f;//шаг
        float y = 80.34f;//шаг

        if (character.Contains("Fire"))
        {
            data = new Data("bagStartFire", new Vector3(-260.41583251953127f + (x * rX), -164.7316436767578f + (y * rY), 0f));
        }
        else if(character.Contains("Earth"))
        {
            data = new Data("bagStartEarth", new Vector3(-260.41583251953127f + (x * rX), -164.7316436767578f + (y * rY), 0f));
        }
        backpackData.itemData.items.Add(data);
        backpackData.SaveData("Assets/Saves/backpackData.json");
    }

    virtual public void OnMouseUpAsButton()
    {
        switch (gameObject.name)
        {
            case "BackpackButton":
                LoadBackpack();
                break;
            case "StoreButton":
                player = GameObject.FindGameObjectWithTag("Player");
                Time.timeScale = 0f;
                map = player.GetComponent<Player>().goMap.GetComponent<generateMapScript>();
                map.startPlayerPosition = player.GetComponent<RectTransform>().anchoredPosition;
                player.GetComponent<CharacterStats>().playerTime += 1f;
                map.SaveData("Assets/Saves/mapData.json");
                player.GetComponent<CharacterStats>().SaveData();
                //LoadSceneParameters sceneParameters = new LoadSceneParameters(LoadSceneMode.Single,LocalPhysicsMode.None);
                SceneManager.LoadScene("BackPackShop");
                break;
            case "EndOfBattleButtonOK":
                GameObject.Find("Character").GetComponent<CharacterStats>().SaveData();
                SceneManager.LoadScene("GenerateMap");
                break;
            case "Player_FireStatic":
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetString("characterClass", gameObject.name.Replace("Static",""));
                PlayerPrefs.DeleteKey("mapLevel");
                DeleteAllData();
                StartBackPack();
                SceneManager.LoadScene("GenerateMap");
                break;
            case "Player_EarthStatic":
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetString("characterClass", gameObject.name.Replace("Static", ""));
                PlayerPrefs.DeleteKey("mapLevel");
                DeleteAllData();
                StartBackPack();
                SceneManager.LoadScene("GenerateMap");
                break;
            case "Button_NewGame":
                DeleteAllData();
                SceneManager.LoadScene("Main");
                break;
            case "Button_GoMap":
                GameObject.Find("backpack").GetComponent<BackpackData>().SaveData();
                GameObject.Find("Stats").GetComponent<CharacterStats>().SaveData();
                GameObject.Find("Storage").GetComponent<BackpackData>().SaveData();
                GameObject.Find("Shop").GetComponent<Shop>().SaveData("Assets/Saves/shopData.json");
                SceneManager.LoadScene("GenerateMap");
                break;
            case "Button_LoadGame":
                SceneManager.LoadScene("GenerateMap");
                break;
            case "Button_Home":
                player = GameObject.FindGameObjectWithTag("Player");
                Time.timeScale = 0f;
                map = player.GetComponent<Player>().goMap.GetComponent<generateMapScript>();
                map.startPlayerPosition = player.GetComponent<RectTransform>().anchoredPosition;
                map.SaveData("Assets/Saves/mapData.json");
                SceneManager.LoadScene("Main");
                break;
            case "Button_GoMapFromForge":
                GameObject.FindGameObjectWithTag("Forge").GetComponent<Forge>().SaveData("Assets/Saves/forgeData.json");
                GameObject.FindGameObjectWithTag("Stat").GetComponent<CharacterStats>().SaveData();
                SceneManager.LoadScene("GenerateMap");
                break;
            case "ForgeButton":
                SceneManager.LoadScene("Forge");
                break;


        }
    }
    void DeleteAllData()
    {
        var saveDirectory = "Assets/Saves";
        foreach(var file in  Directory.GetFiles(saveDirectory))
        { 
            File.Delete(file);
        }
    }
    //void OnMouseDown()
    //{
    //    buttonColor = GetComponent<Image>().color;
    //    GetComponent<Image>().color = Color.red;
    //}

    //void OnMouseUp()
    //{
    //    GetComponent<Image>().color = buttonColor;
    //}
}
