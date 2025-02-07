using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemSwampDragon : DescriptionItem
{
    public int poisonStack = 5;//надо заменить
    public int blindnessStack = 2;//надо заменить
    public int fireStack = 3;//надо заменить
    public void SetTextBody()
    {
        string text = "On hit: inflict <u>" + poisonStack.ToString() + "</u>       and <u>" + blindnessStack.ToString() + "</u> \r\n\r\nWhen taking damage, gives <u>" + fireStack.ToString() + "</u>";
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
