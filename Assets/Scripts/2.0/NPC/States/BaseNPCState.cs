using UnityEngine;
public abstract class BaseNPCState : INPCState
{
    public abstract NPCStateType Type { get; }

    protected NPCNavigationAgent navigationAgent;
    protected NPCController npcController;

    public virtual void EnterState(NPCController npc)
    {
        npcController = npc;
        navigationAgent = npc.GetComponent<NPCNavigationAgent>();
        Debug.Log($"{npc.name} перешел в состояние: {Type}");
    }

    public virtual void UpdateState(NPCController npc) { }

    public virtual void ExitState(NPCController npc)
    {
        navigationAgent?.StopMovement();
    }

    public virtual void OnPlayerDetected(NPCController npc, TopDownCharacterController player) { }

    public virtual void OnPlayerLost(NPCController npc) { }
}