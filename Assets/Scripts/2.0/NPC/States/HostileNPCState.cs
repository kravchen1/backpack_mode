using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostileNPCState : BaseNPCState
{
    public override NPCStateType Type => NPCStateType.Hostile;
    private Coroutine chaseCoroutine;

    public override void EnterState(NPC npc)
    {
        base.EnterState(npc);
        npc.ChangeColor(npc.Config.hostileColor);
        npcController.SetSpeed(npc.Config.chaseSpeed);
    }

    public override void OnPlayerDetected(NPC npc, TopDownCharacterController player)
    {
        chaseCoroutine = npc.StartCoroutine(ChasePlayerCoroutine(player));
    }

    public override void OnPlayerLost(NPC npc)
    {
        if (chaseCoroutine != null)
        {
            npc.StopCoroutine(chaseCoroutine);
            chaseCoroutine = null;
        }
        npcController.StopMovement();
        // Можно добавить поиск игрока или возврат на пост
    }

    public override void ExitState(NPC npc)
    {
        base.ExitState(npc);
        if (chaseCoroutine != null)
        {
            npc.StopCoroutine(chaseCoroutine);
        }
        npcController.ResetSpeed();
    }

    private IEnumerator ChasePlayerCoroutine(TopDownCharacterController player)
    {
        while (player != null && npcController.HasDetectedPlayer)
        {
            float distanceToPlayer = Vector2.Distance(npcController.transform.position, player.transform.position);

            if (distanceToPlayer > npcController.Config.attackRange)
            {
                // Преследуем игрока
                npcController.MoveToPosition(player.transform.position, npcController.Config.chaseStoppingDistance);
            }
            else
            {
                // Атакуем игрока (останавливаемся на расстоянии атаки)
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
                }
            }

            yield return new WaitForSeconds(0.2f); // Оптимизация частоты обновления
        }
    }

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
}