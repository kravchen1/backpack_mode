using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public List<GameObject> nextDoors;
    public int doorId;
    [HideInInspector] public int eventId;
    public int caveLevel;
    public bool isLastDoor;
    //public Door(GameObject currentDoor)
    //{
    //    currentDoor = this.currentDoor;
    //    nextDoors = new List<Door>();
    //}
}
