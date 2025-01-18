using UnityEngine;

public class IconEvasion : Icon
{
    public IconEvasion(int countStack, GameObject gameObject) : base(countStack)
    {

    }
    public override void Activation()
    {
        Debug.Log(gameObject.name);
    }
}