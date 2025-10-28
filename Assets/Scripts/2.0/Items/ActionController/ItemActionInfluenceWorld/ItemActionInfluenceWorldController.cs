using UnityEngine;
using System.Collections;

public abstract class ItemActionInfluenceWorldController : ItemActionController
{
    public bool isActive = false;
    private GameObject _playerInventory;
    protected override void Awake()
    {
        base.Awake();

        if (GameObject.Find("InventoryData"))
        {
            _playerInventory = GameObject.Find("InventoryData");
        }
        else
        {
            _playerInventory = GameObject.Find("InventoryTradeData");
        }
        if (transform.parent == _playerInventory)
        {
            isActive = true;
        }
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