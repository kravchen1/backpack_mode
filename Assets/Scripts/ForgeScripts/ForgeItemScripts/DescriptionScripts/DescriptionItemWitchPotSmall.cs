using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DescriptionItemWitchPotSmall : DescriptionItem
{
    public int givePoisonStack = 5;//надо заменить
    public void SetTextBody()
    {
        string text = "Every <u>" + cooldown.ToString() + "</u> sec: inflict <u>" + givePoisonStack.ToString() + "</u>";
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
