using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemManaRing : DescriptionItem
{
    public int countNeedManaStack = 2;
    public int countBurnStack = 2;
    public void SetTextBody()
    {
       // string text = "Every <u>" + coolDown.ToString() + "</u> sec: ToDo";
        string text = "Every <u>" + cooldown.ToString() + "</u> sec: spends <u>" + countNeedManaStack.ToString() + "</u>      \r\nand gives <u>" + countBurnStack.ToString() + "</u>\r\n";
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
