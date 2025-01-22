using System;
using UnityEngine;

[Serializable]
public class EnemyStatData
{
    public string jsonStat;

    public EnemyStatData(string jsonStat)
    {
        this.jsonStat = jsonStat;
    }
}
