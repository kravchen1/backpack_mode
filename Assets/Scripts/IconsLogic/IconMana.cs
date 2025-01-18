using UnityEngine;

public class IconMana : Icon
{
    public IconMana(int countStack, GameObject gameObject) : base(countStack)
    {

    }
    public override void Activation()
    {
        Debug.Log(gameObject.name);
    }
}