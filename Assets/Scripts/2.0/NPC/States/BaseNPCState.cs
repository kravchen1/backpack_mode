using UnityEngine;
public abstract class BaseNPCState : INPCState
{
    public abstract NPCStateType Type { get; }

    protected NPC npcController;
    protected NPCDataManager npcDataManager;

    public virtual void EnterState(NPC npc)
    {
        npcController = npc;
        npcDataManager = npc.GetComponent<NPCDataManager>();
        //Debug.Log($"{npc.name} перешел в состояние: {Type}");
    }

    public virtual void UpdateState(NPC npc) { }

    public virtual void ExitState(NPC npc)
    {
        npcController.StopMovement();
    }

    public virtual void OnPlayerDetected(NPC npc, TopDownCharacterController player) { }

    public virtual void OnPlayerLost(NPC npc) { }
}