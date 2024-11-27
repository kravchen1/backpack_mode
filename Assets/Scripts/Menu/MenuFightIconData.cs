using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MenuFightIconData
{
    public GameObject icon;
    public int countStack;

    public GameObject sceneGameObjectIcon;

    public MenuFightIconData(GameObject icon, int countStack)
    {
        this.icon = icon;
        this.countStack = countStack;
    }
}
