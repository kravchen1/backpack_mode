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

    [Header("References")]
    [SerializeField] protected TextMeshPro nameTextMeshPro;
    [SerializeField] private CircleCollider2D detectionCollider;
    [SerializeField] private NPCAnimationController animationController;
    [SerializeField] private WaypointContainer waypointContainer;
    [SerializeField] private AreaMovementContainer areaMovementContainer;

    // State Management
    private NPCDataManager dataManager;
    private Rigidbody2D rb;
    [HideInInspector] public NavMeshAgent navMeshAgent;

    // Visual Components
    private GameObject sprites;
    private GameObject light2d;
    private GameObject trigger;
    private GameObject nameText;
    private GameObject behindForward;
    private Animator animator;

    // Properties
    public NPCConfig Config => config;
    public NPCDataManager DataManager => dataManager;
    public NPCAnimationController AnimationController => animationController;
    public WaypointContainer WaypointContainer => waypointContainer;
    public TopDownCharacterController DetectedPlayer { get; set; }
    public AreaMovementContainer AreaMovementContainer => areaMovementContainer;
    public bool HasDetectedPlayer => DetectedPlayer != null;
    public bool InFightNow { get; set; }


    #region Unity Lifecycle
    private void Awake()
    {
        InitializeComponents();
        InitializeVisualComponents();
        SetVisualComponentsState(false); // Выключаем визуальные компоненты по умолчанию
    }

    protected virtual void Start()
    {
        SetupDetectionCollider();
    }

    private void Update()
    {

        // Debug visualization
        if (HasDetectedPlayer)
        {
            Debug.DrawLine(transform.position, DetectedPlayer.transform.position, Color.gold);
        }
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

    private void InitializeVisualComponents()
    {
        // Получаем ссылки на визуальные компоненты
        animator = GetComponent<Animator>();
        animationController = GetComponent<NPCAnimationController>();

        // Получаем ссылки на дочерние объекты
        if (transform.childCount >= 5)
        {
            sprites = transform.GetChild(0).gameObject;
            light2d = transform.GetChild(1).gameObject;
            trigger = transform.GetChild(2).gameObject;
            nameText = transform.GetChild(3).gameObject;
            behindForward = transform.GetChild(4).gameObject;
        }
        else
        {
            Debug.LogWarning($"{name}: Not enough child objects for visual components", this);
        }
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

    private void SetupDetectionCollider()
    {
        if (detectionCollider != null)
        {
            detectionCollider.radius = config.detectionRadius;
        }
    }
    #endregion

    #region Visual Components Management
    /// <summary>
    /// Включает или выключает все визуальные компоненты NPC
    /// </summary>
    public void SetVisualComponentsState(bool isEnabled)
    {
        // Управляем компонентами
        if (animationController != null)
            animationController.enabled = isEnabled;

        if (animator != null)
            animator.enabled = isEnabled;

        // Управляем дочерними объектами
        if (sprites != null)
            sprites.SetActive(isEnabled);

        if (light2d != null)
            light2d.SetActive(isEnabled);

        if (trigger != null)
            trigger.SetActive(isEnabled);

        if (nameText != null)
            nameText.SetActive(isEnabled);

        if (behindForward != null)
            behindForward.SetActive(isEnabled);

        // Логируем только в редакторе для отладки
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            //Debug.Log($"{name}: Visual components {(isEnabled ? "enabled" : "disabled")}");
        }
#endif
    }

    /// <summary>
    /// Включает визуальные компоненты NPC
    /// </summary>
    public void EnableVisualComponents()
    {
        SetVisualComponentsState(true);
    }

    /// <summary>
    /// Выключает визуальные компоненты NPC
    /// </summary>
    public void DisableVisualComponents()
    {
        SetVisualComponentsState(false);
    }

    /// <summary>
    /// Проверяет, включены ли визуальные компоненты
    /// </summary>
    public bool AreVisualComponentsEnabled()
    {
        return sprites != null && sprites.activeInHierarchy &&
               animationController != null && animationController.enabled;
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

    // Метод для проверки и коррекции позиции относительно области движения
    public Vector3 GetMovementCorrectedPosition(Vector3 targetPosition)
    {
        if (areaMovementContainer == null)
            return targetPosition;

        return areaMovementContainer.GetClosestPointInArea(targetPosition);
    }

    // Проверка находится ли точка в разрешенной области
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

        var playerController = other.GetComponent<TopDownCharacterController>();
        if (playerController != null)
        {
            DetectedPlayer = playerController;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !other.isTrigger) return;

        var playerController = other.GetComponent<TopDownCharacterController>();
    }
    #endregion
}