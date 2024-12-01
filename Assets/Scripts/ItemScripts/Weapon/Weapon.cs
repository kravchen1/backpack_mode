using UnityEngine;

public class Weapon : Item
{
    public int attackMin;
    public int attackMax;
    public int stamina;
    public int Accuracy;
    public int damageCrit = 150;

    protected float timer = 0f;
    protected bool timer_locked_out = true;
}
