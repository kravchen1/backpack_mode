using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FountainButtonYesNO : Button
{
    private GameObject player;
    private Map map;
    private PlayerOld_ classPlayer;
    private CharacterStats characterStats;

    public override void OnMouseUpAsButton()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        classPlayer = player.GetComponent<PlayerOld_>();
        map = classPlayer.goMap.GetComponent<generateMapScript>();
        characterStats = player.GetComponent<CharacterStats>();
        switch (gameObject.name)
        {
            case "Button_Yes":
                ActivateFountain();
                Destroy(transform.parent.gameObject);
                ChangeTile();
                break;
            case "Button_No":
                Destroy(transform.parent.gameObject);
                break;
        }
        classPlayer.startMove = true;
    }

    void ActivateFountain()
    {
        var randomHeal = Random.Range(1, 4);
        switch(randomHeal)
        {
            case 1:
                RestoreHP(30);
                Debug.Log("ActiveFountain Restore 30");
                break;
            case 2:
                RestoreHP(40);
                Debug.Log("ActiveFountain Restore 40");
                break;
            case 3:
                RestoreHP(50);
                Debug.Log("ActiveFountain Restore 30");
                break;
        }
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
