using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemStructure : MonoBehaviour
{
    [SerializeField] private Vector2Int _size = new Vector2Int(3, 3);
    [SerializeField] private bool[] _cells;

    // �������� ������ ��� ������
    public Vector2Int Size => _size;
    public bool[] Cells => _cells;

    // ������������� ���������� ��� ��������� � ����������
    private void OnValidate()
    {
        // ����������� ������ ���� ������ ���������
        if (_cells == null || _cells.Length != _size.x * _size.y)
        {
            _cells = new bool[_size.x * _size.y];
        }
    }

    // ��������� �������� ������ �� �����������
    public bool GetCell(int x, int y)
    {
        if (x < 0 || x >= _size.x || y < 0 || y >= _size.y)
            return false;

        return _cells[y * _size.x + x];
    }

    // ���������� ��� Vector2Int
    public bool GetCell(Vector2Int position)
    {
        return GetCell(position.x, position.y);
    }

    // ��������� �������� ������
    public void SetCell(int x, int y, bool value)
    {
        if (x < 0 || x >= _size.x || y < 0 || y >= _size.y)
            return;

        _cells[y * _size.x + x] = value;
    }

    // �������������� �������� ������
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