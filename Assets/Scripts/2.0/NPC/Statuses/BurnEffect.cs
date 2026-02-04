using UnityEngine;

public class BurnEffect : StatusEffect
{
    [Header("Burn Settings")]
    [SerializeField] private float damagePerSecond = 5f;
    [SerializeField] private Color burnColor = new Color(1f, 0.5f, 0f); // Оранжевый

    private void Start()
    {
        effectColor = burnColor;
    }

    public void ApplyBurn(float dps, float burnDuration)
    {
        damagePerSecond = dps;
        duration = burnDuration;
        ApplyEffect();
    }

    protected override void UpdateEffect(float deltaTime)
    {
        // Наносим периодический урон через IDamageable
        if (damageable != null)
        {
            damageable.TakeDamage(damagePerSecond * deltaTime);
        }
    }

    protected override void OnEffectApplied()
    {
        Debug.Log($"{name} начал гореть! Урон: {damagePerSecond}/сек");
    }

    protected override void OnEffectRemoved()
    {
        Debug.Log($"{name} перестал гореть");
    }
}