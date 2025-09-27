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
    void EnterState(NPCController npc);
    void UpdateState(NPCController npc);
    void ExitState(NPCController npc);
    void OnPlayerDetected(NPCController npc, TopDownCharacterController player);
    void OnPlayerLost(NPCController npc);
}