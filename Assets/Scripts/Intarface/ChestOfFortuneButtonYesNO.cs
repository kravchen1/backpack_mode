using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChestOfFortuneButtonYesNO : Button
{
    private GameObject player;
    private Map map;
    private Player classPlayer;
    private CharacterStats characterStats;

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
                ChangeTile();
                break;
            case "Button_No":
                Destroy(transform.parent.gameObject);
                break;
        }
        classPlayer.startMove = true;
    }

    void RestoreHP(int percentHeal)
    {
       var healValue = characterStats.playerMaxHp / 100 * percentHeal;
        if(characterStats.playerHP + healValue <= characterStats.playerMaxHp)
        {
            characterStats.playerHP += healValue;
        }
        else
        {
            characterStats.playerHP = characterStats.playerMaxHp;
        }
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
