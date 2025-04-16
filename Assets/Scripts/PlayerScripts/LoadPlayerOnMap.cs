using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPlayerOnMap : MonoBehaviour
{
    private GameObject playerPrefab;
    private GameObject player;
    public GameObject spawnForFirstStart;
    public Vector3 charScale = new Vector3(7.5f, 7.5f, 1);
    public float charSpeed = 100f;
    public bool flipChar = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerPrefab = Resources.Load<GameObject>($@"MainChars/{PlayerPrefs.GetString("characterClass")}");
        InstantinatePlayer();
    }


    void InstantinatePlayer()
    {
        //if (playerPrefab.name.ToUpper().Contains("ICE"))
            //charScale = new Vector3(1f, 1f, 1);
        var startPlayerPosition = spawnForFirstStart.transform.localPosition;
        if(SceneManager.GetActiveScene().name == "Cave")
            PlayerPrefs.DeleteKey("PostionMapX");
        //PlayerPrefs.DeleteKey("PostionMapX");
        if (!PlayerPrefs.HasKey("PostionMapX"))
        {
            player = Instantiate(playerPrefab, startPlayerPosition, Quaternion.identity, GameObject.FindGameObjectWithTag("Main Canvas").transform);
            player.GetComponent<RectTransform>().localPosition = startPlayerPosition;
        }
        else
        {
            //Debug.Log(PlayerPrefs.GetFloat("PostionMapX"));
            //Debug.Log(PlayerPrefs.GetFloat("PostionMapY"));
            player = Instantiate(playerPrefab, startPlayerPosition, Quaternion.identity, GameObject.FindGameObjectWithTag("Main Canvas").transform);
            player.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            player.GetComponent<RectTransform>().anchoredPosition = new Vector2(PlayerPrefs.GetFloat("PostionMapX"), PlayerPrefs.GetFloat("PostionMapY"));
        }
        player.transform.localScale = charScale;
        player.GetComponent<Player>().speed = charSpeed;
        if(flipChar)
        {
            player.GetComponent<Player>().Flip();
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
