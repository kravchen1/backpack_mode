using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemVampireSword : DescriptionItem
{
    //public int countNeedManaStack = 2;
    //public float coolDown = 1.1f;
    //public void SetTextBody()
    //{
    //    string text = "ToDo";
    //    textBody.text = text;
    //}

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
     Every <u>1</u> sec: applies <u>1</u>      on enemy  
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
