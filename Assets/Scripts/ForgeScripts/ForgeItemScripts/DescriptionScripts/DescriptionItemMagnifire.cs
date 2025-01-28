using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemMagnifire : DescriptionItem
{
    public int giveBlindnessStack = 5;//надо заменить
    
    public void SetTextBody()
    {
        string text = "Every <u>" + cooldown.ToString() + "</u> sec apply <u>" + giveBlindnessStack.ToString() + "</u>";
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
