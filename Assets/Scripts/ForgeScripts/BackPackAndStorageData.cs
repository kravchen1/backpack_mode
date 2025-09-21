//using System.IO;
//using UnityEngine;

//public class BackPackAndStorageData : MonoBehaviour
//{
//    [HideInInspector] public BackpackData backPackData, storageData;

//    private void Awake()
//    {
//        backPackData = new BackpackData();
//        storageData = new BackpackData();
//        storageData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "storageData.json"));
//        backPackData.LoadData(Path.Combine(PlayerPrefs.GetString("savePath"), "backpackData.json"));
//    }
//}
