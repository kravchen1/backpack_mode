using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemSling : DescriptionItem
{
   

    //public int damageMin = 1, damageMax = 2;
    //public int staminaCost = 1;
    //public int accuracyPercent = 95;
    //public float cooldown = 1.1f;

    public void SetTextBody()
    {
        string text = "";
        textBody.text = text;
    }

    public void SetTextStat()
    {
        //string text = "<u>" + damageMin.ToString() + "</u>-<u>" + damageMax.ToString() + "</u>   " + Mathf.Round(((damageMin + damageMax)/2 / cooldown)).ToString() + "/sec\r\n<u>" 
        //    + staminaCost.ToString() + "</u>\r\n<u>" 
        //    + accuracyPercent.ToString() + "%\r\n" + cooldown.ToString() + " sec";
        string text = "<u>" + damageMin.ToString() + "</u>-<u>" + damageMax.ToString() + "</u> (" + Mathf.Round(((damageMin + damageMax) / 2 / cooldown)).ToString() + ")/sec"
            + "\r\n<u>" + chanceCrit.ToString() +
            "</u>%\r\n<u>" + critDamage.ToString() +
            "</u>%\r\n<u>" + staminaCost.ToString() +
            "</u>\r\n<u>" + accuracyPercent.ToString() +
            "</u>%\r\n<u>" + cooldown.ToString() + "</u> sec";

        Stats.text = text;
    }

    /*
     <size=200>On hit: applies <u>1</u>        on enemy                                fire items      activate:  Drop <u>2</u>          from enemy and deal <u>5</u> damage  </size> 
    */

    private void Start()
    {
        //SetTextBody();
        SetTextStat();
    }

    void Update()
    {
        
    }
}
