using System.Collections.Generic;
using UnityEngine;

public class TargetFinder : MonoBehaviour
{
    public enum TargetingStrategy
    {
        Nearest,
        LowestHealth,
        HighestHealth,
        Random,
        FirstInRange
    }

    [SerializeField] private TargetingStrategy currentStrategy = TargetingStrategy.Nearest;
    [SerializeField] private float detectionRange = 1500f;
    [SerializeField] private LayerMask enemyLayer;// = 1 << LayerMask.NameToLayer("Enemy");

    private Transform currentTarget;
    private List<Transform> enemiesInRange = new List<Transform>();

    void Update()
    {
        UpdateEnemiesInRange();

        if (enemiesInRange.Count > 0)
        {
            currentTarget = SelectTargetByStrategy();
        }
        else
        {
            currentTarget = null;
        }
    }

    private void UpdateEnemiesInRange()
    {
        if (enemiesInRange.Count > 0)
        {
            enemiesInRange.Clear();
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            transform.position,
            detectionRange,
            enemyLayer
        );

        foreach (var collider in colliders)
        {
            var npc = collider.GetComponent<BaseNPC>();
            if (npc != null && npc.IsAlive)
            {
                enemiesInRange.Add(collider.transform);
            }
        }
    }

    private Transform SelectTargetByStrategy()
    {
        if (enemiesInRange.Count == 0) return null;

        switch (currentStrategy)
        {
            case TargetingStrategy.Nearest:
                return GetNearestTarget();

            case TargetingStrategy.LowestHealth:
                return GetLowestHealthTarget();

            case TargetingStrategy.HighestHealth:
                return GetHighestHealthTarget();

            case TargetingStrategy.Random:
                return enemiesInRange[Random.Range(0, enemiesInRange.Count)];

            case TargetingStrategy.FirstInRange:
                return enemiesInRange[0];

            default:
                return enemiesInRange[0];
        }
    }

    private Transform GetNearestTarget()
    {
        Transform nearest = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var target in enemiesInRange)
        {
            float distance = Vector2.Distance(transform.position, target.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = target;
            }
        }

        return nearest;
    }

    private Transform GetLowestHealthTarget()
    {
        Transform lowestHealthTarget = null;
        int lowestHealth = int.MaxValue;

        foreach (var target in enemiesInRange)
        {
            var npc = target.GetComponent<BaseNPC>();
            if (npc != null && npc.GetCurrentHealth() < lowestHealth)
            {
                lowestHealth = npc.GetCurrentHealth();
                lowestHealthTarget = target;
            }
        }

        return lowestHealthTarget;
    }

    private Transform GetHighestHealthTarget()
    {
        Transform highestHealthTarget = null;
        int highestHealth = int.MinValue;

        foreach (var target in enemiesInRange)
        {
            var npc = target.GetComponent<BaseNPC>();
            if (npc != null && npc.GetCurrentHealth() > highestHealth)
            {
                highestHealth = npc.GetCurrentHealth();
                highestHealthTarget = target;
            }
        }

        return highestHealthTarget;
    }

    public void SetStrategy(TargetingStrategy newStrategy)
    {
        currentStrategy = newStrategy;
    }

    public void SetDetectionRange(float range)
    {
        detectionRange = range;
    }

    public bool HasTarget => currentTarget != null;
    public Transform CurrentTarget => currentTarget;
    public List<Transform> EnemiesInRange => enemiesInRange;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (currentTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }
}