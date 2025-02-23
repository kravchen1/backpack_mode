using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemBlueCrystal : DescriptionItem
{
    public int manaStackChance = 5;//���� ��������
    public int manaStack = 2;//���� ��������
    public void SetTextBody()
    {
        string text = "��tivating an item with       \r\ngives you a <u>" + manaStackChance.ToString() + "</u>% chance to gain <u>" + manaStack.ToString() + "</u>";
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
