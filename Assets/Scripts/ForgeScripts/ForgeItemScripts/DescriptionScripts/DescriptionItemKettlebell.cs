using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemKettlebell : DescriptionItem
{
    public int givePowerStack = 5;//надо заменить
    public void SetTextBody()
    {
        string text = "start battle: Every <u>" + cooldown.ToString() + "</u> seconds gives <u>" + givePowerStack.ToString() + "</u>";
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
