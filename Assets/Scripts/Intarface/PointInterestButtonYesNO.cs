using UnityEngine;
using UnityEngine.SceneManagement;

public class PointInterestButtonYesNO : Button
{
    public Collider2D pointInterestCollision;
    private GameObject player;
    private Map map;


    public override void OnMouseUpAsButton()
    {
        switch (gameObject.name)
        {
            case "Button_Yes":
                LoadPointInterest();
                break;
            case "Button_No":
                Time.timeScale = 1f;
                Destroy(transform.parent.gameObject);
                break;
        }
    }

    void LoadPointInterest()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Time.timeScale = 0f;
        map = player.GetComponent<Player>().goMap.GetComponent<generateMapScript>();
        var characterStats = player.GetComponent<CharacterStats>();
        if (pointInterestCollision.gameObject.name.Contains("Battle"))
        {
            Time.timeScale = 0f;
            map.startPlayerPosition = player.GetComponent<RectTransform>().anchoredPosition;
            characterStats.playerTime += 2f;
            map.SaveData();
            characterStats.SaveData();
            //LoadSceneParameters sceneParameters = new LoadSceneParameters(LoadSceneMode.Single,LocalPhysicsMode.None);
            PlayerPrefs.SetString("enemyName", pointInterestCollision.gameObject.name.Replace("(Clone)", ""));
            SceneManager.LoadScene("BackPackBattle");
        }
        if (pointInterestCollision.gameObject.name.Contains("Portal"))
        {
            Time.timeScale = 0f;
            characterStats.playerTime = 0f;
            characterStats.SaveData();
            map.DeleteData("Assets/Saves/mapData.json");
            //LoadSceneParameters sceneParameters = new LoadSceneParameters(LoadSceneMode.Single,LocalPhysicsMode.None);
            SceneManager.LoadScene("GenerateMap");
        }
    }
}
