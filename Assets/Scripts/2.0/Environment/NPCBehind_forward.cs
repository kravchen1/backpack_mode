using System.Collections.Generic;
using UnityEngine;

public class NPCBehind_forward : MonoBehaviour
{
    public List<SpriteRenderer> renderers = new List<SpriteRenderer>();

    [SerializeField] private LayerMask _playerLayerMask = 1 << 6;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(!IsPlayerObject(other)) return;

        foreach (var spriteRenderer in renderers)
        {
            spriteRenderer.GetComponent<SpriteRenderer>().sortingLayerName = "MapEnvironmentForwardCharacter";
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!IsPlayerObject(other)) return;

        foreach (var spriteRenderer in renderers)
        {
            spriteRenderer.GetComponent<SpriteRenderer>().sortingLayerName = "MapEnvironmentBehindCharacter";
        }
    }

    private bool IsPlayerObject(Collider2D collider)
    {
        return ((1 << collider.gameObject.layer) & _playerLayerMask) != 0;
    }
}
