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
    public int countBurnStack = 1;
    public override void StarActivation()
    {
        if (Player != null)
        {
            Player.menuFightIconData.AddBuff(countBurnStack, "IconBurn");
            Debug.Log("����� ���� �������� 1 ���");
            Player.menuFightIconData.CalculateFireFrostStats();//true = Player
        }
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