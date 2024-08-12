using Newtonsoft.Json;
using System;
using UnityEngine;

[Serializable]
public class JsonDataMap
{
    [JsonProperty("tileName")]
    public string tileName { get; set; }
    [JsonProperty("tilePositionX")]
    public string tilePositionX { get; set; }
    [JsonProperty("tilePositionY")]
    public string tilePositionY { get; set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
