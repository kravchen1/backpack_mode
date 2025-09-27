// NeutralNPCState.cs
using System.Collections;
using UnityEngine;

public class NeutralNPCState : BaseNPCState
{
    public override NPCStateType Type => NPCStateType.Neutral;
    private WaypointContainer waypointContainer;
    private Coroutine lookCoroutine;

    public override void EnterState(NPCController npc)
    {
        base.EnterState(npc);
        npc.ChangeColor(npc.Config.neutralColor);

        waypointContainer = npc.GetComponentInChildren<WaypointContainer>();
        StartPatrolBehavior(npc);
    }

    public override void OnPlayerDetected(NPCController npc, TopDownCharacterController player)
    {
        navigationAgent.StopMovement();

        // Отменяем предыдущую корутину если есть
        if (lookCoroutine != null)
        {
            npc.StopCoroutine(lookCoroutine);
        }

        lookCoroutine = npc.StartCoroutine(LookAtPlayerRoutine(npc, player));
    }

    private IEnumerator LookAtPlayerRoutine(NPCController npc, TopDownCharacterController player)
    {
        // Смотрим на игрока пока он в зоне detection
        while (player != null && npc.HasDetectedPlayer)
        {
            Vector2 direction = (player.transform.position - npc.transform.position).normalized;
            npc.ForceLookAt(direction, 0.5f); // Короткий duration, будет обновляться каждый кадр

            yield return new WaitForSeconds(0.1f); // Обновляем направление каждые 0.1 сек
        }

        // Игрок ушел, возвращаемся к патрулированию
        npc.AnimationController?.CancelForcedLook();
        StartPatrolBehavior(npc);
    }

    public override void OnPlayerLost(NPCController npc)
    {
        if (lookCoroutine != null)
        {
            npc.StopCoroutine(lookCoroutine);
            lookCoroutine = null;
        }

        npc.AnimationController?.CancelForcedLook();
        StartPatrolBehavior(npc);
    }

    public override void ExitState(NPCController npc)
    {
        if (lookCoroutine != null)
        {
            npc.StopCoroutine(lookCoroutine);
            lookCoroutine = null;
        }

        npc.AnimationController?.CancelForcedLook();
        navigationAgent?.StopMovement();
    }

    private void StartPatrolBehavior(NPCController npc)
    {
        if (waypointContainer != null && waypointContainer.GetWaypoints().Length > 0)
        {
            Vector3[] patrolPoints = waypointContainer.GetWaypoints();
            navigationAgent.StartPatrol(patrolPoints);
        }
        else
        {
            Vector3[] patrolPoints = GeneratePatrolPoints(npc.transform.position, 3f, 4);
            navigationAgent.StartPatrol(patrolPoints);
        }
    }

    private Vector3[] GeneratePatrolPoints(Vector3 center, float radius, int count)
    {
        Vector3[] points = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            float angle = i * (360f / count);
            Vector3 point = center + Quaternion.Euler(0, 0, angle) * Vector3.right * radius;
            points[i] = point;
        }
        return points;
    }
}