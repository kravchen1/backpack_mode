using UnityEngine;

public class RaycastStructure : ScriptableObject
{
    public bool isDeleted = false;
    public RaycastHit2D raycastHit;

    public RaycastStructure(RaycastHit2D raycastHit)
    {
        this.raycastHit = raycastHit;
        this.isDeleted = false;
    }

}


