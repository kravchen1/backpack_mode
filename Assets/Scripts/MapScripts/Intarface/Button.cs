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
        player = GameObject.Find("Player");
        buttonColor = GetComponent<Image>().color;
        map = player.GetComponent<Player>().goMap.GetComponent<generateMapScript>();
        player.GetComponent<Player>().GetComponent<CharacterStats>().SaveData();
        map.startPlayerPosition = player.GetComponent<RectTransform>().anchoredPosition;
        map.SaveData();
        SceneManager.LoadScene("BackpackView");
    }

    virtual public void OnMouseUpAsButton()
    {
        switch (gameObject.name)
        {
            case "BackpackButton":
                LoadBackpack();
                break;
            case "EndOfBattleButtonOK":
                GameObject.Find("Character").GetComponent<CharacterStats>().SaveData();
                SceneManager.LoadScene("GenerateMap");
                break;
            case "Player_agl":
                PlayerPrefs.SetString("characterClass", gameObject.name);
                PlayerPrefs.DeleteKey("mapLevel");
                DeleteAllData();
                SceneManager.LoadScene("GenerateMap");
                break;
            case "Player_str":
                PlayerPrefs.SetString("characterClass", gameObject.name);
                PlayerPrefs.DeleteKey("mapLevel");
                DeleteAllData();
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
                SceneManager.LoadScene("GenerateMap");
                break;
            case "Button_LoadGame":
                SceneManager.LoadScene("GenerateMap");
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
    void OnMouseDown()
    {
        GetComponent<Image>().color = Color.red;
    }

    void OnMouseUp()
    {
        GetComponent<Image>().color = buttonColor;
    }
}
