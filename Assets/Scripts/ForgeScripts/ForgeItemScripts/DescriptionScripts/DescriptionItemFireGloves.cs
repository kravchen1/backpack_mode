using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemFireGloves : DescriptionItem
{
    public int countStack = 1;
    public float coolDown = 50f;
    public void SetTextBody()
    {
        string text = "fire items      activate:  will spend <u>" + countStack.ToString() + "</u>       and reduces cooldown by <u>" + coolDown.ToString() + "</u>%.";
        textBody.text = text;
    }

    /*
     fire items      activate:  will spend <u>1</u>       and reduces cooldown by <u>50</u>%.  
    */

    private void Start()
    {
        SetTextBody();
    }

    void Update()
    {
        
    }
}
