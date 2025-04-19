using UnityEngine;

public class InvisibleTree : MonoBehaviour
{
    private string sortingLayer;

    public bool needToSetSortTree = false;
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
            foreach (var spriteRenderer in gameObject.transform.parent.GetComponentsInChildren<SpriteRenderer>())
            {
                if(needToSetSortTree)
                    GetComponent<SpriteRenderer>().sortingLayerName = "Tree";
                Color color = spriteRenderer.color;
                //Debug.Log(color.ToString()); 
                spriteRenderer.color = new Color(color.r, color.g, color.b, color.a/2);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Замените "Player" на тег вашего персонажа
        {
            foreach (var spriteRenderer in gameObject.transform.parent.GetComponentsInChildren<SpriteRenderer>())
            {
                if (needToSetSortTree)
                    GetComponent<SpriteRenderer>().sortingLayerName = sortingLayer;
                Color color = spriteRenderer.color;
                //Debug.Log(color.ToString());
                spriteRenderer.color = new Color(color.r, color.g, color.b, color.a*2);
            }
        }
    }
}
