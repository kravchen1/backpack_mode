using UnityEngine;

public class InvisibleObject : MonoBehaviour
{
    private string sortingLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Замените "Player" на тег вашего персонажа
        {
            sortingLayer = GetComponent<SpriteRenderer>().sortingLayerName;
            //Debug.Log(sortingLayer);
            //foreach (var spriteRenderer in gameObject.transform.parent.GetComponentsInChildren<SpriteRenderer>())
            //{
            var spriteRenderer = GetComponent<SpriteRenderer>();
            //spriteRenderer.sortingLayerName = "Tree";
            Color color = spriteRenderer.color;
            //Debug.Log(color.ToString()); 
            spriteRenderer.color = new Color(color.r, color.g, color.b, color.a/2);
            //}
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Замените "Player" на тег вашего персонажа
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            //GetComponent<SpriteRenderer>().sortingLayerName = sortingLayer;
            Color color = spriteRenderer.color;
            //Debug.Log(color.ToString());
            spriteRenderer.color = new Color(color.r, color.g, color.b, color.a*2);
        }
    }
}
