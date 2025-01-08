using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemVampireBoots : DescriptionItem
{
    public int countVampireStack = 2;
    public int countArmorStack = 15;
    //public float coolDown = 1.1f;
    public void SetTextBody()
    {
        string text = "start battle: give <u>" + countVampireStack.ToString() + " </u>        and <u>" + countArmorStack.ToString() + "</u>";
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
