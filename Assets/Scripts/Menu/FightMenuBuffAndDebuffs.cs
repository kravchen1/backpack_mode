using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.TerrainTools;

public class FightMenuBuffAndDebuffs : MonoBehaviour
{
    //public GameObject playerIconsMenu, enemyIconsMenu;


    public List<GameObject> generateIcons;
    private GameObject[] prefabs;
    public List<Icon> icons = new List<Icon>();

    public GameObject placeGenerationIcons;

    private int rowIconsCount = 4;

    private float firstElementX = -20;
    private float firstElementY = 30;

    private float stepSizeX = 65f;
    private float stepSizeY = -50f;

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
        //placeGenerationIcons = GameObject.FindGameObjectWithTag("PlaceBuffPlayer");
        LoadPrefabs("ICON");
        //foreach (GameObject icon in generateIcons)
        //{
        //    AddBuff(1, icon.name);
        //}
        //foreach (GameObject icon in generateIcons)
        //{
        //    AddBuff(1, icon.name);
        //}

        //DeleteBuff(5, "IconFrost");

        //DeleteBuff(1, "IconBleed");

    }



    public void AddBuff(int count, string buffName)
    {
        if (icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains(buffName.ToUpper())))
        {
            foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains(buffName.ToUpper())))
            {
                icon.countStack += count;
            }

        }
        else
        {
            var sceneGameObjectIcon = Instantiate(Resources.Load<GameObject>("Icons/"+ buffName), new Vector2(0, 0), Quaternion.identity, placeGenerationIcons.transform);
            var icon = sceneGameObjectIcon.GetComponent<Icon>();
            icon.countStack = count;
            icon.sceneGameObjectIcon = sceneGameObjectIcon;
            sceneGameObjectIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(firstElementX + stepSizeX * (icons.Count() % rowIconsCount), firstElementY + stepSizeY * ((int)(icons.Count() / rowIconsCount)));
            icons.Add(icon);
        }
    }
    public void AddDebuff(int count, string debuffName)
    {
        if (icons.Any(e => e.sceneGameObjectIcon.name.ToUpper().Contains(debuffName.ToUpper())))
        {
            foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains(debuffName.ToUpper())))
            {
                icon.countStack += count;
            }

        }
        else
        {
            var sceneGameObjectIcon = Instantiate(Resources.Load<GameObject>("Icons/" + debuffName), new Vector2(0, 0), Quaternion.identity, placeGenerationIcons.transform);
            var icon = sceneGameObjectIcon.GetComponent<Icon>();
            icon.countStack = count;
            sceneGameObjectIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(firstElementX + stepSizeX * icons.Count() % rowIconsCount, firstElementY + stepSizeY * icons.Count() / rowIconsCount);
            icons.Add(icon);
        }
    }

    public void DeleteBuff(int count, string buffName)
    {
        Icon iconToRemove = null;

        foreach (var icon in icons.Where(e => e.sceneGameObjectIcon.name.ToUpper().Contains(buffName.ToUpper())))
        {
            if (icon.countStack - count <= 0)
            {
                icon.countStack = 0;
                iconToRemove = icon;
            }
            else
                icon.countStack -= count;
        }

        foreach (var icon in icons)
        {
            Destroy(icon.sceneGameObjectIcon);
        }

        icons.Remove(iconToRemove);
        List<Icon> oldIcons = new List<Icon>(icons);
        icons.Clear();
        foreach (var icon in oldIcons)
        {
            AddBuff(icon.countStack, icon.sceneGameObjectIcon.name.Replace("(Clone)", ""));
        }
    }

    public void DeleteDebuff(int count, string debuffName)
    {
        Icon iconToRemove = null;
        foreach (var icon in icons)
        {
            if (icon.sceneGameObjectIcon.name.ToUpper().Contains(debuffName.ToUpper()))
            {
                iconToRemove = icon;
            }
            Destroy(icon.sceneGameObjectIcon);
            
        }
        if(iconToRemove != null)
            icons.Remove(iconToRemove);

        var oldIcons = icons;
        icons.Clear();
        foreach (var icon in oldIcons)
        {
            AddBuff(icon.countStack, icon.sceneGameObjectIcon.name.Replace("(Clone)", ""));
        }

    }
    //public void addBleed(int count)
    //{
    //    foreach (var icon in Icons.Where(e => e.iconType.name.Contains("Bleed")))
    //    {
    //        icon.countStack++;
    //    }
    //}
    //public void addMana(int count)
    //{
    //    foreach (var icon in Icons.Where(e => e.iconType.name.Contains("Mana")))
    //    {
    //        icon.countStack++;
    //    }
    //}

    //создаём объект с позицией на сцене. Меняем позицию в зависимости от количества объектов в массиве?


    //public void ShowIcons()
    //{
    //    foreach(var icon in IconsPlayer)
    //    {
    //        if(icon.countStack > 0)
    //        {
    //            if(icon.sceneGameObjectIcon == null)
    //                icon.sceneGameObjectIcon = Instantiate(icon.iconType,);
    //        }
    //    }
    //}

    private void FixedUpdate()
    {
        //ShowIcons();
    }

}