// BaseNPC.cs - убираем abstract
using UnityEngine;

public class BaseNPC : MonoBehaviour, IDamageable  // Убрали abstract
{
    [Header("Health Settings")]
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int currentHealth = 100;

    [Header("Visual")]
    [SerializeField] protected SpriteRenderer spriteRenderer;

    public bool IsAlive => currentHealth > 0;
    public event System.Action OnDeath;

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    protected virtual void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        if (!IsAlive) return;

        //currentHealth -= Mathf.RoundToInt(amount);
        Debug.Log($"{name} получил {amount} урона. HP: {currentHealth}/{maxHealth}");

        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            HandleDeath();
        }
    }

    private System.Collections.IEnumerator DamageFlash()
    {
        if (spriteRenderer == null) yield break;

        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = original;
    }

    protected virtual void HandleDeath()
    {
        Debug.Log($"{name} умер!");
        OnDeath?.Invoke();

        var effects = GetComponents<StatusEffect>();
        foreach (var effect in effects)
            effect.enabled = false;

        if (spriteRenderer != null)
            spriteRenderer.color = new Color(0.3f, 0.3f, 0.3f, 0.7f);

        Destroy(gameObject, 2f);
    }

    public void Heal(int amount)
    {
        if (!IsAlive) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
}