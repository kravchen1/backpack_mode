using UnityEngine;

public class IconBaseCrit : Icon
{
    public IconBaseCrit(int countStack, GameObject gameObject) : base(countStack)
    {

    }
    public override void Activation()
    {
        Debug.Log(gameObject.name);
    }
}