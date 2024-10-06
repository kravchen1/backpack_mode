using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForgeButtonYesNo : Button
{
    private GameObject player;
    private Map map;
    private Player classPlayer;
    private CharacterStats characterStats;

    public GameObject forgeCanvas;
    private GameObject mainCanvas;
    private Forge forge;
    public override void OnMouseUpAsButton()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        classPlayer = player.GetComponent<Player>();
        map = classPlayer.goMap.GetComponent<generateMapScript>();
        characterStats = player.GetComponent<CharacterStats>();
        switch (gameObject.name)
        {
            case "Button_Yes":
                //ActivateFountain();
                Destroy(transform.parent.gameObject);
                SetActiveCanvases();
                ForgeActivate();
                //ChangeTile();

                break;
            case "Button_No":
                Destroy(transform.parent.gameObject);
                break;
        }
        classPlayer.startMove = true;
    }

    void SetActiveCanvases()
    {
        var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var obj in allObjects)
        {
            if (obj.name == "ForgeCanvas")
            {
                forgeCanvas = obj;
                break;
            }
        }
        mainCanvas = GameObject.FindGameObjectWithTag("Main Canvas");
        forgeCanvas.SetActive(mainCanvas.activeSelf);
        mainCanvas.SetActive(!forgeCanvas.activeSelf);
    }


    void ForgeActivate()
    {
        forge = forgeCanvas.transform.GetChild(0).GetComponent<Forge>();
        List<string> tagNames = new List<string>() { "ForgeItem" };
        foreach (var tag in tagNames)
        {
            forge.LoadForgeItems(tag);
            forge.LoadForgeItems(tag);
            forge.LoadForgeItems(tag);
            forge.LoadForgeItems(tag);
        }
        forge.InitializatedForgeItems();

    }

    void ChangeTile()
    {
        for (int i = 0; i < map.tiles.Count(); i++)
        {
            if (map.tiles[i].tileName == classPlayer.activePoint.name.Replace("(Clone)","") && map.tiles[i].tilePosition == classPlayer.activePoint.GetComponent<RectTransform>().anchoredPosition)
            {
                var generateMap = classPlayer.map.GetComponent<generateMapScript>();
                generateMap.generateTile(generateMap.road, map.tiles[i].tilePosition);
                map.tiles.Add(new Tile(generateMap.road.name, map.tiles[i].tilePosition));
                Destroy(classPlayer.activePoint);
                map.tiles.Remove(map.tiles[i]);
                break;
            }
        }
    }
}
