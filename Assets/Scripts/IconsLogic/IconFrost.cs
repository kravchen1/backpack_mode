using UnityEngine;

public class IconFrost : Icon
{
    public IconFrost(int countStack, GameObject gameObject) : base(countStack)
    {

    }
    public override void Activation()
    {
        Debug.Log(gameObject.name);
    }
}