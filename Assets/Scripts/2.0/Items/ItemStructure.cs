using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemStructure : MonoBehaviour
{
    [SerializeField] private Vector2Int _size = new Vector2Int(3, 3);
    [SerializeField] private bool[] _cells;

    // Свойства только для чтения
    public Vector2Int Size => _size;
    public bool[] Cells => _cells;

    // Автоматически вызывается при изменении в инспекторе
    private void OnValidate()
    {
        // Пересоздаем массив если размер изменился
        if (_cells == null || _cells.Length != _size.x * _size.y)
        {
            _cells = new bool[_size.x * _size.y];
        }
    }

    // Получение значения ячейки по координатам
    public bool GetCell(int x, int y)
    {
        if (x < 0 || x >= _size.x || y < 0 || y >= _size.y)
            return false;

        return _cells[y * _size.x + x];
    }

    // Перегрузка для Vector2Int
    public bool GetCell(Vector2Int position)
    {
        return GetCell(position.x, position.y);
    }

    // Установка значения ячейки
    public void SetCell(int x, int y, bool value)
    {
        if (x < 0 || x >= _size.x || y < 0 || y >= _size.y)
            return;

        _cells[y * _size.x + x] = value;
    }

    // Дополнительные полезные методы
    public int GetOccupiedCellCount()
    {
        int count = 0;
        foreach (bool cell in _cells)
        {
            if (cell) count++;
        }
        return count;
    }

    public bool HasAnyOccupiedCell()
    {
        foreach (bool cell in _cells)
        {
            if (cell) return true;
        }
        return false;
    }
}