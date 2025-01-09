using UnityEngine;

public class LoadPlayerOnMap : MonoBehaviour
{
    private GameObject playerPrefab;
    private GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerPrefab = Resources.Load<GameObject>(PlayerPrefs.GetString("characterClass"));
        InstantinatePlayer();
    }


    void InstantinatePlayer()
    {
        var startPlayerPosition = new Vector2(0,0);
        player = Instantiate(playerPrefab, startPlayerPosition, Quaternion.identity, GameObject.FindGameObjectWithTag("Main Canvas").transform);
        player.GetComponent<RectTransform>().anchoredPosition = startPlayerPosition;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
