using UnityEngine;
using UnityEngine.EventSystems;

public class DescriptionMenuButtonController : MonoBehaviour
{
    public void SelectButton()
    {
        Debug.Log("SelectButton");
    }

    public void DeSelectButton()
    {
        Debug.Log("DeSelectButton");
    }

    public void HoverButton()
    {
        Debug.Log("HoverButton");
    }
}