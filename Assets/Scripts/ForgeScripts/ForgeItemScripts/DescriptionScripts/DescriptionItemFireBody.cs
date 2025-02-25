using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemFireBody : DescriptionItem
{
    public int DamageForStack = 5;
    public int SpendStack = 2;
    public void SetTextBody()
    {
        string text = "start battle: give <u>" + Armor.ToString() + "</u>\r\nfire items      : spend <u>" + SpendStack.ToString() + "</u>       and deals <u>" + DamageForStack.ToString() + "</u> damage";
        textBody.text = text;
    }

    /*
     start battle: give <u>20</u>\r\n\r\nfire items      activate:  will spend <u>1</u>       and deal <u>5</u> damage 
    */

    private void Start()
    {
        SetTextBody();
    }

    void Update()
    {
        
    }
}
