using System.Collections.Generic;
using UnityEngine;

public class ItemStructure : MonoBehaviour
{
    public List<ItemType> itemTypes;
    public ItemRarity itemRarity;

    [SerializeField] private Vector2Int _size = new Vector2Int(3, 3);
    [SerializeField] private bool[] _cells;

    [SerializeField] private List<DescriptionTriple> _descriptionTriples = new List<DescriptionTriple>();

    [Header("Click Settings")]
    [SerializeField] private float _doubleClickTime = 0.3f;

    private float _lastClickTime;
    private bool _hasCollider;

    // �������� ������ ��� ������
    public Vector2Int Size => _size;
    public bool[] Cells => _cells;
    public IReadOnlyList<DescriptionTriple> DescriptionTriples => _descriptionTriples;

    private void Awake()
    {
        CheckCollider();
    }

    private void CheckCollider()
    {
        _hasCollider = GetComponent<Collider2D>() != null;
        if (!_hasCollider)
        {
            Debug.LogWarning($"ItemStructure on {gameObject.name} requires a 2D Collider for click detection", this);
        }
    }

    private void Update()
    {
        if (!_hasCollider) return;

        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        // ��������� hover ��� ��������
        if (IsMouseOverObject())
        {
            if (Input.GetMouseButtonDown(0)) // ���
            {
                HandleLeftClick();
            }
            else if (Input.GetMouseButtonDown(1)) // ���
            {
                HandleRightClick();
            }
        }
    }

    private bool IsMouseOverObject()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePosition);
        return hit != null && hit.gameObject == gameObject;
    }

    private void HandleLeftClick()
    {
        Debug.Log($"Left click on: {gameObject.name}");

        // ������ ���������� �����
        //OnLeftClick();

        // �������� �� ������� ����
        float timeSinceLastClick = Time.time - _lastClickTime;

        if (timeSinceLastClick < _doubleClickTime)
        {
            Debug.Log($"Double click on: {gameObject.name}");
            OnDoubleClick();
            _lastClickTime = 0f; // ���������� ������
        }
        else
        {
            _lastClickTime = Time.time;
        }
    }

    private void HandleRightClick()
    {
        Debug.Log($"Right click on: {gameObject.name}");
        OnRightClick();
    }

    // ������ � ������� ��������� ������
    private void OnDoubleClick()
    {
        // ������ �������� �����
        Debug.Log("Double click action executed!");

        // ������: ��������/������ �������������� ����������
        ToggleDetailedInfo();
    }

    private void OnRightClick()
    {
        // ������ ������� �����
        Debug.Log("Right click action executed!");

        // ������: ������� ����������� ����
        ShowContextMenu();
    }

    // ������� ���������� ��������
    private void ToggleDetailedInfo()
    {
        // ������������ ����������� ��������� ����������
        Debug.Log("Toggling detailed info...");

        // ����� ����� �������� ������ ������/������� UI ���������
        // ��� ��������� �������� ���� ��������
    }

    private void ShowContextMenu()
    {
        // �������� ����������� ���� � �������
        Debug.Log("Showing context menu...");

        // ������: ������� � ������� ��� description triples
        if (_descriptionTriples.Count > 0)
        {
            Debug.Log("Available description triples:");
            foreach (var triple in _descriptionTriples)
            {
                Debug.Log($"- {triple.NameKey}: {triple.AnswerKey} ({triple.DescriptionKey})");
            }
        }
    }

    private void SelectItem()
    {
        // ��������� ��������
        Debug.Log("Item selected!");

        // ����� �������� ���������� ���������, �������� �������� ����
    }

    // ������������� ���������� ��� ��������� � ����������
    private void OnValidate()
    {
        // ����������� ������ ���� ������ ���������
        if (_cells == null || _cells.Length != _size.x * _size.y)
        {
            _cells = new bool[_size.x * _size.y];
        }

        CheckCollider();
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

    // ������ ��� ������ � �������� ��������
    public void AddDescriptionTriple(string nameKey, string answerKey, string descriptionKey)
    {
        _descriptionTriples.Add(new DescriptionTriple(nameKey, answerKey, descriptionKey));
    }

    public void RemoveDescriptionTripleAt(int index)
    {
        if (index >= 0 && index < _descriptionTriples.Count)
        {
            _descriptionTriples.RemoveAt(index);
        }
    }

    public void ClearDescriptionTriples()
    {
        _descriptionTriples.Clear();
    }

    public DescriptionTriple GetDescriptionTriple(int index)
    {
        if (index >= 0 && index < _descriptionTriples.Count)
            return _descriptionTriples[index];
        return null;
    }

    public bool TryGetDescriptionTriple(int index, out DescriptionTriple result)
    {
        if (index >= 0 && index < _descriptionTriples.Count)
        {
            result = _descriptionTriples[index];
            return true;
        }
        result = null;
        return false;
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