using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemRabbitPaw : DescriptionItem
{
    public int DamageForStack = 5;//���� ��������
    public int giveCritStack = 2;//���� ��������
    public int giveManaStack = 4;//���� ��������
    public void SetTextBody()
    {
        string text = "Every <u>" + cooldown.ToString() + "</u> sec gives you <u>" + giveCritStack.ToString() + "</u>  \r\n\r\nStart battle: \r\nmushroom and Witchcraft        \r\ngives you <u>" + giveManaStack.ToString() + "</u> ";
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
