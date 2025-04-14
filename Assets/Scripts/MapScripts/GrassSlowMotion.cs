using UnityEngine;

public class GrassSlowMotion : MonoBehaviour
{
    [HideInInspector] public Player player;
    private bool isPlayerInTrigger = false;
    private float reducedSpeedPercent = 10f;
    private float reducedSpeed;
    private string sortingLayer;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Замените "Player" на тег вашего персонажа
        {
            if (!isPlayerInTrigger)
            {
                if (player == null)
                {
                    player = other.GetComponent<Player>();
                    //sortingLayer = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName;
                }
                reducedSpeed = player.speed / 100 * reducedSpeedPercent;
                player.speed -= reducedSpeed;
                isPlayerInTrigger = true;
                foreach (var spriteRenderer in gameObject.transform.GetComponentsInChildren<SpriteRenderer>())
                {
                    //GetComponent<SpriteRenderer>().sortingLayerName = "Tree";
                    Color color = spriteRenderer.color;
                    //Debug.Log(color.ToString()); 
                    spriteRenderer.color = new Color(color.r, color.g, color.b, color.a / 2);
                }
            }
            //var spriteRenderer = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
            //spriteRenderer.sortingLayerName = "Tree";
            //Color color = spriteRenderer.color;
            //Debug.Log(color.ToString()); 
            //spriteRenderer.color = new Color(color.r, color.g, color.b, color.a / 2);

        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Замените "Player" на тег вашего персонажа
        {
            if (isPlayerInTrigger)
            {
                isPlayerInTrigger = false;
                player.speed += reducedSpeed;

                foreach (var spriteRenderer in gameObject.transform.GetComponentsInChildren<SpriteRenderer>())
                {
                    //GetComponent<SpriteRenderer>().sortingLayerName = sortingLayer;
                    Color color = spriteRenderer.color;
                    //Debug.Log(color.ToString()); 
                    spriteRenderer.color = new Color(color.r, color.g, color.b, color.a * 2);
                }
            }
        }
    }
}
