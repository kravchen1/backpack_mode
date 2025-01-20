using UnityEngine;

public class IconPosion : Icon
{
    public IconPosion(int countStack, GameObject gameObject) : base(countStack)
    {

    }
    public override void Activation()
    {
        Debug.Log(gameObject.name);
    }
}