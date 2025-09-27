using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCNavigationAgent : MonoBehaviour
{
    [Header("Navigation References")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private NPCConfig config;

    private Coroutine currentMovementCoroutine;
    private Vector3[] patrolWaypoints;
    private int currentWaypointIndex = 0;

    public NavMeshAgent Agent => navMeshAgent;
    public bool IsPathValid => navMeshAgent.hasPath && navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance;
    public bool HasReachedDestination => navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;

    void Awake()
    {
        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();

        SetupNavMeshAgent();
    }

    private void SetupNavMeshAgent()
    {
        if (navMeshAgent == null) return;

        navMeshAgent.speed = config.moveSpeed;
        navMeshAgent.acceleration = config.acceleration;
        navMeshAgent.angularSpeed = config.angularSpeed;
        navMeshAgent.stoppingDistance = config.stoppingDistance;
        navMeshAgent.autoBraking = config.autoBraking;

        // Для 2D навигации
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    #region Public Movement API

    public void MoveToPosition(Vector2 targetPosition, float customStoppingDistance = -1f)
    {
        StopAllMovement();

        if (customStoppingDistance >= 0)
            navMeshAgent.stoppingDistance = customStoppingDistance;

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(targetPosition);
    }

    public void MoveToTransform(Transform targetTransform, float customStoppingDistance = -1f)
    {
        StopAllMovement();
        currentMovementCoroutine = StartCoroutine(FollowTransformCoroutine(targetTransform, customStoppingDistance));
    }

    public void StartPatrol(Vector3[] waypoints)
    {
        StopAllMovement();
        patrolWaypoints = waypoints;
        currentMovementCoroutine = StartCoroutine(PatrolCoroutine());
    }

    public void StopMovement()
    {
        navMeshAgent.isStopped = true;
        StopAllMovement();
    }

    public void ResumeMovement()
    {
        navMeshAgent.isStopped = false;
    }

    public void SetSpeed(float speed)
    {
        navMeshAgent.speed = speed;
    }

    public void ResetSpeed()
    {
        navMeshAgent.speed = config.moveSpeed;
    }

    #endregion

    #region Movement Coroutines

    private IEnumerator FollowTransformCoroutine(Transform target, float stoppingDistance = -1f)
    {
        float actualStoppingDistance = stoppingDistance >= 0 ? stoppingDistance : config.stoppingDistance;

        while (target != null)
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

            yield return new WaitForSeconds(0.1f); // Оптимизация: проверяем каждые 0.1 сек
        }
    }

    private IEnumerator PatrolCoroutine()
    {
        if (patrolWaypoints == null || patrolWaypoints.Length == 0)
        {
            Debug.LogWarning("No patrol waypoints set!");
            yield break;
        }

        while (true)
        {
            // Двигаемся к текущей точке
            navMeshAgent.SetDestination(patrolWaypoints[currentWaypointIndex]);
            navMeshAgent.isStopped = false;

            // Ждем достижения точки
            yield return new WaitUntil(() =>
                HasReachedDestination ||
                Vector2.Distance(transform.position, patrolWaypoints[currentWaypointIndex]) <= config.waypointReachedDistance);

            // Ждем на точке
            yield return new WaitForSeconds(config.waypointWaitTime);

            // Переходим к следующей точке
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
        }
    }

    #endregion

    private void StopAllMovement()
    {
        if (currentMovementCoroutine != null)
        {
            StopCoroutine(currentMovementCoroutine);
            currentMovementCoroutine = null;
        }
    }

    // Для визуализации в редакторе
    private void OnDrawGizmosSelected()
    {
        if (navMeshAgent != null && navMeshAgent.hasPath)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, navMeshAgent.destination);

            // Рисуем путь
            for (int i = 0; i < navMeshAgent.path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);
                Gizmos.DrawSphere(navMeshAgent.path.corners[i], 0.1f);
            }
        }

        // Рисуем точки патрулирования
        if (patrolWaypoints != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var point in patrolWaypoints)
            {
                Gizmos.DrawWireSphere(point, 0.2f);
            }
        }
    }
}