using UnityEngine;

public class LoadPlayerOnMap : MonoBehaviour
{
    private GameObject playerPrefab;
    private GameObject player;
    public GameObject spawnForFirstStart;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerPrefab = Resources.Load<GameObject>(PlayerPrefs.GetString("characterClass"));
        InstantinatePlayer();
    }


    void InstantinatePlayer()
    {
        var startPlayerPosition = spawnForFirstStart.transform.localPosition;
        //PlayerPrefs.DeleteKey("PostionMapX");
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
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
