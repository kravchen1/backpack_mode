using UnityEngine;

public class IceProjectile : Projectile
{
    [Header("Ice Effect")]
    [SerializeField] private float slowMultiplier = 0.4f;
    [SerializeField] private float slowDuration = 3f;
    [SerializeField] private GameObject iceImpactEffect;

    protected override void ApplyEffects(Collider2D target)
    {
        base.ApplyEffects(target);

        // Применяем эффект замедления
        var slowEffect = target.GetComponent<SlowEffect>();
        if (slowEffect != null)
        {
            slowEffect.ApplySlow(slowMultiplier, slowDuration);
        }

        // Визуальный эффект попадания
        if (iceImpactEffect != null)
        {
            Instantiate(iceImpactEffect, transform.position, Quaternion.identity);
        }
    }
}