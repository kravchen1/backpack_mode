using UnityEngine;

public class TalantLine : MonoBehaviour
{
    private Color buttonColorNoActive = new Color(0.5f, 0.5f, 0.5f), buttonColorActive = new Color(1f, 1f, 1f);
    public bool activated = false;

    public void Activate(bool booling)
    {
        if (!booling)
        {
            GetComponent<SpriteRenderer>().color = buttonColorNoActive;
            activated = false;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = buttonColorActive;
            activated = true;
        }
    }
}
