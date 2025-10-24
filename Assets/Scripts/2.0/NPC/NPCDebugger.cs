// NPCDebugger.cs
using UnityEngine;

public class NPCDebugger : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool showDetectionRadius = true;
    public bool showCurrentState = true;
    public bool showPath = true;

    private NPC npcController;

    void Start()
    {
        npcController = GetComponent<NPC>();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        if (npcController != null && showDetectionRadius)
        {
            // Радиус детекции
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, npcController.Config.detectionRadius);

            // Радиус взаимодействия
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, npcController.Config.interactionRadius);
        }

        if (showCurrentState && npcController != null)
        {
            // Текст состояния над NPC
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2f,
                //$"State: {npcController.CurrentState}\n" +
                $"Speed: {npcController.navMeshAgent.speed:F1}");
#endif
        }
    }
}