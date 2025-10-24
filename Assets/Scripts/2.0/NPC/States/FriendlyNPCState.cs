using System.Collections;
using UnityEngine;
public class FriendlyNPCState : BaseNPCState
{
    public override NPCStateType Type => NPCStateType.Friendly;

    public override void EnterState(NPC npc)
    {
        base.EnterState(npc);
        npc.ChangeColor(npc.Config.friendlyColor);

        // Спокойное поведение - случайное блуждание
        StartWandering(npc);
    }

    public override void OnPlayerDetected(NPC npc, TopDownCharacterController player)
    {
        // Подходим к игроку для взаимодействия
        npcController.MoveToPosition(player.transform.position, npc.Config.interactionRadius);
    }

    private void StartWandering(NPC npc)
    {
        npc.StartCoroutine(WanderCoroutine());
    }

    private IEnumerator WanderCoroutine()
    {
        while (true)
        {
            // Случайная точка в радиусе 5 метров
            Vector2 randomPoint = (Vector2)npcController.transform.position + Random.insideUnitCircle * 5f;
            npcController.MoveToPosition(randomPoint);

            // Ждем достижения точки или случайное время
            yield return new WaitForSeconds(Random.Range(3f, 8f));
        }
    }
}