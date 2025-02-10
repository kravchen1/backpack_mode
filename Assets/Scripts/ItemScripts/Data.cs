using System;
using UnityEngine;

[Serializable]
public class Data
{
    public string name;
    public Vector3 position;
    public Quaternion rotation;

    public Data(string name, Vector3 position)
    {
        this.name = name;
        this.position = position;
        this.rotation = Quaternion.identity;
    }
    public Data(string name, Vector3 position, Quaternion rotation)
    {
        this.name = name;
        this.position = position;
        this.rotation = rotation;
    }
}
