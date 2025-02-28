using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemBottleOfWine : DescriptionItem
{
    public int poison = 5;//надо заменить
    public int critChance = 5;
    public void SetTextBody()
    {
        string text = "In the next battle, you will  receive <u>" + poison.ToString() + "</u>     and <u>" + critChance.ToString() + "</u>\r\nThe effect doesn't stack up";
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
