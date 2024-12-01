using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemFireFlask : DescriptionItem
{
    public int countStack = 2;
    public int giveStack = 30;
    public void SetTextBody()
    {
        string text = "drink when you get <u>" + countStack.ToString() + "</u>\r\n give <u>" + giveStack.ToString() + "</u>";
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
