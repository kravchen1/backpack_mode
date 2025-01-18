using UnityEngine;
using TMPro;

public class Icon : MonoBehaviour
{
    public TextMeshPro countText;
    public int countStack = 0;

    public GameObject sceneGameObjectIcon;



    public Icon(int countStack)
    {
        this.countStack = countStack;
    }

    public void FixedUpdate()
    {
        countText.text = countStack.ToString();
    }

    public virtual void Activation()
    {
        //Debug.Log("��������� " + this.name);
    }
}