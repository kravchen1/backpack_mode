using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemFireHelmet : DescriptionItem
{
    public int countStack = 1;
    //public float coolDown = 2.1f;
    public void SetTextBody()
    {
        string text = "Every <u>" + cooldown.ToString() + "</u> sec: give you <u>" + countStack.ToString() + "</u>";
        textBody.text = text;
    }

    /*
     Every <u>1</u> sec: applies <u>1</u>      on enemy  
    */

    private void Start()
    {
        SetTextBody();
    }

    void Update()
    {
        
    }
}
