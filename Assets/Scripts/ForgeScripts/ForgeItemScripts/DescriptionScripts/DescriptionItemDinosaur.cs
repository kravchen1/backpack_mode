using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemDinosaur : DescriptionItem
{
    public int powerStackChance = 5;//надо заменить
    public int powerStack = 2;//надо заменить
    public void SetTextBody()
    {
        string text = "On hit: Gives a <u>" + powerStackChance.ToString() + "</u>% chance to gain <u>" + powerStack.ToString() + "</u> ";
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
