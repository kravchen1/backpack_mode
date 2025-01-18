using UnityEngine;

public class IconBlind : Icon
{
    public IconBlind(int countStack, GameObject gameObject) : base(countStack)
    {

    }
    public override void Activation()
    {
        Debug.Log(gameObject.name);
    }
}