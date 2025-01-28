using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemOysterMushroom : DescriptionItem
{
    public int stamina = 5;//надо заменить
    public int activationForStar = 2;//надо заменить
    public void SetTextBody()
    {
        string text = "Every <u>" + cooldown.ToString() + "</u> sec: gives you <u>" + stamina.ToString() + "</u>\r\n\r\nEvery mushroom       gives a <u>" + activationForStar.ToString() + "</u>% faster activation";
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
