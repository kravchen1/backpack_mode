using System;
using UnityEngine;

[Serializable]
public class BattleData
{
    public int type;
    public int lvlEnemy;
    public int id;

    public BattleData(int id, int type, int lvlEnemy)
    {
        this.id = id;
        this.type = type;
        this.lvlEnemy = lvlEnemy;
    }
}
