using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemVampireBody : DescriptionItem
{
    public int countBleedStack = 2;
    public void SetTextBody()
    {
        //string text = "Every <u>" + cooldown.ToString() + "</u> sec: give count        equal\r\n      on enemy";
        string text = "Every <u>" + Math.Round(cooldown,1).ToString() + "</u> sec: gives      equal      on enemy\r\n\r\nVampire items       :  apply <u>" + countBleedStack.ToString() + "</u>";
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
