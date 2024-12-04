using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemManaGloves : DescriptionItem
{
    public int countSteelManaStack = 2;
    public void SetTextBody()
    {
        //string text = "Every <u>" + coolDown.ToString() + "</u> sec: ToDo";
        string text = "Every <u>" + cooldown.ToString() + "</u> sec: steal <u>" + countSteelManaStack.ToString() + "</u> \r\nfrom enemy";
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
