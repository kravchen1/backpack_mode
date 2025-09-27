using System.Collections;
using UnityEngine;

public class HostileNPCState : BaseNPCState
{
    public override NPCStateType Type => NPCStateType.Hostile;
    private Coroutine chaseCoroutine;

    public override void EnterState(NPCController npc)
    {
        base.EnterState(npc);
        npc.ChangeColor(npc.Config.hostileColor);
        navigationAgent.SetSpeed(npc.Config.chaseSpeed);
    }

    public override void OnPlayerDetected(NPCController npc, TopDownCharacterController player)
    {
        chaseCoroutine = npc.StartCoroutine(ChasePlayerCoroutine(player));
    }

    public override void OnPlayerLost(NPCController npc)
    {
        if (chaseCoroutine != null)
        {
            npc.StopCoroutine(chaseCoroutine);
            chaseCoroutine = null;
        }
        navigationAgent.StopMovement();
        // Можно добавить поиск игрока или возврат на пост
    }

    public override void ExitState(NPCController npc)
    {
        base.ExitState(npc);
        if (chaseCoroutine != null)
        {
            npc.StopCoroutine(chaseCoroutine);
        }
        navigationAgent.ResetSpeed();
    }

    private IEnumerator ChasePlayerCoroutine(TopDownCharacterController player)
    {
        while (player != null && npcController.HasDetectedPlayer)
        {
            float distanceToPlayer = Vector2.Distance(npcController.transform.position, player.transform.position);

            if (distanceToPlayer > npcController.Config.attackRange)
            {
                // Преследуем игрока
                navigationAgent.MoveToPosition(player.transform.position, npcController.Config.chaseStoppingDistance);
            }
            else
            {
                // Атакуем игрока (останавливаемся на расстоянии атаки)
                navigationAgent.StopMovement();
                // npcController.AttackPlayer(player);
            }

            yield return new WaitForSeconds(0.2f); // Оптимизация частоты обновления
        }
    }
}