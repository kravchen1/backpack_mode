using UnityEngine;

public class NPCBack_NPCFront : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("in" + other.gameObject);
        if (other.CompareTag("Player")) // Замените "Player" на тег вашего персонажа
        {
            foreach (var spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderer.GetComponent<SpriteRenderer>().sortingLayerName = "NPCFront";
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        //Debug.Log("out" + other.gameObject);
        foreach (var spriteRenderer in gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.GetComponent<SpriteRenderer>().sortingLayerName = "NPCBack";
        }
    }
}
