using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody2D), typeof(NPCDataManager))]
public class NPC : MonoBehaviour
{
    [Header("NPC Configuration")]
    [SerializeField] private NPCConfig config;
    [SerializeField] public List<NPC> npcGroups;

    [Header("References")]
    [SerializeField] private TextMeshPro nameTextMeshPro;
    [SerializeField] private CircleCollider2D detectionCollider;
    [SerializeField] private NPCAnimationController animationController;
    [SerializeField] private WaypointContainer waypointContainer;
    [SerializeField] private AreaMovementContainer areaMovementContainer;

    // State Management
    private Dictionary<NPCStateType, INPCState> states;
    [HideInInspector] public INPCState currentState;
    private NPCDataManager dataManager;
    private Rigidbody2D rb;
    [HideInInspector] public NavMeshAgent navMeshAgent;

    // Properties
    public NPCConfig Config => config;
    public NPCDataManager DataManager => dataManager;
    public NPCAnimationController AnimationController => animationController;
    public WaypointContainer WaypointContainer => waypointContainer;
    public TopDownCharacterController DetectedPlayer { get; set; }
    public AreaMovementContainer AreaMovementContainer => areaMovementContainer; // НОВОЕ: Свойство доступа
    public bool HasDetectedPlayer => DetectedPlayer != null;
    public bool InFightNow { get; set; }

    // Events
    public System.Action<INPCState, INPCState> OnStateChanged;

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeComponents();
        InitializeStates();
    }

    private void Start()
    {
        SetState(config.initialState);
        SetupDetectionCollider();
    }

    private void Update()
    {
        currentState?.UpdateState(this);

        // Debug visualization
        if (HasDetectedPlayer)
        {
            Debug.DrawLine(transform.position, DetectedPlayer.transform.position, GetStateColor());
        }
    }

    private void OnDestroy()
    {
        currentState?.ExitState(this);
    }
    #endregion

    #region Initialization
    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        dataManager = GetComponent<NPCDataManager>();

        // Initialize NavMeshAgent
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            SetupNavMeshAgent();
        }

        if (nameTextMeshPro == null)
            nameTextMeshPro = GetComponentInChildren<TextMeshPro>();

        GetComponent<NPCDataManager>().CharacterName = Config.NPCName;
        nameTextMeshPro.text = Config.NPCName;

        InFightNow = false;
    }

    private void SetupNavMeshAgent()
    {
        navMeshAgent.speed = config.moveSpeed;
        navMeshAgent.acceleration = config.acceleration;
        navMeshAgent.angularSpeed = config.angularSpeed;
        navMeshAgent.stoppingDistance = config.stoppingDistance;
        navMeshAgent.autoBraking = config.autoBraking;
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
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
    #endregion

    #region State Management
    public void SetState(NPCStateType newStateType)
    {
        if (states.TryGetValue(newStateType, out INPCState newState))
        {
            var previousState = currentState;
            currentState?.ExitState(this);
            currentState = newState;
            currentState.EnterState(this);

            OnStateChanged?.Invoke(previousState, currentState);
        }
    }

    public T GetState<T>() where T : class, INPCState
    {
        foreach (var state in states.Values)
        {
            if (state is T typedState)
                return typedState;
        }
        return null;
    }
    #endregion

    #region Movement API
    public void MoveToPosition(Vector2 targetPosition, float customStoppingDistance = -1f)
    {
        if (navMeshAgent == null || !navMeshAgent.isActiveAndEnabled) return;

        if (customStoppingDistance >= 0)
            navMeshAgent.stoppingDistance = customStoppingDistance;

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(targetPosition);
    }



    public void MoveToTransform(Transform targetTransform, float customStoppingDistance = -1f)
    {
        if (navMeshAgent == null) return;

        StopAllCoroutines();
        StartCoroutine(FollowTransformCoroutine(targetTransform, customStoppingDistance));
    }

    public void StartPatrol(Vector3[] waypoints)
    {
        if (navMeshAgent == null) return;

        StopAllCoroutines();
        StartCoroutine(PatrolCoroutine(waypoints));
    }

    public void StopMovement()
    {
        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
        {
            navMeshAgent.isStopped = true;
        }
        StopAllCoroutines();
    }

    public void SetSpeed(float speed)
    {
        if (navMeshAgent != null)
            navMeshAgent.speed = speed;
    }

    public void ResetSpeed()
    {
        if (navMeshAgent != null)
            navMeshAgent.speed = config.moveSpeed;
    }

    public bool HasReachedDestination
    {
        get
        {
            if (navMeshAgent == null || !navMeshAgent.isActiveAndEnabled || !navMeshAgent.isOnNavMesh)
                return true;

            return navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending;
        }
    }

    // НОВОЕ: Метод для проверки и коррекции позиции относительно области движения
    public Vector3 GetMovementCorrectedPosition(Vector3 targetPosition)
    {
        if (areaMovementContainer == null)
            return targetPosition;

        return areaMovementContainer.GetClosestPointInArea(targetPosition);
    }

    // НОВОЕ: Проверка находится ли точка в разрешенной области
    public bool IsPositionInMovementArea(Vector3 position)
    {
        if (areaMovementContainer == null)
            return true;

        return areaMovementContainer.IsPointInArea(position);
    }

    #endregion

    #region Visual & Animation
    public void ChangeColor(Color color)
    {
        if (nameTextMeshPro != null)
        {
            nameTextMeshPro.color = color;
        }
    }

    public void ForceLookAt(Vector2 direction, float duration = -1f)
    {
        animationController?.ForceLookAt(direction, duration);
    }

    public void LookAtPosition(Vector3 position, float duration = 3f)
    {
        Vector2 direction = (position - transform.position).normalized;
        ForceLookAt(direction, duration);
    }

    private Color GetStateColor()
    {
        return currentState?.Type switch
        {
            NPCStateType.Hostile => config.hostileColor,
            NPCStateType.Neutral => config.neutralColor,
            NPCStateType.Friendly => config.friendlyColor,
            _ => Color.white
        };
    }
    #endregion

    #region Behavior API
    public void MakeHostile() => SetState(NPCStateType.Hostile);
    public void MakeNeutral() => SetState(NPCStateType.Neutral);
    public void MakeFriendly() => SetState(NPCStateType.Friendly);

    public bool IsHostile() => currentState?.Type == NPCStateType.Hostile;
    public bool IsNeutral() => currentState?.Type == NPCStateType.Neutral;
    public bool IsFriendly() => currentState?.Type == NPCStateType.Friendly;

    public void CheckCharisma()
    {
        int charisma = PlayerDataManager.Instance.Stats.attributes.Charisma;
        if (charisma >= config.CharismaForFriendly)
        {
            MakeFriendly();
        }
        else if (charisma <= config.CharismaForHostile)
        {
            MakeHostile();
        }
        else
        {
            MakeNeutral();
        }
    }
    #endregion

    #region Movement Coroutines
    private IEnumerator FollowTransformCoroutine(Transform target, float stoppingDistance = -1f)
    {
        float actualStoppingDistance = stoppingDistance >= 0 ? stoppingDistance : config.stoppingDistance;

        while (target != null && navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
        {
            if (Vector2.Distance(transform.position, target.position) > actualStoppingDistance)
            {
                navMeshAgent.SetDestination(target.position);
            }
            else
            {
                navMeshAgent.isStopped = true;
                yield break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator PatrolCoroutine(Vector3[] waypoints)
    {
        if (waypoints == null || waypoints.Length == 0 || navMeshAgent == null)
            yield break;

        int currentWaypointIndex = 0;

        while (navMeshAgent.isActiveAndEnabled)
        {
            navMeshAgent.SetDestination(waypoints[currentWaypointIndex]);
            navMeshAgent.isStopped = false;

            yield return new WaitUntil(() =>
                HasReachedDestination ||
                Vector2.Distance(transform.position, waypoints[currentWaypointIndex]) <= config.waypointReachedDistance);

            yield return new WaitForSeconds(config.waypointWaitTime);
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
    #endregion

    #region Trigger Events
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !other.isTrigger) return;
        if (currentState.Type != NPCStateType.Hostile)
        {
            CheckCharisma();
        }

        var playerController = other.GetComponent<TopDownCharacterController>();
        if (playerController != null)
        {
            DetectedPlayer = playerController;
            currentState?.OnPlayerDetected(this, playerController);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !other.isTrigger) return;

        var playerController = other.GetComponent<TopDownCharacterController>();
        if (playerController == DetectedPlayer)
        {
            // Для дружественных NPC не теряем игрока сразу при выходе из триггера
            // Они будут сами проверять зону движения в своем состоянии
            if (currentState?.Type != NPCStateType.Friendly)
            {
                currentState?.OnPlayerLost(this);
                DetectedPlayer = null;
            }
            // Для дружественных NPC оставляем DetectedPlayer != null
            // Они сами сбросят его когда игрок выйдет за зону движения
        }
    }
    #endregion

    // Для обратной совместимости и удобства
    [ContextMenu("Test Make Hostile")]
    private void TestMakeHostile() => MakeHostile();

    [ContextMenu("Test Make Friendly")]
    private void TestMakeFriendly() => MakeFriendly();

    [ContextMenu("Test Make Neutral")]
    private void TestMakeNeutral() => MakeNeutral();
}