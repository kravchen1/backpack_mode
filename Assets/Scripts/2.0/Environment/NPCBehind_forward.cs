using System.Collections.Generic;
using UnityEngine;

public class NPCBehind_forward : MonoBehaviour
{
    public List<SpriteRenderer> renderers = new List<SpriteRenderer>();
    void OnTriggerEnter2D(Collider2D other)
    {
        foreach (var spriteRenderer in renderers)
        {
            spriteRenderer.GetComponent<SpriteRenderer>().sortingLayerName = "MapEnvironmentForwardCharacter";
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        foreach (var spriteRenderer in renderers)
        {
            spriteRenderer.GetComponent<SpriteRenderer>().sortingLayerName = "MapEnvironmentBehindCharacter";
        }
    }
}
