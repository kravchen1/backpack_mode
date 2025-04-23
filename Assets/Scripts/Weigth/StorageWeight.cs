using System;
using System.Linq;
using UnityEngine;

public class StorageWeight : MonoBehaviour
{
    public CharacterStats characterStats;

    void GetStorageWeight()
    {
        characterStats.storageWeight = (float)Math.Round(gameObject.GetComponentsInChildren<Item>().ToList().Sum(e => e.weight), 2);
    }

    private void Update()
    {
        GetStorageWeight();
    }

}