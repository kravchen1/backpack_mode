using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPlayerOnMap : MonoBehaviour
{
    private GameObject playerPrefab;
    private GameObject player;
    public GameObject spawnForFirstStart;
    public Vector3 charScale = new Vector3(7.5f, 7.5f, 1);
    public float charSpeed = 100f;
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

        if (!PlayerPrefs.HasKey("PostionMapX"))
        {
            player = Instantiate(playerPrefab, startPlayerPosition, Quaternion.identity, GameObject.FindGameObjectWithTag("Main Canvas").transform);
            player.GetComponent<RectTransform>().localPosition = startPlayerPosition;
        }
        else
        {
            player = Instantiate(playerPrefab, startPlayerPosition, Quaternion.identity, GameObject.FindGameObjectWithTag("Main Canvas").transform);
            player.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
            player.GetComponent<RectTransform>().anchoredPosition = new Vector2(PlayerPrefs.GetFloat("PostionMapX"), PlayerPrefs.GetFloat("PostionMapY"));
        }
        player.transform.localScale = charScale;
        player.GetComponent<Player>().speed = charSpeed;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
