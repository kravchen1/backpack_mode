using UnityEngine;

public class IconBurn : Icon
{
    public IconBurn(int countStack, GameObject gameObject) : base(countStack)
    {

    }
    public override void Activation()
    {
        Debug.Log(gameObject.name);
    }
}