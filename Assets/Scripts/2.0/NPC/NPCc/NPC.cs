using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D), typeof(NPCDataManager))]
public class NPC : MonoBehaviour
{
    [Header("NPC Configuration")]
    [SerializeField] private NPCConfig config;
    [Header("Group Behavior")]
    [SerializeField] private bool isMoveGroup = false;
    [SerializeField] private bool isLeader = false;
    [SerializeField] public List<NPC> npcGroups;

    [Header("References")]
    [SerializeField] protected TextMeshPro nameTextMeshPro;
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

    // Group Behavior
    private Coroutine groupFollowCoroutine;
    public bool IsMoveGroup => isMoveGroup;
    public bool IsLeader => isLeader;
    public NPC GroupLeader { get; private set; }

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

        var animScript = GetComponent<NPCAnimationController>();
        var animatior = GetComponent<Animator>();
        var sprites = transform.GetChild(0).gameObject;
        var light2d = transform.GetChild(1).gameObject;
        var trigger = transform.GetChild(2).gameObject;
        var nameText = transform.GetChild(3).gameObject;
        var behindForward = transform.GetChild(4).gameObject;

        animScript.enabled = false;
        animatior.enabled = false;
        sprites.SetActive(false);
        light2d.SetActive(false);
        trigger.SetActive(false);
        nameText.SetActive(false);
        behindForward.SetActive(false);
    }

    protected virtual void Start()
    {
        SetState(config.initialState);
        SetupDetectionCollider();
        InitializeGroupBehavior(); // Инициализация группы
    }

    private void Update()
    {
        currentState?.UpdateState(this);

        // Debug visualization
        if (HasDetectedPlayer)
        {
            Debug.DrawLine(transform.position, DetectedPlayer.transform.position, GetStateColor());
        }

        // Визуализация групповых связей
        if (isMoveGroup && !isLeader && GroupLeader != null)
        {
            Debug.DrawLine(transform.position, GroupLeader.transform.position, Color.cyan);
        }
    }

    private void OnDestroy()
    {
        currentState?.ExitState(this);
        StopGroupBehavior();
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

    #region Group Behavior Methods
    public void InitializeGroupBehavior()
    {
        if (!isMoveGroup || npcGroups == null || npcGroups.Count == 0)
            return;

        // Находим лидера в группе
        GroupLeader = FindGroupLeader();

        if (GroupLeader == null)
        {
            Debug.LogWarning($"{name}: Группа настроена, но лидер не найден!");
            isMoveGroup = false;
            return;
        }

        // Если этот NPC не лидер, настраиваем следование за лидером
        if (!isLeader && GroupLeader != null)
        {
            StartGroupFollowing();
        }
    }

    private NPC FindGroupLeader()
    {
        // Ищем лидера в группе
        foreach (var npc in npcGroups)
        {
            if (npc != null && npc.isLeader && npc != this)
                return npc;
        }

        // Если этот NPC лидер
        if (isLeader)
            return this;

        return null;
    }

    private void StartGroupFollowing()
    {
        if (GroupLeader == null || isLeader) return;

        StopGroupBehavior();
        groupFollowCoroutine = StartCoroutine(FollowGroupLeaderCoroutine());
    }

    private void StopGroupBehavior()
    {
        if (groupFollowCoroutine != null)
        {
            StopCoroutine(groupFollowCoroutine);
            groupFollowCoroutine = null;
        }
    }

    private IEnumerator FollowGroupLeaderCoroutine()
    {
        while (GroupLeader != null && isMoveGroup && !isLeader && GroupLeader.isActiveAndEnabled)
        {
            float distanceToLeader = Vector2.Distance(transform.position, GroupLeader.transform.position);

            // Если далеко от лидера - двигаемся к нему
            if (distanceToLeader > 3f)
            {
                Vector3 groupPosition = GetGroupPosition();
                Vector3 correctedPosition = GetMovementCorrectedPosition(groupPosition);

                MoveToPosition(correctedPosition, 1.5f);
            }
            else
            {
                // Близко к лидеру - можно остановиться или делать мелкие корректировки
                if (distanceToLeader < 1f)
                {
                    StopMovement();
                }
            }

            yield return new WaitForSeconds(0.5f);
        }

        // Если вышли из цикла - лидер потерян
        if (isMoveGroup && !isLeader)
        {
            Debug.LogWarning($"{name}: Потерял лидера группы!");
            GroupLeader = FindGroupLeader();
            if (GroupLeader != null)
            {
                StartGroupFollowing();
            }
        }
    }

    private Vector3 GetGroupPosition()
    {
        if (GroupLeader == null) return transform.position;

        // Получаем индекс этого NPC в группе для определения позиции
        int followerIndex = GetFollowerIndex();

        // Распределяем позиции вокруг лидера
        float angleStep = 360f / (GetActiveFollowersCount() + 1);
        float angle = followerIndex * angleStep;
        float radius = 2f; // Радиус формирования

        Vector3 offset = Quaternion.Euler(0, 0, angle) * Vector3.right * radius;
        return GroupLeader.transform.position + offset;
    }

    private int GetFollowerIndex()
    {
        if (npcGroups == null) return 0;

        int index = 0;
        foreach (var npc in npcGroups)
        {
            if (npc == this)
                return index;
            if (npc != null && !npc.isLeader && npc.isActiveAndEnabled)
                index++;
        }
        return index;
    }

    private int GetActiveFollowersCount()
    {
        if (npcGroups == null) return 0;

        int count = 0;
        foreach (var npc in npcGroups)
        {
            if (npc != null && !npc.isLeader && npc.isActiveAndEnabled)
                count++;
        }
        return count;
    }

    public void OnGroupLeaderChanged()
    {
        if (isMoveGroup && !isLeader)
        {
            StopGroupBehavior();
            InitializeGroupBehavior();
        }
    }

    public bool ShouldFollowGroup()
    {
        return isMoveGroup && !isLeader && GroupLeader != null && GroupLeader.isActiveAndEnabled;
    }

    public Vector3 GetGroupFormationPosition()
    {
        return GetGroupPosition();
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

        StartCoroutine(FollowTransformCoroutine(targetTransform, customStoppingDistance));
    }

    public void StartPatrol(Vector3[] waypoints)
    {
        if (navMeshAgent == null) return;

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
    public virtual void MakeHostile() => SetState(NPCStateType.Hostile);
    public virtual void MakeNeutral() => SetState(NPCStateType.Neutral);
    public virtual void MakeFriendly() => SetState(NPCStateType.Friendly);

    public bool IsHostile() => currentState?.Type == NPCStateType.Hostile;
    public bool IsNeutral() => currentState?.Type == NPCStateType.Neutral;
    public bool IsFriendly() => currentState?.Type == NPCStateType.Friendly;

    public virtual void CheckCharisma()
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
    protected virtual void OnTriggerEnter2D(Collider2D other)
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

    protected virtual void OnTriggerExit2D(Collider2D other)
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

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        ValidateGroupSettings();
    }

    private void ValidateGroupSettings()
    {
        if (isMoveGroup)
        {
            // Проверяем что группа не пустая
            if (npcGroups == null || npcGroups.Count == 0)
            {
                Debug.LogWarning($"{name}: isMoveGroup=true, но npcGroups пуст!", this);
                return;
            }

            // Проверяем наличие лидера в группе
            bool hasLeader = false;
            foreach (var npc in npcGroups)
            {
                if (npc != null && npc.isLeader && npc != this)
                {
                    hasLeader = true;
                    break;
                }
            }

            // Проверяем себя как лидера
            if (!hasLeader && isLeader)
            {
                hasLeader = true;
            }

            if (!hasLeader)
            {
                Debug.LogError($"{name}: isMoveGroup=true, но в группе нет лидера (isLeader=true)!", this);
            }

            // Предупреждение если этот NPC и лидер, и в группе
            if (isLeader && npcGroups.Contains(this))
            {
                Debug.LogWarning($"{name}: NPC является лидером и включен в свою же группу. Это может вызвать циклические ссылки!", this);
            }
        }
    }
#endif

    // Для обратной совместимости и удобства
    [ContextMenu("Test Make Hostile")]
    private void TestMakeHostile() => MakeHostile();

    [ContextMenu("Test Make Friendly")]
    private void TestMakeFriendly() => MakeFriendly();

    [ContextMenu("Test Make Neutral")]
    private void TestMakeNeutral() => MakeNeutral();
}