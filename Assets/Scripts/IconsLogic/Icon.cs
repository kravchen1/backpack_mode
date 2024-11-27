using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Xml.Linq;
using TMPro;

public class Icon : MonoBehaviour
{
    public TextMeshPro countText;
    public int countStack = 0;
    public void FixedUpdate()
    {
        countText.text = countStack.ToString();
    }

    public virtual void Activation()
    {
        //Debug.Log("��������� " + this.name);
    }
}