using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemVampireBoots : DescriptionItem
{
    //public int countNeedManaStack = 2;
    //public float coolDown = 1.1f;
    public void SetTextBody()
    {
        string text = "ToDo";
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
