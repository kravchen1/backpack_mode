using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class CaveEvent : MonoBehaviour
{
    public List<GameObject> battlePrefabs;
    public GameObject fountainPrefab;
    public GameObject chestPrefab;
    public GameObject storePrefab;
    public GameObject caveBossPrefab;

    

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
        Debug.Log(eventId);
        switch (eventId)
        {
            case 0:
                var r = UnityEngine.Random.Range(0, battlePrefabs.Count);
                newObject = Instantiate(battlePrefabs[r], new Vector3(0, 0, -1), Quaternion.identity, gameObject.transform);
                newObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, -1);
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
                newObject = Instantiate(caveBossPrefab, new Vector3(0, 0, -1), Quaternion.identity, gameObject.transform);
                newObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, -1);
                break;

        }
       
    }
}
