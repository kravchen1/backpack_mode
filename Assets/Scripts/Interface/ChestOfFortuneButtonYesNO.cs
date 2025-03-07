//using System.Collections.Generic;
//using System.Data.SqlTypes;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class ChestOfFortuneButtonYesNO : Button
//{
//    private GameObject player;
//    private Map map;
//    private PlayerOld_ classPlayer;
//    private CharacterStats characterStats;

//    public GameObject chestCanvas;
//    private GameObject mainCanvas;

//    private Chest chest;
//    public override void OnMouseUpAsButton()
//    {
//        player = GameObject.FindGameObjectWithTag("Player");
//        classPlayer = player.GetComponent<PlayerOld_>();
//        map = classPlayer.goMap.GetComponent<generateMapScript>();
//        characterStats = player.GetComponent<CharacterStats>();
//        switch (gameObject.name)
//        {
//            case "Button_Yes":
//                //ActivateFountain();
//                OpenChest();
//                Destroy(transform.parent.gameObject);
//                ChangeTile();
//                break;
//            case "Button_No":
//                Destroy(transform.parent.gameObject);
//                break;
//        }
//        classPlayer.startMove = true;
//    }

//    void SetActiveCanvases()
//    {
//        var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
//        foreach (var obj in allObjects)
//        {
//            if (obj.name == "ChestCanvas")
//            {
//                chestCanvas = obj;
//                break;
//            }
//        }
//        mainCanvas = GameObject.FindGameObjectWithTag("Main Canvas");
//        chestCanvas.SetActive(mainCanvas.activeSelf);
//        mainCanvas.SetActive(!chestCanvas.activeSelf);
//    }

//    void OpenChest()
//    {
//        SetActiveCanvases();
//        chest = chestCanvas.transform.GetChild(0).GetComponent<Chest>();
//        chest.animator.enabled = true;
//        //TO DO поробовать сделать шаблон для тегов предмтов(Contains(chest_..))
//        List<string> tagNames = new List<string>() { "ChestItem" };//, "Gloves", "bag" };
//        while (chest.chestItems.Count < 25) 
//        {
//            foreach (var tag in tagNames)
//            {
//                chest.LoadChestItems(tag);
//            }
//        }
//        chest.SetWinner();
//        chest.foolItemChestList = true;
//        chest.AddNewItemInStorage();
//    }

//    void ChangeTile()
//    {
//        for (int i = 0; i < map.tiles.Count(); i++)
//        {
//            if (map.tiles[i].tileName == classPlayer.activePoint.name.Replace("(Clone)","") && map.tiles[i].tilePosition == classPlayer.activePoint.GetComponent<RectTransform>().anchoredPosition)
//            {
//                var generateMap = classPlayer.map.GetComponent<generateMapScript>();
//                generateMap.generateTile(generateMap.road, map.tiles[i].tilePosition);
//                map.tiles.Add(new Tile(generateMap.road.name, map.tiles[i].tilePosition));
//                Destroy(classPlayer.activePoint);
//                map.tiles.Remove(map.tiles[i]);
//                break;
//            }
//        }
//    }
//}
