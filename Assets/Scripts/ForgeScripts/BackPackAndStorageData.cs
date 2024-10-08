using UnityEngine;

public class BackPackAndStorageData : MonoBehaviour
{
    [HideInInspector] public BackpackData backPackData, storageData;

    private void Awake()
    {
        backPackData = new BackpackData();
        storageData = new BackpackData();
        storageData.LoadData("Assets/Saves/storageData.json");
        backPackData.LoadData("Assets/Saves/backpackData.json");
    }
}
