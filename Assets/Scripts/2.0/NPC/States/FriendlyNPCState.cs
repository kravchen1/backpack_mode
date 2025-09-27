using System.Collections;
using UnityEngine;
public class FriendlyNPCState : BaseNPCState
{
    public override NPCStateType Type => NPCStateType.Friendly;

    public override void EnterState(NPCController npc)
    {
        base.EnterState(npc);
        npc.ChangeColor(npc.Config.friendlyColor);

        // ��������� ��������� - ��������� ���������
        StartWandering(npc);
    }

    public override void OnPlayerDetected(NPCController npc, TopDownCharacterController player)
    {
        // �������� � ������ ��� ��������������
        navigationAgent.MoveToPosition(player.transform.position, npc.Config.interactionRadius);
    }

    private void StartWandering(NPCController npc)
    {
        npc.StartCoroutine(WanderCoroutine());
    }

    private IEnumerator WanderCoroutine()
    {
        while (true)
        {
            // ��������� ����� � ������� 5 ������
            Vector2 randomPoint = (Vector2)npcController.transform.position + Random.insideUnitCircle * 5f;
            navigationAgent.MoveToPosition(randomPoint);

            // ���� ���������� ����� ��� ��������� �����
            yield return new WaitForSeconds(Random.Range(3f, 8f));
        }
    }
}