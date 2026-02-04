using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float speed = 20f;
    [SerializeField] protected string[] targetTags = { "Enemy" };

    protected Rigidbody2D rb;
    protected Transform owner;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void Initialize(float damageValue, float speedValue, Transform ownerTransform = null)
    {
        damage = damageValue;
        speed = speedValue;
        owner = ownerTransform;
        rb.linearVelocity = transform.right * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (owner != null && other.transform == owner)
            return;

        bool isTarget = false;
        foreach (string tag in targetTags)
        {
            if (other.CompareTag(tag))
            {
                isTarget = true;
                break;
            }
        }

        if (!isTarget) return;

        ApplyEffects(other);

        Destroy(gameObject);
    }

    protected virtual void ApplyEffects(Collider2D target)
    {
        // Базовый урон
        var damageable = target.GetComponent<IDamageable>();
        damageable?.TakeDamage(damage);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}