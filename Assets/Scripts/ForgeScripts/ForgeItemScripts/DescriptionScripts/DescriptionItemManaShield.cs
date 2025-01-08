using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemManaShield : DescriptionItem
{
    public int countStartResistanceStack = 5;
    public int countNeedManaStack = 1;
    public int blockDamage = 11;
    public int countStealManaStack = 1;
    public void SetTextBody()
    {
        //string text = "Every <u>" + coolDown.ToString() + "</u> sec: ToDo";
        string text = "Start battle: give <u>" + countStartResistanceStack.ToString() +"</u>\r\nOn attacked: spend <u>" + countNeedManaStack.ToString() 
            + "</u>      and block <u>" + blockDamage.ToString() + "</u> damage\r\n\r\nmana items    activate:  steal <u>" + countStealManaStack.ToString() + "</u>";
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
