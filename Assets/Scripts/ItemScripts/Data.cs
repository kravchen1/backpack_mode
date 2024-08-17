using System;
using UnityEngine;

[Serializable]
public class Data
{
    public string name;
    public Vector2 position;
    public Quaternion rotation;

    public Data(string name, Vector2 position)
    {
        this.name = name;
        this.position = position;
    }
    public Data(string name, Vector2 position, Quaternion rotation)
    {
        this.name = name;
        this.position = position;
        this.rotation = rotation;
    }
}
