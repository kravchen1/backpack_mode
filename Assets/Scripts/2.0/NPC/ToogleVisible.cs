using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class VisibilityController : MonoBehaviour
{
    [SerializeField] private LayerMask _npcLayerMask = 1 << 11;
    [SerializeField] private LayerMask _treeLayerMask = 1 << 25;

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleObjectEnter(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        HandleObjectExit(other);
    }

    private void HandleObjectEnter(Collider2D other)
    {
        // Обрабатываем NPC
        if (IsNPCObject(other))
        {
            NPC npc = other.GetComponent<NPC>();
            if (npc != null)
            {
                npc.EnableVisualComponents();
                return;
            }
        }

        // Обрабатываем деревья
        if (IsTreeObject(other))
        {
            Tree tree = other.GetComponent<Tree>();
            if (tree != null)
            {
                tree.EnableVisualComponents();
                return;
            }
        }
    }

    private void HandleObjectExit(Collider2D other)
    {
        // Обрабатываем NPC
        if (IsNPCObject(other))
        {
            NPC npc = other.GetComponent<NPC>();
            if (npc != null)
            {
                npc.DisableVisualComponents();
                return;
            }
        }

        // Обрабатываем деревья
        if (IsTreeObject(other))
        {
            Tree tree = other.GetComponent<Tree>();
            if (tree != null)
            {
                tree.DisableVisualComponents();
                return;
            }
        }
    }

    private bool IsNPCObject(Collider2D collider)
    {
        return ((1 << collider.gameObject.layer) & _npcLayerMask) != 0;
    }

    private bool IsTreeObject(Collider2D collider)
    {
        return ((1 << collider.gameObject.layer) & _treeLayerMask) != 0;
    }
}