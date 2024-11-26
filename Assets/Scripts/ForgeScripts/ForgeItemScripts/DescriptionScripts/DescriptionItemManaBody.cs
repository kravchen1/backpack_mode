using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemManaBody : DescriptionItem
{
    public int countStarArmorStack;
    public int countStarManaStack;
    public int countManaStack;
    public void SetTextBody()
    {
        string text = "give       <u>" + Armor.ToString() + "</u> + <u>" + countStarArmorStack.ToString() 
            + "</u> * count mana items\r\ngive       <u>" + countManaStack.ToString() + "</u> + <u>" + countStarManaStack.ToString() + "</u> * count mana items";
        textBody.text = text;
    }

    /*
     give       <u>30</u> + <u>5</u> * count mana items
give       <u>1</u> + <u>5</u> * count mana items
    */

    private void Start()
    {
        SetTextBody();
    }

    void Update()
    {
        
    }
}
