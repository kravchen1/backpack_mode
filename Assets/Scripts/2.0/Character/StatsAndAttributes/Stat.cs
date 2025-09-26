// Stat.cs
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private int _baseValue;

    public int BaseValue
    {
        get => _baseValue;
        set
        {
            if (_baseValue != value)
            {
                _baseValue = value;
                OnValueChanged?.Invoke(GetValue());
            }
        }
    }

    // Список модификаторов для будущих баффов/дебаффов
    //private List<StatModifier> _modifiers = new List<StatModifier>();

    // Событие, которое будет вызываться при изменении значения
    public event System.Action<int> OnValueChanged;

    public int GetValue()
    {
        int finalValue = _baseValue;
        // Здесь в будущем можно применить модификаторы
        // foreach (var mod in _modifiers) finalValue += mod.Value;
        return finalValue;
    }

    // Методы для добавления/удаления модификаторов...
}