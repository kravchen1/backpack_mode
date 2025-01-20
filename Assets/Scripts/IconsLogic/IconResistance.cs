using UnityEngine;

public class IconResistance : Icon
{
    public IconResistance(int countStack, GameObject gameObject) : base(countStack)
    {

    }
    public override void Activation()
    {
        Debug.Log(gameObject.name);
    }
}