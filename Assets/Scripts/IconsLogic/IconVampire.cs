using UnityEngine;

public class IconVampire : Icon
{
    public IconVampire(int countStack, GameObject gameObject) : base(countStack)
    {

    }
    public override void Activation()
    {
        Debug.Log(gameObject.name);
    }
}