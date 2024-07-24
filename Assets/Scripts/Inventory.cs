using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private List<AssetItem> Items;
        [SerializeField] private InventoryCell _inventoryCellTemplate;


        public void OnAnimatorIK(int layerIndex)
        {
            
        }
    }
}
