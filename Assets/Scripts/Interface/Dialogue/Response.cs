

[System.Serializable]
public class Response
{
    public string responseText;
    public Dialogue nextDialogue; // следующая часть диалога
    public bool quest = false;
    public string questName = "";
    public string questDescription = "";
    public int necessaryProgress = 0;
}