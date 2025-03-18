using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemWitchPot : DescriptionItem
{
    public int giveManaStack = 5;//надо заменить
    public int givePoisonStack = 2;//надо заменить
    public int giveRegenerationStack = 2;//надо заменить
    public int spendManaStack = 2;//надо заменить
    public void SetTextBody()
    {
        string text = "Every <u>" + cooldown.ToString() + "</u> sec:inflict <u>" + givePoisonStack.ToString() + "</u>     on enemy\r\nspends <u>" + spendManaStack.ToString() + "</u>     and gives you <u>" + giveRegenerationStack.ToString() + "</u> \r\n\r\n  Start battle: \r\nmushroom        gives you <u>" + giveManaStack.ToString() + "</u>";
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
