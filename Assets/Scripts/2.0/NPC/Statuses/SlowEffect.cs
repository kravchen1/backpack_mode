using UnityEngine;
using UnityEngine.AI;

public class SlowEffect : StatusEffect
{
    [Header("Slow Settings")]
    [SerializeField] private float speedMultiplier = 0.5f;
    [SerializeField] private Color slowColor = new Color(0.5f, 0.8f, 1f); // Голубой

    private NavMeshAgent navAgent;
    private Rigidbody2D rb2D;
    private MonoBehaviour movementScript;
    private float originalSpeed;

    protected override void Awake()
    {
        base.Awake();
        effectColor = slowColor;

        navAgent = GetComponent<NavMeshAgent>();
        rb2D = GetComponent<Rigidbody2D>();

        if (navAgent != null)
            originalSpeed = navAgent.speed;
    }

    public void ApplySlow(float multiplier, float slowDuration)
    {
        speedMultiplier = multiplier;
        duration = slowDuration;
        ApplyEffect();
    }

    protected override void UpdateEffect(float deltaTime)
    {
        // Логика замедления в Update не нужна
    }

    protected override void OnEffectApplied()
    {
        Debug.Log($"{name} замедлен! Скорость: x{speedMultiplier}");

        if (navAgent != null)
        {
            navAgent.speed = originalSpeed * speedMultiplier;
        }

        if (rb2D != null)
        {
            rb2D.linearVelocity *= speedMultiplier;
        }

        // Ищем скрипт NPCController
        var npcController = GetComponent<NPCController>();
        if (npcController != null)
        {
            movementScript = npcController;
            var speedField = typeof(NPCController).GetField("moveSpeed");
            if (speedField != null)
            {
                originalSpeed = (float)speedField.GetValue(npcController);
                speedField.SetValue(npcController, originalSpeed * speedMultiplier);
            }
        }
    }

    protected override void OnEffectRemoved()
    {
        Debug.Log($"{name} больше не замедлен");

        if (navAgent != null)
        {
            navAgent.speed = originalSpeed;
        }

        if (movementScript != null)
        {
            var speedField = movementScript.GetType().GetField("moveSpeed");
            if (speedField != null)
            {
                speedField.SetValue(movementScript, originalSpeed);
            }
        }
    }
}