using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    private Color buttonColor;
    //[SerializeField] private GameObject goMap;
    private GameObject player;
    //private Map map;
    private QuestData questData;
    private void Awake()
    {
        
    }

    //public void LoadBackpack()
    //{
    //    //player = GameObject.Find("Player");
    //    player = GameObject.FindGameObjectWithTag("Player");
    //    buttonColor = GetComponent<Image>().color;
    //    //map = player.GetComponent<PlayerOld_>().goMap.GetComponent<generateMapScript>();
    //    //player.GetComponent<PlayerOld_>().GetComponent<CharacterStats>().SaveData();
    //    //map.startPlayerPosition = player.GetComponent<RectTransform>().anchoredPosition;
    //    //map.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "mapData.json"));
    //    //SceneManager.LoadScene("BackpackView");
    //    //SceneLoader.Instance.LoadScene("BackpackView");
    //}

    private void StartBackPack()
    {
        BackpackData backpackData = new BackpackData();
        backpackData.itemData = new ItemData();
        Data data = null; 
        string character = PlayerPrefs.GetString("characterClass");

        int rX = UnityEngine.Random.Range(0, 5);
        int rY = UnityEngine.Random.Range(0, 5);
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
        backpackData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "backpackData.json"));
    }

    virtual public void OnMouseUpAsButton()
    {
        switch (gameObject.name)
        {
            case "BackpackButton":
                //LoadBackpack();
                break;
            case "StoreButton":
                player = GameObject.FindGameObjectWithTag("Player");
                Time.timeScale = 0f;
                //map = player.GetComponent<PlayerOld_>().goMap.GetComponent<generateMapScript>();
                //map.startPlayerPosition = player.GetComponent<RectTransform>().anchoredPosition;
                //player.GetComponent<CharacterStats>().playerTime += 1f;
                //map.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "mapData.json"));
                player.GetComponent<CharacterStats>().SaveData();
                //LoadSceneParameters sceneParameters = new LoadSceneParameters(LoadSceneMode.Single,LocalPhysicsMode.None);
                //SceneManager.LoadScene("BackPackShop");
                SceneLoader.Instance.LoadScene("BackPackShop");
                break;
            case "EndOfBattleButtonOK":
                GameObject.Find("Character").GetComponent<CharacterStats>().SaveData();
                //SceneManager.LoadScene("GenerateMap");
                SceneLoader.Instance.LoadScene("GenerateMap");
                break;
            case "Player_FireStatic":
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ChooseCharCamera>().CharacterSelection(gameObject);
                break;
            case "Player_aglStatic":
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ChooseCharCamera>().CharacterSelection(gameObject);
                break;
            case "Player_IceStatic":
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ChooseCharCamera>().CharacterSelection(gameObject);
                break;
            case "Button_GoPlay":
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetString("savePath", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games\\Backpack Seeker's"));
                if (!Directory.Exists(PlayerPrefs.GetString("savePath")))
                {
                    Directory.CreateDirectory(PlayerPrefs.GetString("savePath"));
                }
                PlayerPrefs.SetString("characterClass", gameObject.transform.parent.name.Replace("Static", ""));
                Debug.Log(gameObject.transform.parent.name.Replace("Static", ""));
                //PlayerPrefs.DeleteKey("mapLevel");
                DeleteAllData();
                StartBackPack();


                //SceneManager.LoadScene("GenerateMapInternumFortress1");
                SceneLoader.Instance.LoadScene("GenerateMapInternumFortress1");


                questData = new QuestData();
                questData.questData = new QDataList();

                Quest quest = new Quest("Еhe beginning of time", "talk to the king", -1, 1);

                questData.questData.quests.Add(quest);
                questData.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "questData.json"));

                break;
            case "Player_EarthStatic":
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ChooseCharCamera>().CharacterSelection(gameObject);
                break;
            case "Button_NewGame":
                DeleteAllData();
                //SceneManager.LoadScene("Main");
                SceneLoader.Instance.LoadScene("Main");
                break;
            case "Button_GoMap":
                GameObject.Find("backpack").GetComponent<BackpackData>().SaveData();
                GameObject.Find("Stats").GetComponent<CharacterStats>().SaveData();
                GameObject.Find("Storage").GetComponent<BackpackData>().SaveData();
                GameObject.Find("Shop").GetComponent<Shop>().SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "shopData.json"));

                //SceneManager.LoadScene("GenerateMapFortress1");
                SceneLoader.Instance.LoadScene("GenerateMapFortress1");
                break;
            case "Button_GoMap3":
                GameObject.Find("backpack").GetComponent<BackpackData>().SaveData();
                GameObject.Find("Stats").GetComponent<CharacterStats>().SaveData();
                GameObject.Find("Storage").GetComponent<BackpackData>().SaveData();

                //SceneManager.LoadScene(PlayerPrefs.GetString("currentLocation"));
                SceneLoader.Instance.LoadScene(PlayerPrefs.GetString("currentLocation"));
                break;
            case "Button_GoMapEat":
                GameObject.Find("backpack").GetComponent<BackpackData>().SaveData();
                GameObject.Find("Stats").GetComponent<CharacterStats>().SaveData();
                GameObject.Find("Storage").GetComponent<BackpackData>().SaveData();
                GameObject.Find("Shop").GetComponent<Shop>().SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "shopDataEat.json"));

                //SceneManager.LoadScene("GenerateMapFortress1");
                SceneLoader.Instance.LoadScene("GenerateMapFortress1");
                break;
            case "Button_DevSave":
                GameObject.Find("backpack").GetComponent<BackpackData>().SaveData();
                //GameObject.Find("Stats").GetComponent<CharacterStats>().SaveData();
                //GameObject.Find("Storage").GetComponent<BackpackData>().SaveData();
                //GameObject.Find("Shop").GetComponent<Shop>().SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "shopDataEat.json"));

                //SceneManager.LoadScene("GenerateMapFortress1");
                break;
            case "Button_GoMapFromCave":
                GameObject.Find("backpack").GetComponent<BackpackData>().SaveData();
                GameObject.FindWithTag("CaveStone").GetComponent<BackpackData>().SaveNewData(Path.Combine(PlayerPrefs.GetString("savePath"), "caveStoneData.json"));
                GameObject.Find("Stats").GetComponent<CharacterStats>().SaveData();
                GameObject.Find("Storage").GetComponent<BackpackData>().SaveData();
                //SceneManager.LoadScene("GenerateMap");
                SceneLoader.Instance.LoadScene("GenerateMap");
                break;
            case "Button_LoadGame":
                SceneManager.LoadScene("GenerateMap");
                SceneLoader.Instance.LoadScene("GenerateMap");
                break;
            case "Button_Home":
                //player = GameObject.FindGameObjectWithTag("Player");
                //Time.timeScale = 0f;
                //map = player.GetComponent<PlayerOld_>().goMap.GetComponent<generateMapScript>();
                //map.startPlayerPosition = player.GetComponent<RectTransform>().anchoredPosition;
                //map.SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "mapData.json"));
                //SceneManager.LoadScene("Main");
                SceneLoader.Instance.LoadScene("Main");
                break;
            case "Button_GoMapFromForge":
                GameObject.FindGameObjectWithTag("Forge").GetComponent<Forge>().SaveData(Path.Combine(PlayerPrefs.GetString("savePath"), "forgeData.json"));
                GameObject.FindGameObjectWithTag("Stat").GetComponent<CharacterStats>().SaveData();
                //SceneManager.LoadScene("GenerateMap");
                SceneLoader.Instance.LoadScene("GenerateMap");
                break;
            case "ForgeButton":
                //SceneManager.LoadScene("Forge");
                //SceneLoader.Instance.LoadScene("Forge");
                SceneLoader.Instance.LoadScene("Forge");
                break;


        }
    }
    void DeleteAllData()
    {
        var saveDirectory = PlayerPrefs.GetString("savePath");
        foreach(var file in  Directory.GetFiles(saveDirectory))
        { 
            File.Delete(file);
        }
    }
    

}
