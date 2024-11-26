using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemFireDagger : DescriptionItem
{
    public int hitFireStack;
    public int dropFireStack;
    public int dealDamageDropStack;

    //public int damageMin = 1, damageMax = 2;
    //public int staminaCost = 1;
    //public int accuracyPercent = 95;
    //public float cooldown = 1.1f;

    public void SetTextBody()
    {
        string text = "<size=200>On hit: applies <u>" + hitFireStack .ToString() 
            + "</u>        on enemy                                fire items      activate:  Drop <u>" 
            + dropFireStack .ToString() + "</u>          from enemy and deal <u>" + dealDamageDropStack + "</u> damage  </size>";
        textBody.text = text;
    }

    public void SetTextStat()
    {
        string text = damageMin.ToString() + "-" + damageMax.ToString() + "   " + Mathf.Round(((damageMin + damageMax)/2 / cooldown)).ToString() + "/sec\r\n" + staminaCost.ToString() + "\r\n" + accuracyPercent.ToString() + "%\r\n" + cooldown.ToString() + " sec";
        Stats.text = text;
    }

    /*
     <size=200>On hit: applies <u>1</u>        on enemy                                fire items      activate:  Drop <u>2</u>          from enemy and deal <u>5</u> damage  </size> 
    */

    private void Start()
    {
        SetTextBody();
        SetTextStat();
    }

    void Update()
    {
        
    }
}
