using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaveEvent : MonoBehaviour
{
    public List<GameObject> battlePrefabs;
    public GameObject fountainPrefab;
    public GameObject chestPrefab;
    public GameObject storePrefab;
    public List<GameObject> caveBossPrefab;

    

    private GameObject newObject;

    private void Start()
    {
        InstantiateCaveEvent();
    }

    void InstantiateCaveEvent()
    {
        var distributor = GameObject.FindGameObjectWithTag("DoorEventDistributor").GetComponent<DoorEventDistributor>();
        var currentDoor = distributor.doorData.DoorDataClass.currentDoorId;
        var door = distributor.doorsList.Where(e => e.GetComponent<Door>().doorId == currentDoor).ToList();
        var eventId = door[0].GetComponent<Door>().eventId;
        if(currentDoor == 0)
        {
            eventId = -1;
        }
        switch (eventId)
        {
            case 0:
                if (!PlayerPrefs.HasKey("battlePrefabId"))
                {
                    var r = UnityEngine.Random.Range(0, battlePrefabs.Count);
                    //r = 0;
                    newObject = Instantiate(battlePrefabs[r], new Vector3(0, 0, -1), Quaternion.identity, gameObject.transform);
                    newObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, -1);
                    newObject.GetComponent<Enemy>().lvlEnemy = PlayerPrefs.GetInt("caveEnemyLvl");
                    newObject.GetComponent<Enemy>().JSONBackpackInitialized();
                    PlayerPrefs.SetInt("battlePrefabId", r);
                    PlayerPrefs.SetInt("isEnemyAlive", 1);
                }
                else
                {
                    if (!PlayerPrefs.HasKey("isEnemyDied"))
                    {
                        newObject = Instantiate(battlePrefabs[PlayerPrefs.GetInt("battlePrefabId")], new Vector3(0, 0, -1), Quaternion.identity, gameObject.transform);
                        newObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, -1);
                        newObject.GetComponent<Enemy>().lvlEnemy = PlayerPrefs.GetInt("caveEnemyLvl");
                        newObject.GetComponent<Enemy>().JSONBackpackInitialized();
                        if (PlayerPrefs.GetInt("isEnemyAlive") == 0)
                        {
                            newObject.GetComponentInChildren<Animator>().Play("Die");
                            newObject.GetComponentInChildren<Enemy>().Die();
                            PlayerPrefs.SetInt("isEnemyDied", 1);
                        }
                    }
                }
                break;
            case 1:
                newObject = Instantiate(chestPrefab, new Vector3(0, 0, -1), Quaternion.identity, gameObject.transform);
                newObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, -1);
                break;
            case 2:
                newObject = Instantiate(fountainPrefab, new Vector3(0, 0, -1), Quaternion.identity, gameObject.transform);
                newObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, -1);
                break;
            case 3:
                newObject = Instantiate(storePrefab, new Vector3(0, 0, -1), Quaternion.identity, gameObject.transform);
                newObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, -1);
                break;
            case 4:
                if (!PlayerPrefs.HasKey("battlePrefabId"))
                {
                    var r = UnityEngine.Random.Range(0, caveBossPrefab.Count);
                    newObject = Instantiate(caveBossPrefab[r], gameObject.transform);
                    //newObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, -1);
                    newObject.GetComponent<Enemy>().lvlEnemy = PlayerPrefs.GetInt("caveEnemyLvl");
                    newObject.GetComponent<Enemy>().JSONBackpackInitialized();
                    PlayerPrefs.SetInt("battlePrefabId", r);
                    PlayerPrefs.SetInt("isEnemyAlive", 1);
                }
                else
                {
                    if (!PlayerPrefs.HasKey("isEnemyDied"))
                    {
                        newObject = Instantiate(caveBossPrefab[PlayerPrefs.GetInt("battlePrefabId")], gameObject.transform);
                        //newObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, -1);
                        newObject.GetComponent<Enemy>().lvlEnemy = PlayerPrefs.GetInt("caveEnemyLvl");
                        newObject.GetComponent<Enemy>().JSONBackpackInitialized();
                        if (PlayerPrefs.GetInt("isEnemyAlive") == 0)
                        {
                            newObject.GetComponentInChildren<Animator>().Play("Die");
                            newObject.GetComponentInChildren<Enemy>().Die();
                            PlayerPrefs.SetInt("isEnemyDied", 1);
                        }
                    }
                }
                break;

        }
       
    }
}
