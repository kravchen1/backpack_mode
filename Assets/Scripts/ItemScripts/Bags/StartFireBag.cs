using static UnityEditor.Progress;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.U2D;
using UnityEngine.SceneManagement;
using System;

public class StartFireBag : Bag
{
    public override void Activation()
    {
        base.Activation();
    }

    public override void Update()
    {
        if (SceneManager.GetActiveScene().name == "BackPackBattle")
        {
            // Activation();
        }

        if (SceneManager.GetActiveScene().name == "BackPackShop")
        {
            BagDefauldUpdate();
        }
    }
}