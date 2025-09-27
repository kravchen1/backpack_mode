using System.Linq;
using UnityEngine;

public class NPCReactionSystem : MonoBehaviour
{
    [SerializeField] private float reactionCheckInterval = 0.5f;

    private NPCController[] allNPCs;
    private TopDownCharacterController player;

    void Start()
    {
        player = FindObjectOfType<TopDownCharacterController>();
        allNPCs = FindObjectsOfType<NPCController>();

        InvokeRepeating(nameof(CheckNPCReactions), 0f, reactionCheckInterval);
    }

    private void CheckNPCReactions()
    {
        if (player == null) return;

        foreach (var npc in allNPCs)
        {
            // Динамически изменяем состояния на основе условий игры
            // Например: репутация игрока, время суток, квесты и т.д.
            AdjustNPCStateBasedOnConditions(npc);
        }
    }

    private void AdjustNPCStateBasedOnConditions(NPCController npc)
    {
        // Пример: если игрок атаковал NPC, сделать его враждебным
        // Или если выполнен квест, сделать дружественным
    }

    // Public API для внешних систем
    public void SetNPCsHostileInRadius(Vector3 position, float radius)
    {
        var npcsInRadius = allNPCs.Where(npc =>
            Vector3.Distance(npc.transform.position, position) <= radius);

        foreach (var npc in npcsInRadius)
        {
            npc.MakeHostile();
        }
    }
}