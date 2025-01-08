using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemManaHelmet: DescriptionItem
{
    public int hpDrop = 71;
    public int countArmorStack = 34;
    public int countResistStack = 10;
    public int countSpendManaStack = 2;
    public void SetTextBody()
    {
        string text = "health drop below <u>" + hpDrop.ToString() + "</u>%:\r\nspend <u>" + countSpendManaStack.ToString() + "</u>      give <u>" + countArmorStack.ToString() 
            + "</u>        and <u>" + countResistStack.ToString() + "</u>";
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
