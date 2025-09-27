// NPCController.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private NPCConfig config;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private CircleCollider2D detectionCollider;
    [SerializeField] private NPCAnimationController animationController;

    // State Management
    private Dictionary<NPCStateType, INPCState> states;
    private INPCState currentState;

    // Components
    private Rigidbody2D rb;
    private TopDownCharacterController player;

    // Properties
    public NPCConfig Config => config;
    public TopDownCharacterController Player => player;
    public NPCAnimationController AnimationController => animationController;
    public bool HasDetectedPlayer => player != null;

    void Awake()
    {
        InitializeComponents();
        InitializeStates();
    }

    void Start()
    {
        SetState(config.initialState);
        SetupDetectionCollider();
    }

    void Update()
    {
        currentState?.UpdateState(this);

        // Debug visualization
        if (HasDetectedPlayer)
        {
            Debug.DrawLine(transform.position, player.transform.position, GetStateColor());
        }
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void InitializeStates()
    {
        states = new Dictionary<NPCStateType, INPCState>
        {
            { NPCStateType.Hostile, new HostileNPCState() },
            { NPCStateType.Neutral, new NeutralNPCState() },
            { NPCStateType.Friendly, new FriendlyNPCState() }
        };
    }

    private void SetupDetectionCollider()
    {
        if (detectionCollider != null)
        {
            detectionCollider.radius = config.detectionRadius;
        }
    }

    public void SetState(NPCStateType newStateType)
    {
        if (states.TryGetValue(newStateType, out INPCState newState))
        {
            currentState?.ExitState(this);
            currentState = newState;
            currentState.EnterState(this);
        }
    }

    public void ChangeColor(Color color)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
        }
    }

    private Color GetStateColor()
    {
        return currentState.Type switch
        {
            NPCStateType.Hostile => config.hostileColor,
            NPCStateType.Neutral => config.neutralColor,
            NPCStateType.Friendly => config.friendlyColor,
            _ => Color.white
        };
    }

    // Movement methods similar to your character controller
    public void Move(Vector2 direction)
    {
        if (rb != null)
        {
            rb.velocity = direction.normalized * config.moveSpeed;
        }
    }

    public void StopMovement()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void StartChasing(TopDownCharacterController target)
    {
        // Implementation for chasing behavior
        StartCoroutine(ChaseCoroutine(target));
    }

    public void ApproachPlayer(TopDownCharacterController player)
    {
        // Implementation for friendly approach
        StartCoroutine(ApproachCoroutine(player));
    }

    private IEnumerator ChaseCoroutine(TopDownCharacterController target)
    {
        while (HasDetectedPlayer && currentState.Type == NPCStateType.Hostile)
        {
            Vector2 direction = (target.transform.position - transform.position).normalized;
            Move(direction * config.chaseSpeed);
            yield return null;
        }
        StopMovement();
    }

    private IEnumerator ApproachCoroutine(TopDownCharacterController target)
    {
        float distance = Vector2.Distance(transform.position, target.transform.position);

        while (distance > config.interactionRadius && HasDetectedPlayer)
        {
            Vector2 direction = (target.transform.position - transform.position).normalized;
            Move(direction * config.moveSpeed);
            distance = Vector2.Distance(transform.position, target.transform.position);
            yield return null;
        }

        StopMovement();
        // Trigger interaction
    }

    // Trigger events
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Проверяем, что это именно триггерный коллайдер
        if (!other.isTrigger) return;

        var playerController = other.GetComponent<TopDownCharacterController>();
        Debug.Log("Trigger Player in");
        if (playerController != null)
        {
            player = playerController;
            currentState?.OnPlayerDetected(this, player);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Проверяем, что это именно триггерный коллайдер
        if (!other.isTrigger) return;

        var playerController = other.GetComponent<TopDownCharacterController>();
        Debug.Log("Trigger Player out");
        if (playerController == player)
        {
            currentState?.OnPlayerLost(this);
            player = null;
        }
    }

    // Добавляем методы для управления анимациями
    public void ForceLookAt(Vector2 direction, float duration = -1f)
    {
        animationController?.ForceLookAt(direction, duration);
    }
    
    // Новый метод для плавного поворота к точке
    public void LookAtPosition(Vector3 position, float duration = 3f)
    {
        Vector2 direction = (position - transform.position).normalized;
        ForceLookAt(direction, duration);
    }


    // Public API для динамического изменения состояний
    public void MakeHostile() => SetState(NPCStateType.Hostile);
    public void MakeNeutral() => SetState(NPCStateType.Neutral);
    public void MakeFriendly() => SetState(NPCStateType.Friendly);

    public bool IsHostile() => currentState.Type == NPCStateType.Hostile;
    public bool IsNeutral() => currentState.Type == NPCStateType.Neutral;
    public bool IsFriendly() => currentState.Type == NPCStateType.Friendly;


    [ContextMenu("Test Make Hostile")]
    private void TestMakeHostile()
    {
        MakeHostile();
    }

    [ContextMenu("Test Make Friendly")]
    private void TestMakeFriendly()
    {
        MakeFriendly();
    }

    [ContextMenu("Test Make Neutral")]
    private void TestMakeNeutral()
    {
        MakeNeutral();
    }

}