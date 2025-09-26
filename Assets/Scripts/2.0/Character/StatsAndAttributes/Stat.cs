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

    // ������ ������������� ��� ������� ������/��������
    //private List<StatModifier> _modifiers = new List<StatModifier>();

    // �������, ������� ����� ���������� ��� ��������� ��������
    public event System.Action<int> OnValueChanged;

    public int GetValue()
    {
        int finalValue = _baseValue;
        // ����� � ������� ����� ��������� ������������
        // foreach (var mod in _modifiers) finalValue += mod.Value;
        return finalValue;
    }

    // ������ ��� ����������/�������� �������������...
}