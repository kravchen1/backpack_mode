using System.Linq;
using UnityEngine;

public class StorageWeight : MonoBehaviour
{
    public CharacterStats characterStats;

    void GetStorageWeight()
    {
        characterStats.storageWeight = gameObject.GetComponentsInChildren<Item>().ToList().Sum(e => e.weight);
    }

    private void Update()
    {
        GetStorageWeight();
    }

}