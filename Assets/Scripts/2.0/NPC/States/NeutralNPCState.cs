// NeutralNPCState.cs
using System.Collections;
using UnityEngine;

public class NeutralNPCState : BaseNPCState
{
    public override NPCStateType Type => NPCStateType.Neutral;
    private Coroutine lookCoroutine;

    public override void EnterState(NPC npc)
    {
        base.EnterState(npc);
        npc.ChangeColor(npc.Config.neutralColor);
        npcController.SetSpeed(npc.Config.moveSpeed);
        StartPatrolBehavior(npc);
    }

    public override void OnPlayerDetected(NPC npc, TopDownCharacterController player)
    {
        npcController.StopMovement();
        //Debug.Log(" neutral OnPlayerDetected");
        // Отменяем предыдущую корутину если есть
        if (lookCoroutine != null)
        {
            npc.StopCoroutine(lookCoroutine);
        }

        lookCoroutine = npc.StartCoroutine(LookAtPlayerRoutine(npc, player));
    }

    private IEnumerator LookAtPlayerRoutine(NPC npc, TopDownCharacterController player)
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

    public override void OnPlayerLost(NPC npc)
    {
        if (lookCoroutine != null)
        {
            npc.StopCoroutine(lookCoroutine);
            lookCoroutine = null;
        }

        npc.AnimationController?.CancelForcedLook();
        StartPatrolBehavior(npc);
    }

    public override void ExitState(NPC npc)
    {
        if (lookCoroutine != null)
        {
            npc.StopCoroutine(lookCoroutine);
            lookCoroutine = null;
        }

        npc.AnimationController?.CancelForcedLook();
        npcController?.StopMovement();
    }

    private void StartPatrolBehavior(NPC npc)
    {
        if (npcController.WaypointContainer != null && npcController.WaypointContainer.GetWaypoints().Length > 0)
        {
            Vector3[] patrolPoints = npcController.WaypointContainer.GetWaypoints();
            npcController.StartPatrol(patrolPoints);
        }
        else
        {
            Vector3[] patrolPoints = GeneratePatrolPoints(npc.transform.position, 10f, 4);
            npcController.StartPatrol(patrolPoints);
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