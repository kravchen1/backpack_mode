using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class VisibilityController : MonoBehaviour
{
    [SerializeField] private LayerMask _npcLayerMask = 1 << 11;

    // Компоненты, которые будут включаться/выключаться
    private const int VISUAL_COMPONENTS_START_INDEX = 0;
    private const int VISUAL_COMPONENTS_COUNT = 5;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsNPCObject(other)) return;

        EnableNPCComponents(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsNPCObject(other)) return;

        DisableNPCComponents(other.gameObject);
    }

    private bool IsNPCObject(Collider2D collider)
    {
        return ((1 << collider.gameObject.layer) & _npcLayerMask) != 0;
    }

    private void EnableNPCComponents(GameObject npcObject)
    {
        SetNPCComponentsState(npcObject, true);
    }

    private void DisableNPCComponents(GameObject npcObject)
    {
        SetNPCComponentsState(npcObject, false);
    }

    private void SetNPCComponentsState(GameObject npcObject, bool isEnabled)
    {
        // Включаем/выключаем компоненты
        SetComponentState<NPCAnimationController>(npcObject, isEnabled);
        SetComponentState<Animator>(npcObject, isEnabled);

        // Включаем/выключаем дочерние объекты
        SetChildObjectsState(npcObject.transform, isEnabled);
    }

    private void SetComponentState<T>(GameObject targetObject, bool isEnabled) where T : Behaviour
    {
        var component = targetObject.GetComponent<T>();
        if (component != null)
        {
            component.enabled = isEnabled;
        }
        else
        {
            Debug.LogWarning($"Component {typeof(T).Name} not found on {targetObject.name}", targetObject);
        }
    }

    private void SetChildObjectsState(Transform parent, bool isEnabled)
    {
        for (int i = VISUAL_COMPONENTS_START_INDEX; i < VISUAL_COMPONENTS_START_INDEX + VISUAL_COMPONENTS_COUNT; i++)
        {
            if (i < parent.childCount)
            {
                parent.GetChild(i).gameObject.SetActive(isEnabled);
            }
            else
            {
                Debug.LogWarning($"Child index {i} is out of range for {parent.name}", parent.gameObject);
                break;
            }
        }
    }
}