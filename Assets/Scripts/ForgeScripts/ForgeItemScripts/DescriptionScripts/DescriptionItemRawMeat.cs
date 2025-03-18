using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemRawMeat : DescriptionItem
{
    public int health = 5;//���� ��������
    public void SetTextBody()
    {
        string text = "When moved into a backpack, it disappears and restores you <u>" + health.ToString() + "</u>";
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
