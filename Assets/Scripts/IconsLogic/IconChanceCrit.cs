using UnityEngine;

public class IconChanceCrit : Icon
{
    public IconChanceCrit(int countStack, GameObject gameObject) : base(countStack)
    {

    }
    public override void Activation()
    {
        Debug.Log(gameObject.name);
    }
}