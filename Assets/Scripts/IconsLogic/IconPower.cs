using UnityEngine;
public class IconPower : Icon
{
    public IconPower(int countStack, GameObject gameObject) : base(countStack)
    {
        
    }    
    public override void Activation()
    {
        Debug.Log(gameObject.name);
    }
}