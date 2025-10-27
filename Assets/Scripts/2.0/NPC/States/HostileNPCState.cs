using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostileNPCState : BaseNPCState
{
    #region Properties and Fields
    public override NPCStateType Type => NPCStateType.Hostile;

    private Coroutine chaseCoroutine;
    private Coroutine groupFollowCoroutine;
    private Coroutine patrolCoroutine;
    #endregion

    #region State Lifecycle Methods
    public override void EnterState(NPC npc)
    {
        base.EnterState(npc);
        npc.ChangeColor(npc.Config.hostileColor);
        npcController.SetSpeed(npc.Config.chaseSpeed);

        StartNeutralBehavior(npc);
    }

    public override void UpdateState(NPC npc)
    {
        // Поддерживаем нейтральное поведение, если не преследуем игрока
        if (chaseCoroutine == null && groupFollowCoroutine == null && patrolCoroutine == null)
        {
            StartNeutralBehavior(npc);
        }
    }

    public override void ExitState(NPC npc)
    {
        base.ExitState(npc);

        StopAllBehaviors(npc);
        npcController.ResetSpeed();
    }
    #endregion

    #region Player Interaction Methods
    public override void OnPlayerDetected(NPC npc, TopDownCharacterController player)
    {
        // Если в группе - только лидер начинает преследование
        if (npc.IsMoveGroup && !npc.IsLeader)
        {
            // Не-лидеры продолжают нейтральное поведение
            return;
        }

        // Останавливаем нейтральное поведение перед началом преследования
        StopNeutralBehavior(npc);

        // Лидер начинает преследование
        if (chaseCoroutine != null)
        {
            npc.StopCoroutine(chaseCoroutine);
        }
        chaseCoroutine = npc.StartCoroutine(ChasePlayerCoroutine(player));
    }

    public override void OnPlayerLost(NPC npc)
    {
        // Для не-лидеров в группе игнорируем потерю игрока
        if (npc.IsMoveGroup && !npc.IsLeader)
        {
            return;
        }

        // Останавливаем преследование
        if (chaseCoroutine != null)
        {
            npc.StopCoroutine(chaseCoroutine);
            chaseCoroutine = null;
        }

        // Возвращаемся к нейтральному поведению
        StartNeutralBehavior(npc);
    }
    #endregion

    #region Neutral Behavior Methods
    private void StartNeutralBehavior(NPC npc)
    {
        StopNeutralBehavior(npc);

        if (npc.ShouldFollowGroup())
        {
            StartGroupFollowing(npc);
        }
        else
        {
            StartPatrolBehavior(npc);
        }
    }

    private void StopNeutralBehavior(NPC npc)
    {
        StopGroupFollowing();
        StopPatrolBehavior(npc);
    }

    private void StopAllBehaviors(NPC npc)
    {
        if (chaseCoroutine != null)
        {
            npc.StopCoroutine(chaseCoroutine);
            chaseCoroutine = null;
        }

        StopNeutralBehavior(npc);
    }
    #endregion

    #region Group Behavior Methods
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
        StopPatrolBehavior(npc);

        if (npcController.WaypointContainer != null && npcController.WaypointContainer.GetWaypoints().Length > 0)
        {
            Vector3[] patrolPoints = npcController.WaypointContainer.GetWaypoints();
            patrolCoroutine = npc.StartCoroutine(PatrolCoroutine(patrolPoints));
        }
        else
        {
            Vector3[] patrolPoints = GeneratePatrolPoints(npc.transform.position, 10f, 4);
            patrolCoroutine = npc.StartCoroutine(PatrolCoroutine(patrolPoints));
        }
    }

    private void StopPatrolBehavior(NPC npc)
    {
        if (patrolCoroutine != null)
        {
            npc.StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
        }
        npcController.StopMovement();
    }

    private IEnumerator PatrolCoroutine(Vector3[] patrolPoints)
    {
        int currentPointIndex = 0;

        while (true)
        {
            Vector3 targetPosition = patrolPoints[currentPointIndex];
            Vector3 correctedPosition = npcController.GetMovementCorrectedPosition(targetPosition);

            npcController.MoveToPosition(correctedPosition);

            // Ждем достижения точки или таймаут
            yield return new WaitForSeconds(2f);

            // Переходим к следующей точке
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
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

    #region Chase Behavior Methods
    private IEnumerator ChasePlayerCoroutine(TopDownCharacterController player)
    {
        // Останавливаем нейтральное поведение на время преследования
        StopNeutralBehavior(npcController.GetComponent<NPC>());

        while (player != null && npcController.HasDetectedPlayer)
        {
            float distanceToPlayer = Vector2.Distance(npcController.transform.position, player.transform.position);

            if (distanceToPlayer > npcController.Config.attackRange)
            {
                Vector3 chasePosition = npcController.GetMovementCorrectedPosition(player.transform.position);
                npcController.MoveToPosition(chasePosition);
            }
            else
            {
                npcController.StopMovement();
                if (!npcController.InFightNow)
                {
                    if (!BattleManager.Instance.isBattleActive)
                    {
                        StartBattleWithPlayer();
                    }
                    else
                    {
                        AddBattleWithPlayer();
                    }
                    npcController.InFightNow = true;
                    yield break;
                }
            }

            yield return new WaitForSeconds(0.2f);
        }

        // После преследования возвращаемся к нейтральному поведению
        StartNeutralBehavior(npcController.GetComponent<NPC>());
    }
    #endregion

    #region Battle Methods
    private void StartBattleWithPlayer()
    {
        BattleStarter battleStarter = GameObject.FindAnyObjectByType<BattleStarter>();
        List<NPCDataManager> enemiesInThisEncounter = new List<NPCDataManager>();
        enemiesInThisEncounter.Add(npcDataManager);
        if (battleStarter != null)
            battleStarter.StartBattleOnCollision(enemiesInThisEncounter);
    }

    private void AddBattleWithPlayer()
    {
        BattleManager.Instance.AddEnemyToBattle(npcDataManager);
    }
    #endregion
}