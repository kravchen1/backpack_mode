using System.Collections.Generic;
using UnityEngine;

public class Door
{
    public string name;
    public Transform transform;
    public List<Door> nextDoors;

    public Door(string name, Transform transform)
    {
        this.name = name;
        this.transform = transform;
        nextDoors = new List<Door>();
    }
}
