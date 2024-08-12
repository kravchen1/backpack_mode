using System;
using UnityEngine;

[Serializable]
public class Tile
{
    public string tileName;
    public Vector2 tilePosition;

    public Tile(string tileName, Vector2 tilePosition)
    {
        this.tileName = tileName;
        this.tilePosition = tilePosition;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
}
