using UnityEngine;
using UnityEngine.SceneManagement;

public class PointInterestButtonYesNO : Button
{
    private Collider2D pointInterestCollision;
    private Collider2D lastCrossCollider;
    private GameObject player;
    private Map map;
    private Player classPlayer;


    public override void OnMouseUpAsButton()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        classPlayer = player.GetComponent<Player>();
        map = classPlayer.goMap.GetComponent<generateMapScript>();
        pointInterestCollision = classPlayer.hit.collider;
        lastCrossCollider = classPlayer.lastCrossCollider;
        switch (gameObject.name)
        {
            case "Button_Yes":
                LoadPointInterest();
                break;
            case "Button_No":
                ReturnPlayer();
                Time.timeScale = 1f;
                Destroy(transform.parent.gameObject);
                break;
        }
    }

    void ReturnPlayer()
    {
        Input.ResetInputAxes();
        //classPlayer.animator.SetFloat("Move", 0);
        player.GetComponent<RectTransform>().anchoredPosition = lastCrossCollider.GetComponent<RectTransform>().anchoredPosition;
        classPlayer.startMove = true;
    }

    void LoadPointInterest()
    {
        Time.timeScale = 0f;
        var characterStats = player.GetComponent<CharacterStats>();
        characterStats.activeTile = new Tile(pointInterestCollision.gameObject.name.Replace("(Clone)", ""), pointInterestCollision.gameObject.GetComponent<RectTransform>().anchoredPosition);
        if (pointInterestCollision.gameObject.name.Contains("Battle"))
        {
            Time.timeScale = 0f;
            map.startPlayerPosition = player.GetComponent<RectTransform>().anchoredPosition;
            characterStats.playerTime += 2f;
            map.SaveData("Assets/Saves/mapData.json");
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
