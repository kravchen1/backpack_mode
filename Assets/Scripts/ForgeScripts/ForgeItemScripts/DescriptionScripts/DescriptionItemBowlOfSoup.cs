using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemBowlOfSoup : DescriptionItem
{
    public int health = 5;//надо заменить
    public int fireBuff = 2;//надо заменить
    public void SetTextBody()
    {
        string text = "When moved into a backpack, it disappears and restores you <u>" + health.ToString() + "</u>\r\nAt the beginning of the next battle will give you <u>" + fireBuff.ToString() + "</u> \r\nThe effect doesn't stack up";
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
