using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class RealDoorCreation: MonoBehaviour
{
    private Vector3 step;
    private Vector3 position;

    public GameObject doorPrefab;
    List<GameObject> GetNextDoorData()
    {
        var doorEventDistributor = GameObject.FindGameObjectWithTag("DoorEventDistributor").GetComponent<DoorEventDistributor>();
        var currentDoor = doorEventDistributor.doorData.DoorDataClass.currentDoorId;
        var door = doorEventDistributor.doorsList.Where(e => e.GetComponent<Door>().doorId == currentDoor).ToList();
        return door[0].GetComponent<Door>().nextDoors;
    }

    void InstantiateDoor()
    {
        var nextDoors = GetNextDoorData();

        switch (nextDoors.Count)
        {
            case 1:
                position = new Vector3(60f, -20f, 0f);
                break;
            case 2:
                position = new Vector3(-100f, -20f, 0f);
                step = new Vector3(320f, 0f, 0f);
                break;
            case 3:
                position = new Vector3(-100f, -20f, 0f);
                step = new Vector3(160f, 0f, 0f);
                break;
        }

        foreach (var door in nextDoors)
        {
            var newRealDoor = Instantiate(doorPrefab, position, Quaternion.identity, gameObject.transform);
            newRealDoor.GetComponent<RectTransform>().anchoredPosition = position;
            newRealDoor.AddComponent<Door>();
            newRealDoor.GetComponent<Door>().doorId = door.GetComponent<Door>().doorId;
            newRealDoor.GetComponent<Door>().nextDoors = door.GetComponent<Door>().nextDoors;
            position += step;
        }
    }

    private void Start()
    {
        InstantiateDoor();
    }
}

