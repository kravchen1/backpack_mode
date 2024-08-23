using UnityEngine;

public class Weapon : Item
{
    public float timer_cooldown = 5f;
    protected float timer = 0f;
    protected bool timer_locked_out = true;
}
