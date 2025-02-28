using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemJam : DescriptionItem
{
    public int avoidance = 5;//надо заменить
    public void SetTextBody()
    {
        string text = "In the next battle, you will  receive <u>" + avoidance.ToString() + "</u>\r\nThe effect doesn't stack up";
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
