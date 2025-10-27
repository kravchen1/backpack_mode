using System.Collections;
using UnityEngine;

public class FriendlyNPCState : BaseNPCState
{
    #region Properties and Fields
    public override NPCStateType Type => NPCStateType.Friendly;

    private Coroutine followCoroutine;
    private Coroutine wanderCoroutine;
    private Coroutine groupFollowCoroutine;
    private bool isFollowingPlayer = false;
    #endregion

    #region State Lifecycle Methods
    public override void EnterState(NPC npc)
    {
        base.EnterState(npc);
        npc.ChangeColor(npc.Config.friendlyColor);

        // Подписываемся на событие начала боя
        BattleManager.Instance.OnBattleStart += OnBattleStart;

        // Начинаем поведение с учетом группы
        StartGroupAwareBehavior(npc);
    }

    public override void UpdateState(NPC npc)
    {
        // Для лидеров группы - проверяем игрока
        if ((!npc.IsMoveGroup || npc.IsLeader) && isFollowingPlayer && npcController.HasDetectedPlayer)
        {
            CheckPlayerInMovementArea();
        }

        // Для всех - проверяем групповое поведение
        if (npc.ShouldFollowGroup() && groupFollowCoroutine == null)
        {
            StartGroupFollowing(npc);
        }
    }

    public override void ExitState(NPC npc)
    {
        base.ExitState(npc);

        // Отписываемся от событий
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnBattleStart -= OnBattleStart;
        }

        // Останавливаем все корутины
        StopCurrentBehavior();
        StopDefaultBehavior();
    }
    #endregion

    #region Player Interaction Methods
    public override void OnPlayerDetected(NPC npc, TopDownCharacterController player)
    {
        // Если в группе - лидер решает, следовать ли за игроком
        if (npc.IsMoveGroup && !npc.IsLeader)
        {
            // Не-лидеры в группе следуют только за лидером, а не за игроком
            return;
        }

        // Останавливаем текущее поведение
        StopCurrentBehavior();

        // Начинаем следовать за игроком
        StartFollowingPlayer(player);
    }

    public override void OnPlayerLost(NPC npc)
    {
        // Для не-лидеров в группе игнорируем потерю игрока
        if (npc.IsMoveGroup && !npc.IsLeader)
        {
            return;
        }

        // Игрок вышел из зоны обнаружения - прекращаем следование
        StopFollowingPlayer();

        // Возвращаемся к поведению с учетом группы
        StartGroupAwareBehavior(npc);
    }
    #endregion

    #region Behavior Management Methods
    private void StopCurrentBehavior()
    {
        StopFollowingPlayer();
        StopDefaultBehavior();
        StopGroupFollowing();
    }

    private void StartGroupAwareBehavior(NPC npc)
    {
        // Если в группе и не лидер - следуем за лидером
        if (npc.ShouldFollowGroup())
        {
            StartGroupFollowing(npc);
        }
        else
        {
            // Иначе стандартное поведение
            StartDefaultBehavior(npc);
        }
    }

    private void StartDefaultBehavior(NPC npc)
    {
        if (npcController.WaypointContainer != null && npcController.WaypointContainer.GetWaypoints().Length > 0)
        {
            // Если есть waypoints - начинаем патрулирование
            StartPatrolBehavior(npc);
        }
        else
        {
            // Если нет waypoints - случайное блуждание
            StartWandering(npc);
        }
    }

    private void StopDefaultBehavior()
    {
        if (wanderCoroutine != null)
        {
            npcController.StopCoroutine(wanderCoroutine);
            wanderCoroutine = null;
        }
        npcController.StopMovement();
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

            npc.MoveToPosition(correctedPosition, 1.5f);
            yield return new WaitForSeconds(0.5f);
        }

        // Если вышли из группового поведения - возвращаемся к стандартному
        if (!npc.ShouldFollowGroup())
        {
            StartDefaultBehavior(npc);
        }
    }
    #endregion

    #region Patrol and Wander Methods
    private void StartPatrolBehavior(NPC npc)
    {
        Vector3[] patrolPoints = npcController.WaypointContainer.GetWaypoints();
        npcController.StartPatrol(patrolPoints);
    }

    private void StartWandering(NPC npc)
    {
        wanderCoroutine = npc.StartCoroutine(WanderCoroutine());
    }

    private IEnumerator WanderCoroutine()
    {
        while (true)
        {
            // Генерируем случайную точку в пределах области движения
            Vector2 randomPoint = GetRandomPointInMovementArea();
            npcController.MoveToPosition(randomPoint);

            // Ждем достижения точки или случайное время
            yield return new WaitForSeconds(Random.Range(3f, 8f));
        }
    }

    private Vector2 GetRandomPointInMovementArea()
    {
        if (npcController.AreaMovementContainer != null)
        {
            Vector3[] bounds = npcController.AreaMovementContainer.GetAreaBounds();
            if (bounds.Length > 0)
            {
                // Простая реализация - случайная точка внутри bounding box
                Bounds areaBounds = new Bounds(bounds[0], Vector3.zero);
                foreach (Vector3 point in bounds) areaBounds.Encapsulate(point);

                Vector3 randomPoint = new Vector3(
                    Random.Range(areaBounds.min.x, areaBounds.max.x),
                    Random.Range(areaBounds.min.y, areaBounds.max.y),
                    0
                );

                // Уточняем точку чтобы она была точно внутри полигона
                return npcController.AreaMovementContainer.GetClosestPointInArea(randomPoint);
            }
        }

        // Fallback: случайная точка в радиусе 5 метров
        return (Vector2)npcController.transform.position + Random.insideUnitCircle * 5f;
    }
    #endregion

    #region Player Following Methods
    private void StartFollowingPlayer(TopDownCharacterController player)
    {
        isFollowingPlayer = true;
        if (followCoroutine != null)
        {
            npcController.StopCoroutine(followCoroutine);
        }
        followCoroutine = npcController.StartCoroutine(FollowPlayerCoroutine(player));
    }

    private void StopFollowingPlayer()
    {
        isFollowingPlayer = false;
        if (followCoroutine != null)
        {
            npcController.StopCoroutine(followCoroutine);
            followCoroutine = null;
        }
    }

    private IEnumerator FollowPlayerCoroutine(TopDownCharacterController player)
    {
        //Debug.Log($"{npcController.Config.NPCName}: Начало следования за игроком");

        int iteration = 0;
        while (player != null && isFollowingPlayer)
        {
            iteration++;
            //Debug.Log($"{npcController.Config.NPCName}: Итерация {iteration}, isFollowingPlayer: {isFollowingPlayer}");

            Vector3 followPosition = player.transform.position;
            followPosition = npcController.GetMovementCorrectedPosition(followPosition);
            npcController.MoveToPosition(followPosition, npcController.Config.interactionRadius);

            //Debug.Log($"{npcController.Config.NPCName}: Движение к позиции {followPosition}");

            yield return new WaitForSeconds(0.3f);

            // Проверяем не был ли флаг сброшен во время ожидания
            if (!isFollowingPlayer)
            {
                //Debug.Log($"{npcController.Config.NPCName}: Флаг isFollowingPlayer сброшен во время ожидания!");
                break;
            }
        }

        //Debug.Log($"{npcController.Config.NPCName}: Конец следования. Итераций: {iteration}");
        isFollowingPlayer = false;
    }

    // Проверка, находится ли игрок в разрешенной зоне движения
    private void CheckPlayerInMovementArea()
    {
        if (npcController.DetectedPlayer != null && npcController.AreaMovementContainer != null)
        {
            Vector3 playerPosition = npcController.DetectedPlayer.transform.position;

            // Если игрок вышел за границы зоны движения
            if (!npcController.IsPositionInMovementArea(playerPosition))
            {
                Debug.Log($"{npcController.Config.NPCName}: Игрок вышел за зону движения, прекращаю следование");

                ForceLosePlayer();
            }
        }
    }

    public void ForceLosePlayer()
    {
        if (npcController != null)
        {
            StopFollowingPlayer();
            StartDefaultBehavior(npcController);
            // Сбрасываем обнаружение игрока
            npcController.DetectedPlayer = null;
        }
    }
    #endregion

    #region Battle Support Methods
    // Обработчик начала боя
    private void OnBattleStart()
    {
        if (npcController != null && npcController.HasDetectedPlayer && isFollowingPlayer)
        {
            // Дружественный NPC присоединяется к бою на стороне игрока
            JoinPlayerBattle();
        }
    }

    private void JoinPlayerBattle()
    {
        if (BattleManager.Instance != null && npcDataManager != null)
        {
            // Добавляем дружественного NPC в команду игрока
            if (!npcController.InFightNow)
            {
                BattleManager.Instance.AddFriendToBattle(npcDataManager);
                npcController.InFightNow = true;
            }
            // Временно меняем поведение на боевое
            npcController.StartCoroutine(CombatSupportBehavior());
        }
    }

    private IEnumerator CombatSupportBehavior()
    {
        // Сохраняем состояние следования
        bool wasFollowing = isFollowingPlayer;

        // Прекращаем обычное следование на время боя
        StopFollowingPlayer();

        // Во время боя дружественный NPC занимает позицию рядом с игроком
        while (BattleManager.Instance.isBattleActive && npcController.HasDetectedPlayer)
        {
            if (npcController.DetectedPlayer != null)
            {
                // Занимаем фланговую позицию относительно игрока
                Vector3 supportPosition = GetSupportPosition();
                npcController.MoveToPosition(supportPosition, 2f);
            }
            yield return new WaitForSeconds(1f);
        }

        // После боя возвращаемся к предыдущему поведению
        if (npcController.HasDetectedPlayer && wasFollowing)
        {
            // Продолжаем следовать за игроком
            StartFollowingPlayer(npcController.DetectedPlayer);
        }
        else if (npcController.HasDetectedPlayer)
        {
            // Игрок в зоне, но не следовали ранее - начинаем следовать
            OnPlayerDetected(npcController, npcController.DetectedPlayer);
        }
        else
        {
            // Игрок ушел - возвращаемся к стандартному поведению
            OnPlayerLost(npcController);
        }
    }

    private Vector3 GetSupportPosition()
    {
        if (npcController.DetectedPlayer == null)
            return npcController.transform.position;

        // Позиция сбоку от игрока на случайном фланге
        Vector3 playerPosition = npcController.DetectedPlayer.transform.position;
        Vector3 randomOffset = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0);
        Vector3 supportPosition = playerPosition + randomOffset;

        // Корректируем позицию чтобы оставаться в области движения
        return npcController.GetMovementCorrectedPosition(supportPosition);
    }
    #endregion
}