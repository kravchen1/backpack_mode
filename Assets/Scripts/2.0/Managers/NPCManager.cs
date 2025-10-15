using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public List<GameObject> _NPCPrefabs;
    public Transform _transformSpawnMobs;


    public void Start()
    {
        GenerateNPCBackpack();
    }

    public void GenerateNPCBackpack()
    {
        for (int i = 0; i < _NPCPrefabs.Count; i++)
        {
            if (!PlayerPrefs.HasKey(_NPCPrefabs[i].GetComponent<NPCController>().Config.name + "Die"))//todo
            {
                Instantiate(_NPCPrefabs[i], _transformSpawnMobs);
            }
        }
    }
}