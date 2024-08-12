using System;
using System.Collections.Generic;
using UnityEngine;
using static generateMapScript;

[Serializable]
public class MapData
{
    [HideInInspector] public List<Tile> tiles = new List<Tile>();
    public MapData(List<Tile> tiles)
    {
        this.tiles = tiles;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
