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
    public Vector2 position;


    public BattleData(int id, int type, int lvlEnemy, string jSONBackpack, Vector2 position)
    {
        this.id = id;
        this.type = type;
        this.lvlEnemy = lvlEnemy;
        this.die = false;
        this.JSONBackpack = jSONBackpack;
        this.position = position;
    }
    public BattleData(int id, int type, int lvlEnemy, bool die, string jSONBackpack, Vector2 position)
    {
        this.id = id;
        this.type = type;
        this.lvlEnemy = lvlEnemy;
        this.die = die;
        this.JSONBackpack = jSONBackpack;
        this.position = position;
    }
}
