// NPCState.cs
public enum NPCStateType
{
    Hostile,    // Враждебный
    Neutral,    // Нейтральный
    Friendly    // Дружественный
}

// Базовый интерфейс состояния NPC
public interface INPCState
{
    NPCStateType Type { get; }
    void EnterState(NPC npc);
    void UpdateState(NPC npc);
    void ExitState(NPC npc);
    void OnPlayerDetected(NPC npc, TopDownCharacterController player);
    void OnPlayerLost(NPC npc);
}