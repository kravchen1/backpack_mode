using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemNibbler : DescriptionItem
{
    public int armorStack = 5;//надо заменить
    public void SetTextBody()
    {
        string text = "On hit: gives <u>" + armorStack.ToString() + "</u> ";
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
