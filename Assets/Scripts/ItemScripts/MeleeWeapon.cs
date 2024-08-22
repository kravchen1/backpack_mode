using UnityEngine;

public class MeleeWeapon : Weapon
{
    public int attackMin;
    public int attackMax;
    public int stamina;
    public int Accuracy;

    public float timer_cooldown = 5f;
    protected float timer = 0f;
    protected bool timer_locked_out = false;
}
