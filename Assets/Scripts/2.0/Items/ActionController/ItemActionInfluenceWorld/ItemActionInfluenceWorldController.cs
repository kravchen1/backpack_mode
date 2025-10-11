using UnityEngine;
using System.Collections;

public abstract class ItemActionInfluenceWorldController : ItemActionController
{
    protected override void Awake()
    {
        base.Awake();
    }
    public virtual void InfluenceOnTheWorld()
    {
    }
    public virtual void InfluenceOnThePlayer()
    {

    }

    public virtual void ReverseInfluenceOnTheWorld()
    {
    }
    public virtual void ReverseInfluenceOnThePlayer()
    {

    }
}