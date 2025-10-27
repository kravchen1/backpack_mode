// NeutralNPCState.cs
using System.Collections;
using UnityEngine;

public class NeutralNPCState : BaseNPCState
{
    #region Properties and Fields
    public override NPCStateType Type => NPCStateType.Neutral;

    private Coroutine lookCoroutine;
    private Coroutine groupFollowCoroutine;
    #endregion

    #region State Lifecycle Methods
    public override void EnterState(NPC npc)
    {
        base.EnterState(npc);
        npc.ChangeColor(npc.Config.neutralColor);
        npcController.SetSpeed(npc.Config.moveSpeed);

        StartGroupAwareBehavior(npc);
    }

    public override void UpdateState(NPC npc)
    {
        // Поддерживаем групповое поведение если нужно
        if (npc.ShouldFollowGroup() && groupFollowCoroutine == null && lookCoroutine == null)
        {
            StartGroupFollowing(npc);
        }
    }

    public override void ExitState(NPC npc)
    {
        if (lookCoroutine != null)
        {
            npc.StopCoroutine(lookCoroutine);
            lookCoroutine = null;
        }

        StopGroupFollowing();
        npc.AnimationController?.CancelForcedLook();
        npcController?.StopMovement();
    }
    #endregion

    #region Player Interaction Methods
    public override void OnPlayerDetected(NPC npc, TopDownCharacterController player)
    {
        // Для не-лидеров в группе - продолжаем следовать за лидером
        if (npc.IsMoveGroup && !npc.IsLeader)
        {
            return;
        }

        // Останавливаем групповое поведение на время наблюдения
        StopGroupFollowing();

        npcController.StopMovement();

        if (lookCoroutine != null)
        {
            npc.StopCoroutine(lookCoroutine);
        }

        lookCoroutine = npc.StartCoroutine(LookAtPlayerRoutine(npc, player));
    }

    public override void OnPlayerLost(NPC npc)
    {
        // Для не-лидеров в группе игнорируем потерю игрока
        if (npc.IsMoveGroup && !npc.IsLeader)
        {
            return;
        }

        if (lookCoroutine != null)
        {
            npc.StopCoroutine(lookCoroutine);
            lookCoroutine = null;
        }

        npc.AnimationController?.CancelForcedLook();
        StartGroupAwareBehavior(npc);
    }
    #endregion

    #region Group Behavior Methods
    private void StartGroupAwareBehavior(NPC npc)
    {
        if (npc.ShouldFollowGroup())
        {
            StartGroupFollowing(npc);
        }
        else
        {
            StartPatrolBehavior(npc);
        }
    }

    private void StartGroupFollowing(NPC npc)
    {
        StopGroupFollowing();
        groupFollowCoroutine = npc.StartCoroutine(GroupFollowCoroutine(npc));
    }

    private void StopGroupFollowing()
    {
        if (groupFollowCoroutine != null)
        {
            npcController.StopCoroutine(groupFollowCoroutine);
            groupFollowCoroutine = null;
        }
    }

    private IEnumerator GroupFollowCoroutine(NPC npc)
    {
        while (npc.ShouldFollowGroup())
        {
            Vector3 groupPosition = npc.GetGroupFormationPosition();
            Vector3 correctedPosition = npc.GetMovementCorrectedPosition(groupPosition);

            npc.MoveToPosition(correctedPosition, 2f);
            yield return new WaitForSeconds(0.5f);
        }
    }
    #endregion

    #region Patrol Behavior Methods
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
    #endregion

    #region Coroutines
    private IEnumerator LookAtPlayerRoutine(NPC npc, TopDownCharacterController player)
    {
        while (player != null && npc.HasDetectedPlayer)
        {
            Vector2 direction = (player.transform.position - npc.transform.position).normalized;
            npc.ForceLookAt(direction, 0.5f);
            yield return new WaitForSeconds(0.1f);
        }

        npc.AnimationController?.CancelForcedLook();
        StartGroupAwareBehavior(npc);
    }
    #endregion
}