// NPCState.cs
public enum NPCStateType
{
    Hostile,    // ����������
    Neutral,    // �����������
    Friendly    // �������������
}

// ������� ��������� ��������� NPC
public interface INPCState
{
    NPCStateType Type { get; }
    void EnterState(NPCController npc);
    void UpdateState(NPCController npc);
    void ExitState(NPCController npc);
    void OnPlayerDetected(NPCController npc, TopDownCharacterController player);
    void OnPlayerLost(NPCController npc);
}