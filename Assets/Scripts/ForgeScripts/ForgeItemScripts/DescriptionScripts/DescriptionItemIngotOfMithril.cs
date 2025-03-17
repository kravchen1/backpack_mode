using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemIngotOfMithril : DescriptionItem
{
    
    public void SetTextBody()
    {
        string text = "";
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
