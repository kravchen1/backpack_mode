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
        player = Instantiate(playerPrefab, startPlayerPosition, Quaternion.identity, GameObject.FindGameObjectWithTag("Main Canvas").transform);
        player.GetComponent<RectTransform>().localPosition = startPlayerPosition;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
