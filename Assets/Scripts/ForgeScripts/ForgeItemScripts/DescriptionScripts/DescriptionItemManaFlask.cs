using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemManaFlask : DescriptionItem
{
    public float giveStack = 30;
    public void SetTextBody()
    {
        string text = "Start battle: drink\r\ngive <u>" + giveStack.ToString() + "</u>";
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
