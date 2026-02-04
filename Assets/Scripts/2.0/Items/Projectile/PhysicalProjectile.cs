// Физический снаряд (базовый тип урона)
using UnityEngine;

public class PhysicalProjectile : Projectile
{
    protected override void ApplyEffects(Collider2D target)
    {
        var damageable = target.GetComponent<IDamageable>();
        damageable?.TakeDamage(damage);
    }
}