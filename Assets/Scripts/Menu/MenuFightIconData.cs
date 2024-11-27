using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MenuFightIconData
{
    public GameObject iconType;
    public int countStack;

    public GameObject sceneGameObjectIcon;

    public MenuFightIconData(GameObject iconType, int countStack)
    {
        this.iconType = iconType;
        this.countStack = countStack;
    }
}
