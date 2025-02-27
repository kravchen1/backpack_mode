using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemEmeraldStoneLvl1 : DescriptionItem
{
    
    public void SetTextBody()
    {
        string text = "Insert the stone into a special slot in the cave";
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
