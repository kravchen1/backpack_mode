using static UnityEditor.Progress;
using System.Collections.Generic;
using UnityEngine;

public class Bag : MonoBehaviour
{
    //public static Bag Main;
    //[SerializeField] private Vector2Int _size = new Vector2Int(10, 10);
    //private List<Item> _items;
    //private bool[,] _fill;

    //private void Awake()
    //{
    //    Main = this;
    //    _fill = new bool[_size.x, _size.y];
    //}

    //public void AddItem(Item item)
    //{
    //    _items.Add(item);
    //    RecalculateFill();
    //}

    //public void RemoveItem(Item item)
    //{
    //    if (_items.Remove(item))
    //        RecalculateFill();
    //}

    //// Проверка на на возможность кинуть в сумку
    //public bool CanDrop(Vector2Int tile, Vector2Int size)
    //{
    //    if (tile.x < 0 || tile.x + size.x > _size.x)
    //        return false;
    //    if (tile.y < 0 || tile.y + size.y > _size.y)
    //        return false;
    //    for (int x = 0; x < size.x; x++)
    //        for (int y = 0; y < size.y; y++)
    //            if (_fill[tile.x + x, tile.y + y])
    //                return false;
    //    return true;
    //}

    //private void RecalculateFill()
    //{
    //    for (int x = 0; x < _size.x; x++)
    //        for (int y = 0; y < _size.y; y++)
    //            _fill[x, y] = false;
    //    foreach (Item item in _items)
    //        for (int x = 0; x < item.Size.x; x++)
    //            for (int y = 0; y < item.Size.y; y++)
    //                _fill[item.BagPosition.x + x, item.BagPosition.y + y] = true;
    //}
}