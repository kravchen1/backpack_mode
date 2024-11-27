using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Xml.Linq;
using TMPro;

public class IconMana : Icon
{
    public override void Activation()
    {
        Debug.Log(gameObject.name);
    }
}