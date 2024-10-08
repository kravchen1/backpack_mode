using UnityEngine;

public class ForgeItemScript : MonoBehaviour
{
    public Canvas Description;
    private void OnMouseUpAsButton()
    {
        var anotherDescription = GameObject.FindGameObjectWithTag("ForgeDescription");
        if(anotherDescription != null )
        {
            Destroy(anotherDescription);
        }
        var inst = Instantiate(Description.transform,GameObject.FindGameObjectWithTag("ForgeCanvas").transform);
        inst.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
    }
}
