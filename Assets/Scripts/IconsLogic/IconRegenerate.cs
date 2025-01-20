using UnityEngine;

public class IconRegenerate : Icon
{
    public IconRegenerate(int countStack, GameObject gameObject) : base(countStack)
    {

    }
    public override void Activation()
    {
        Debug.Log(gameObject.name);
    }
}