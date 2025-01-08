using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemManaAmulet : DescriptionItem
{
    public int countNeedManaStack = 2;
    //public float coolDown = 1.1f;
    public int countDebuffStack = 2;
    public void SetTextBody()
    {
        //string text = "Every <u>" + coolDown.ToString() + "</u> sec: will spend <u>" + countNeedManaStack .ToString() + "</u>     and applie 1 of";
        string text = "Every <u>" + cooldown.ToString() + "</u> sec: will spend <u>" + countNeedManaStack .ToString() + "</u>     and applie <u>" + countDebuffStack.ToString() + "</u> of";
        textBody.text = text;
    }

    /*
     Every <u>1</u> sec: applies <u>1</u>      on enemy  
    */

    private void Start()
    {
        SetTextBody();
    }

    void Update()
    {
        
    }
}
