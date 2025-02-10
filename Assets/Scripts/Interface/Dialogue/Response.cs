

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Response
{
    public string responseText;
    public Dialogue nextDialogue; // следующая часть диалога
    public int switchDialogID = 0;


    //взять квест
    public bool quest = false;
    public string questName = "";
    public string questDescription = "";
    public int necessaryProgress = 0;
    public int questID = 0;

    //выдать предметы
    public bool giveItem = false;
    public List<GameObject> giveItemPrefab;

    //выполнить квест
    public bool questComplete = false;
    public int idQuestComplete = 0;
}