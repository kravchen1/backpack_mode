using System;
using UnityEngine;

[Serializable]
public class BattleData
{
    public int type;
    public int lvlEnemy;
    public int id;
    public bool die;
    public string JSONBackpack;


    public BattleData(int id, int type, int lvlEnemy, string jSONBackpack)
    {
        this.id = id;
        this.type = type;
        this.lvlEnemy = lvlEnemy;
        this.die = false;
        this.JSONBackpack = jSONBackpack;
    }
    public BattleData(int id, int type, int lvlEnemy, bool die, string jSONBackpack)
    {
        this.id = id;
        this.type = type;
        this.lvlEnemy = lvlEnemy;
        this.die = die;
        this.JSONBackpack = jSONBackpack;
    }
}
