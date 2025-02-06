using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemBlackCat : DescriptionItem
{
    public int bleedingStack = 5;//надо заменить
    public int resistingStack = 2;//надо заменить
    public void SetTextBody()
    {
        string text = "On hit: inflict <u>" + bleedingStack.ToString() + "</u> \r\n\r\nWhen taking damage, gives <u>" + resistingStack.ToString() + "</u>";
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
