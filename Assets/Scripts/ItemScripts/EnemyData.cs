using System;
using UnityEngine;

[Serializable]
public class EnemyData
{
    public string jsonBackpack;
    public int lvl;

    public EnemyData(string jsonBackpack, int lvl)
    {
        this.jsonBackpack = jsonBackpack;
        this.lvl = lvl;
    }
}
