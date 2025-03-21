using System;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.Windows;


public class DescriptionItemHiddenDagger : DescriptionItem
{
    public int bleeding;//надо заменить

    //public int damageMin = 1, damageMax = 2;
    //public int staminaCost = 1;
    //public int accuracyPercent = 95;
    //public float cooldown = 1.1f;

    //public void SetTextBody()
    //{
    //    //string text = "inflict <u>" + bleeding.ToString() + "</u>";
    //    //string text = LocalizationManager.Instance.Get

    //    //textBody.text = text;

    //    ItemsText itemText = LocalizationManager.Instance.GetTextItem("ru", "hiddenDagger");
        
        
    //    string test = JsonUtility.ToJson(itemText);

    //    JsonObject test2 = new JsonObject();
    //    //test2 = JsonSerializer.Deserialize<JsonObject>(jsonString);


    //    /*
    //      string.Format("Player {0} is at level {1} with {2} health.", playerName, playerLevel, playerHealth);
    //      */



    //    // Регулярное выражение для разбора строки
    //    string pattern = @"^(.*?)\s+(\w+)\s+(\w+)\(([-\d]+f),([-\d]+f)\)$";
    //    Regex regex = new Regex(pattern);

    //    Match match = regex.Match(itemText.description);
    //    if (match.Success)
    //    {
    //        string text = match.Groups[1].Value; // "Первый удар: накладывает"
    //        string variableName = match.Groups[2].Value; // "countIcon1"
    //        string effectName = match.Groups[3].Value; // "IconBleed"
    //        string number1 = match.Groups[4].Value; // "15f"
    //        string number2 = match.Groups[5].Value; // "15f"

    //        //itemText[match.Groups[1].Value] = text;

    //        // Убираем "f" из чисел, если нужно
    //        float value1 = float.Parse(number1.Replace("f", ""));
    //        float value2 = float.Parse(number2.Replace("f", ""));

    //        Debug.Log($"Текст: {text}");
    //        Debug.Log($"Имя переменной: {variableName}");
    //        Debug.Log($"Имя эффекта: {effectName}");
    //        Debug.Log($"Число 1: {value1}");
    //        Debug.Log($"Число 2: {value2}");

    //        textBody.text = text + " " + bleeding + " ";
    //        var icon = Instantiate(Resources.Load<GameObject>("Icons/" + effectName), gameObject.transform.GetChild(1).transform);
    //        icon.GetComponent<RectTransform>().anchoredPosition = new Vector3(value1, value2, 0);
    //    }
    //    else
    //    {
    //        Debug.Log("?");
    //    }



        

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
     <size=200>On hit: applies <u>1</u>        on enemy                                fire items      activate:  Drop <u>2</u>          from enemy and deal <u>5</u> damage  </size> 
    */

    private void Start()
    {
        //SetTextBody();
        //SetTextStat();
    }

    void Update()
    {
        
    }
}
