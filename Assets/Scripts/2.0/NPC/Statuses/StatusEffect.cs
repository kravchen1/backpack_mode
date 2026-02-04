// StatusEffect.cs - убираем зависимость от BaseNPC
using UnityEngine;

public abstract class StatusEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    [SerializeField] protected bool canApply = true;
    [SerializeField] protected float duration = 3f;
    [SerializeField] protected Color effectColor = Color.white;

    [Header("Visual")]
    [SerializeField] protected ParticleSystem particles;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    // Меняем BaseNPC на интерфейс
    protected IDamageable damageable;
    protected MonoBehaviour npcMonoBehaviour;

    protected float timer;
    protected bool isActive;

    protected virtual void Awake()
    {
        damageable = GetComponent<IDamageable>();
        npcMonoBehaviour = GetComponent<MonoBehaviour>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (particles != null)
            particles.Stop();
    }

    protected virtual void Update()
    {
        if (!isActive || damageable == null) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            RemoveEffect();
            return;
        }

        UpdateEffect(Time.deltaTime);
    }

    public virtual void ApplyEffect()
    {
        if (!canApply || damageable == null) return;

        timer = duration;
        isActive = true;

        if (spriteRenderer != null)
            spriteRenderer.color = effectColor;

        if (particles != null)
            particles.Play();

        OnEffectApplied();
    }

    public virtual void RemoveEffect()
    {
        isActive = false;
        timer = 0;

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        if (particles != null)
            particles.Stop();

        OnEffectRemoved();
    }

    protected abstract void UpdateEffect(float deltaTime);
    protected abstract void OnEffectApplied();
    protected abstract void OnEffectRemoved();

    public bool IsEffectActive => isActive;
}