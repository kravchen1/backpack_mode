using UnityEngine;

public class InvisibleTree : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Замените "Player" на тег вашего персонажа
        {
            foreach (var spriteRenderer in gameObject.transform.parent.GetComponentsInChildren<SpriteRenderer>())
            {
                Color color = spriteRenderer.color;
                spriteRenderer.color = new Color(color.r, color.g, color.b, spriteRenderer.color.a/2);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Замените "Player" на тег вашего персонажа
        {
            foreach (var spriteRenderer in gameObject.transform.parent.GetComponentsInChildren<SpriteRenderer>())
            {
                Color color = spriteRenderer.color;
                spriteRenderer.color = new Color(color.r, color.g, color.b, spriteRenderer.color.a*2);
            }
        }
    }
}
