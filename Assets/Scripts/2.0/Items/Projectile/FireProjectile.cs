using UnityEngine;

public class FireProjectile : Projectile
{
    [Header("Fire Effect")]
    [SerializeField] private float burnDPS = 3f;
    [SerializeField] private float burnDuration = 2f;
    [SerializeField] private GameObject fireImpactEffect;

    protected override void ApplyEffects(Collider2D target)
    {
        base.ApplyEffects(target);

        // Применяем эффект горения
        var burnEffect = target.GetComponent<BurnEffect>();
        if (burnEffect != null)
        {
            burnEffect.ApplyBurn(burnDPS, burnDuration);
        }

        // Визуальный эффект попадания
        if (fireImpactEffect != null)
        {
            Instantiate(fireImpactEffect, transform.position, Quaternion.identity);
        }
    }
}