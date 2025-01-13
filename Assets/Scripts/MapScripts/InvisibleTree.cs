using UnityEngine;

public class InvisibleTree : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            Debug.Log("Enter");
            collision.gameObject.transform.parent.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision != null)
        {
            Debug.Log("Exit");
            collision.gameObject.transform.parent.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
