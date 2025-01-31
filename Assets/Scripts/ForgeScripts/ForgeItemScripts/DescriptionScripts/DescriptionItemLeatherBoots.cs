using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemLeatherBoots : DescriptionItem
{
    public int armor = 5;//надо заменить
    public void SetTextBody()
    {
        string text = "start battle: gives <u>" + armor.ToString() + "</u>";
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
