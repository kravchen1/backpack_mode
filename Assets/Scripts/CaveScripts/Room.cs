using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector3 Position; // Позиция комнаты
    public Dictionary<string, Room> Doors; // Соседние комнаты через двери
    private Vector3 vector3;

    public Room(Vector3 position)
    {
        Position = position;
        Doors = new Dictionary<string, Room>();
    }

    public void AddDoor(string direction, Room neighbor)
    {
        Doors[direction] = neighbor;
    }
}
