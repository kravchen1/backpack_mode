using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSettings : MonoBehaviour
{
    public void OnMouseDown()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
    }

    public void OnMouseUp()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
    }



}
