using System.Linq;
using TMPro;
using UnityEngine;

public class DescriptionItemVampireBow : DescriptionItem
{
    public int countIncreasesCritDamage = 10;
    public void SetTextBody()
    {
        string text = "On hit: Increases critdamage by amount      on enemy\r\n\r\nweapon items      hit: weapon increases critdamage by then <u>" 
            + countIncreasesCritDamage.ToString() + "</u>% this item critdamage      \r\n";
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


    private void Start()
    {
        SetTextBody();
        SetTextStat();
    }

    void Update()
    {
        
    }
}
