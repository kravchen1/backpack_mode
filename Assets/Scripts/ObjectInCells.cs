using UnityEngine;

public class ObjectInCells : ScriptableObject
{
    public ItemNew gameObject;
    public bool canInsert;

    public ObjectInCells(ItemNew gameObject)
    {
        this.gameObject = gameObject;
        this.canInsert = false;
    }

}


