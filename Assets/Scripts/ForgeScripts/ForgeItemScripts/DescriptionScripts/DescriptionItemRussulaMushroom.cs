using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemRussulaMushroom : DescriptionItem
{
    public int givePowerStack = 5;//надо заменить
    public int activationForStar = 2;//надо заменить
    public void SetTextBody()
    {
        string text = "Every <u>" + cooldown.ToString() + "</u> sec: give you <u>" + givePowerStack.ToString() + "</u>  \r\n\r\nEvery mushroom       give a <u>" + activationForStar.ToString() + "</u>% faster activation";
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
