using UnityEngine;

public class IconBleed : Icon
{
    public IconBleed(int countStack, GameObject gameObject) : base(countStack)
    {
        
    }    
    public override void Activation()
    {
        Debug.Log(gameObject.name);
    }
}