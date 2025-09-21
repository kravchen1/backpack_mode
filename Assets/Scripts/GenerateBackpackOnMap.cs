//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using UnityEngine;

//public class GenerateBackpackOnMap : GenerateBackpack
//{

//    public GameObject buttonClose;
//    public GameObject background;


//    void Start()
//    {
//        Time.timeScale = 1f;
//        backpackData = GetComponent<BackpackData>();
//        ItemsGenerated = new List<GameObject>();

//        Initialization();
//    }

//    public void Generate(string JSON)
//    {
//        backpackData.LoadDataEnemy(JSON);
//        GenerationBackpack();
//    }

//    public void ClearBackpackObjects()
//    {
//        if (ItemsGenerated.Count > 0)
//        {
//            foreach (var item in ItemsGenerated)
//            {
//                Destroy(item);
//            }

//            ItemsGenerated.Clear();
//        }
//    }

//    public void CloseBackpackEnemy()
//    {
//        ClearBackpackObjects();
//        buttonClose.SetActive(false);
//        background.SetActive(false);
//    }

//}

