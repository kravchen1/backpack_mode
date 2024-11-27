using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FightMenuBuffAndDebuffs : MonoBehaviour
{
    public GameObject playerIconsMenu, enemyIconsMenu;


    public List<GameObject> generateIcons;
    private GameObject[] prefabs;
    private List<MenuFightIconData> IconsPlayer, IconsEnemy;

    private GameObject placeGenerationIconsPlayer, placeGenerationIconsEnemy;

    public void LoadPrefabs(string tagName)//а можно сделать функцией?
    {
        if (prefabs == null)
        {
            prefabs = Resources.LoadAll<GameObject>("");
        }
        generateIcons.AddRange(prefabs.Where(e => e.tag.ToUpper() == tagName).ToList());
    }


    private void Start()
    {
        LoadPrefabs("ICON");
        foreach (GameObject icon in generateIcons)
        {
            IconsPlayer.Add(new MenuFightIconData(icon, 0));
            IconsEnemy.Add(new MenuFightIconData(icon, 0));
        }

        placeGenerationIconsPlayer = GameObject.FindGameObjectWithTag("PlaceBuffPlayer");
        placeGenerationIconsEnemy = GameObject.FindGameObjectWithTag("PlaceBuffEnemy");

    }


    
    public void addPoison(int count, bool Enemy)
    {
        if (Enemy)
        {
            foreach (var icon in IconsEnemy.Where(e => e.iconType.name.Contains("Poson")))
            {
                icon.countStack++;
            }
        }
    }
    public void addBleed(int count)
    {
        foreach (var icon in Icons.Where(e => e.iconType.name.Contains("Bleed")))
        {
            icon.countStack++;
        }
    }
    public void addMana(int count)
    {
        foreach (var icon in Icons.Where(e => e.iconType.name.Contains("Mana")))
        {
            icon.countStack++;
        }
    }

    //создаём объект с позицией на сцене. Меняем позицию в зависимости от количества объектов в массиве?


    public void ShowIcons()
    {
        foreach(var icon in IconsPlayer)
        {
            if(icon.countStack > 0)
            {
                if(icon.sceneGameObjectIcon == null)
                    icon.sceneGameObjectIcon = Instantiate(icon.iconType,);
            }
        }
    }

    private void FixedUpdate()
    {
        ShowIcons();
    }

}