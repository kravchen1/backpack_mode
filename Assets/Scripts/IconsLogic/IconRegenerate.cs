using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Xml.Linq;
using TMPro;

public class IconRegenerate : Icon
{
    public override void Activation()
    {
        Debug.Log(gameObject.name);
    }
}