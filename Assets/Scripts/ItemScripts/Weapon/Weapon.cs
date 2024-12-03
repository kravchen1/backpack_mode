using UnityEngine;

public class Weapon : Item
{
    public int attackMin;
    public int attackMax;
    public int stamina;
    public int accuracy;
    public int baseDamageCrit = 150;

    protected float timer = 0f;
    protected bool timer_locked_out = true;


    protected bool HaveStamina()
    {
        if (Player.stamina - stamina >= 0)
            return true;
        else return false;
    }

   
}
