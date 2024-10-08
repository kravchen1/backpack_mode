using UnityEngine;

public class ButtonForgeOk : MonoBehaviour
{

    public void OnMouseUpAsButton()
    {
        var descriptionForge = GameObject.FindGameObjectWithTag("ForgeDescription").GetComponent<DescriptionForge>();
        if(descriptionForge.haveCountItem1 >= descriptionForge.needCountItem1)
        {
            descriptionForge.CreateAndDeleteItemsFromBackPackAndStorage();
            Destroy(descriptionForge);
        }
    }
}
