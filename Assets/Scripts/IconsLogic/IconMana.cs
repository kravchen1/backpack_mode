using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Xml.Linq;
using TMPro;

public class IconMana : Icon
{
    public IconMana(int countStack, GameObject gameObject) : base(countStack)
    {

    }
    public override void Activation()
    {
        Debug.Log(gameObject.name);
    }
}