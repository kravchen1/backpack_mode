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

    protected void Attack(int damage)
    {
        float armorBefore = Enemy.armor;
        if (Enemy.armor > 0)
        {
            Enemy.armor -= damage;

            if (Enemy.armor < 0)
            {
                Enemy.hp = Enemy.hp + Enemy.armor - damage;
                Debug.Log(gameObject.name + "ломает " + armorBefore + " брони и режет плоть на " + (Enemy.armor - damage) + " здоровья");

            }
            else
            {
                Debug.Log(gameObject.name + "ломает " + armorBefore + " брони");
            }
        }
        else
        {
            Enemy.hp -= damage;
            Debug.Log(gameObject.name + "режет плоть на " + damage + " здоровья");
        }
    }
}
