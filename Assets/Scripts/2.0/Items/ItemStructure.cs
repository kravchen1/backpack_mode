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

    // Свойства только для чтения
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
        // Проверяем hover над объектом
        if (IsMouseOverObject())
        {
            if (Input.GetMouseButtonDown(0)) // ЛКМ
            {
                HandleLeftClick();
            }
            else if (Input.GetMouseButtonDown(1)) // ПКМ
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

        // Логика одинарного клика
        //OnLeftClick();

        // Проверка на двойной клик
        float timeSinceLastClick = Time.time - _lastClickTime;

        if (timeSinceLastClick < _doubleClickTime)
        {
            Debug.Log($"Double click on: {gameObject.name}");
            OnDoubleClick();
            _lastClickTime = 0f; // Сбрасываем таймер
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

    // Методы с логикой обработки кликов
    private void OnDoubleClick()
    {
        // Логика двойного клика
        Debug.Log("Double click action executed!");

        // Пример: показать/скрыть дополнительную информацию
        ToggleDetailedInfo();
    }

    private void OnRightClick()
    {
        // Логика правого клика
        Debug.Log("Right click action executed!");

        // Пример: открыть контекстное меню
        ShowContextMenu();
    }

    // Примеры конкретных действий
    private void ToggleDetailedInfo()
    {
        // Переключение отображения подробной информации
        Debug.Log("Toggling detailed info...");

        // Здесь можно добавить логику показа/скрытия UI элементов
        // или изменения внешнего вида предмета
    }

    private void ShowContextMenu()
    {
        // Показать контекстное меню с опциями
        Debug.Log("Showing context menu...");

        // Пример: вывести в консоль все description triples
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
        // Выделение предмета
        Debug.Log("Item selected!");

        // Можно добавить визуальное выделение, например изменить цвет
    }

    // Автоматически вызывается при изменении в инспекторе
    private void OnValidate()
    {
        // Пересоздаем массив если размер изменился
        if (_cells == null || _cells.Length != _size.x * _size.y)
        {
            _cells = new bool[_size.x * _size.y];
        }

        CheckCollider();
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

    // Методы для работы с тройками описаний
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