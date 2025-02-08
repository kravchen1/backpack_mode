using System;

[Serializable]
public class Quest
{
    public string questName;
    public string description;
    public bool isCompleted;
    public int currentProgress;
    public int necessaryProgress;
    public int id;
    public Quest(string name, string desc, int necessaryProgress = 0, int id = 0)
    {
        questName = name;
        description = desc;
        currentProgress = 0;
        this.necessaryProgress = necessaryProgress;
        isCompleted = false;
        this.id = id;
    }
}