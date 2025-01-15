public class Quest
{
    public string questName;
    public string description;
    public bool isCompleted;
    public int currentProgress;
    public int necessaryProgress;
    public Quest(string name, string desc, int necessaryProgress = 0)
    {
        questName = name;
        description = desc;
        currentProgress = 0;
        this.necessaryProgress = necessaryProgress;
        isCompleted = false;
    }
}