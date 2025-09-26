// PlayerStats.cs
using UnityEngine;

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    // ������ �� ��������
    public PlayerAttributes attributes;

    // ��������
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;

    // ������������
    [SerializeField] private float _maxStamina;
    [SerializeField] private float _currentStamina;

    // ������
    [SerializeField] private float _money;

    // ���
    [SerializeField] private float _currentWeight;
    [SerializeField] private float _maxWeight;

    // �������� ��������
    [SerializeField] private float _baseMoveSpeed = 2f;
    [SerializeField] private float _currentMoveSpeed;

    // ������ ���� (������������� � ����������)
    public float lightLoadThreshold = 30f;
    public float mediumLoadThreshold = 60f;
    public float heavyLoadThreshold = 90f;

    // ������� ��� UI (��������, ������� ��������)
    public event System.Action<int, int> OnHealthChanged; // (current, max)
    public event System.Action<float, float> OnStaminaChanged;
    public event System.Action<float, float> OnWeightChanged; // (current, max)
    public event System.Action<float> OnMoneyChanged;

    // ����� ������� ���������� ��� �����
    public event System.Action<int, int, int> OnDamageTaken; // damage, currentHealth, maxHealth
    public event System.Action OnDeath;

    // ������� ��� ��������� ��������
    public event System.Action<float> OnMoveSpeedChanged;

    // ��������� �������������� �������
    [SerializeField] private float _staminaRegenDelay = 1f; // �������� ����� ������� ��������������
    [SerializeField] private float _staminaRegenRate = 1f; // �������� �������������� � �������
    [SerializeField] private float _staminaRegenRateInCombat = 1f; // ��������� �������������� � ���

    // ������� ��� ��������������
    private float _staminaRegenTimer;
    private bool _isInCombat = false;

    public void SetCombatState(bool inCombat)
    {
        _isInCombat = inCombat;
    }

    public void StartStaminaRegen()
    {
        // ��������� ������ ��������������
        _staminaRegenTimer = _staminaRegenDelay;
    }

    public void StopStaminaRegen()
    {
        _staminaRegenTimer = 0f;
    }

    // ����� ��� ���������� �������������� (����� �������� �����)
    public void UpdateStaminaRegen(float deltaTime)
    {
        if (_currentStamina >= MaxStamina) return;

        if (_staminaRegenTimer > 0)
        {
            _staminaRegenTimer -= deltaTime;
            return;
        }

        // ��������������� �������
        float regenRate = _isInCombat ? _staminaRegenRateInCombat : _staminaRegenRate;
        float regenAmount = regenRate * deltaTime;

        CurrentStamina = Mathf.Min(MaxStamina, _currentStamina + regenAmount);
    }

    public int CurrentHealth
    {
        get => _currentHealth;
        set
        {
            int previousHealth = _currentHealth;
            _currentHealth = Mathf.Clamp(value, 0, MaxHealth);

            // ������ �������� ��������� ��������
            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);

            // ���� �������� ����������� - �������� ������� �����
            if (_currentHealth < previousHealth)
            {
                int damage = previousHealth - _currentHealth;
                OnDamageTaken?.Invoke(damage, _currentHealth, MaxHealth);

                Debug.Log($"Damage taken: {damage}. Health: {_currentHealth}/{MaxHealth}");

                // ��������� ������
                if (_currentHealth <= 0)
                {
                    OnDeath?.Invoke();
                }
            }
        }
    }

    public int MaxHealth
    {
        get => _maxHealth; // ����� �������� ����������� �� ����: _maxHealth + attributes.Strength * 10;
        set
        {
            _maxHealth = value + attributes.Endurance * 10;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, _maxHealth); // ������������ ������� ��������
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }
    }

    // ���������� ��� Stamina...
    public float CurrentStamina
    {
        get => _currentStamina;
        set
        {
            _currentStamina = Mathf.Clamp(value, 0, MaxStamina);
            OnStaminaChanged?.Invoke(_currentStamina, MaxStamina);
        }
    }
    public float MaxStamina
    {
        get => _maxStamina; // ����� �������� ����������� �� ������������: _maxHealth + attributes.Strength * 10;
        set
        {
            _maxStamina = value + attributes.Endurance;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, _maxStamina); // ������������ ������� ��������
            OnStaminaChanged?.Invoke(_currentStamina, _maxStamina);
        }
    }

    public float Money
    {
        get => _money;
        set
        {
            _money = value;
            OnMoneyChanged?.Invoke(_money);
        }
    }

    public float CurrentWeight
    {
        get => _currentWeight;
        set
        {
            _currentWeight = value;
            OnWeightChanged?.Invoke(_currentWeight, MaxWeight);
        }
    }

    public float MaxWeight
    {
        get => _maxWeight; // ����� ������� ��������� �� ����
        set
        {
            _maxWeight = value + attributes.Strength * 2;
            OnWeightChanged?.Invoke(_currentWeight, _maxWeight);
        }
    }

    // ����� ��� ��������� ������� ��������� ��������
    public LoadCategory GetCurrentLoadCategory()
    {
        float loadRatio = CurrentWeight / MaxWeight * 100f;

        if (loadRatio < lightLoadThreshold) return LoadCategory.Light;
        else if (loadRatio < mediumLoadThreshold) return LoadCategory.Medium;
        else if (loadRatio < heavyLoadThreshold) return LoadCategory.Heavy;
        else return LoadCategory.Overloaded;
    }

    public float BaseMoveSpeed
    {
        get => _baseMoveSpeed;
        set
        {
            _baseMoveSpeed = value;
            RecalculateMoveSpeed();
        }
    }

    public float CurrentMoveSpeed
    {
        get => _currentMoveSpeed;
        private set
        {
            if (_currentMoveSpeed != value)
            {
                _currentMoveSpeed = value;
                OnMoveSpeedChanged?.Invoke(_currentMoveSpeed);
            }
        }
    }

    private void RecalculateMoveSpeed()
    {
        float newSpeed = _baseMoveSpeed;

        // ��������� ������������ �� ����
        newSpeed = ApplyWeightModifiers(newSpeed);

        CurrentMoveSpeed = newSpeed;
    }

    private float ApplyWeightModifiers(float speed)
    {
        var loadCategory = GetCurrentLoadCategory();

        switch (loadCategory)
        {
            //case LoadCategory.Light:
            //    return speed * 1.1f; // +10% �������� ��� ������ ��������
            //case LoadCategory.Medium:
            //    return speed; // ������� ��������
            //case LoadCategory.Heavy:
            //    return speed * 0.7f; // -30% �������� ��� ������� ��������
            case LoadCategory.Overloaded:
                return speed * 0.4f; // -60% �������� ��� ����������
            default:
                return speed;
        }
    }

    // ������������� (���������� ����� �������� ���������)
    public void Initialize(PlayerAttributes attrs)
    {
        attributes = attrs;
        // ������������� MaxHealth, MaxStamina, MaxWeight � ����������� �� ���������
        RecalculateDerivedStats();
        // ������������� �� ��������� ���������, ����� ������������� ����������� �����
        attributes.strength.OnValueChanged += (value) => RecalculateDerivedStats();
        attributes.endurance.OnValueChanged += (value) => RecalculateDerivedStats();
        // ... ����������� �� ������ �������� ��� �������������
    }

    private void RecalculateDerivedStats()
    {
        MaxHealth = 100 + (attributes.Endurance * 10); // ������ �������
        MaxStamina = 10 + (attributes.Endurance);
        MaxWeight = 50 + (attributes.Strength * 2);
    }

    // ������ ��� ��������� ������������� �������� (�����/�������)
    public void AddSpeedModifier(float multiplier, float duration)
    {
        // ���������� ��������� �������������
        StartCoroutine(SpeedModifierCoroutine(multiplier, duration));
    }

    private System.Collections.IEnumerator SpeedModifierCoroutine(float multiplier, float duration)
    {
        _baseMoveSpeed *= multiplier;
        RecalculateMoveSpeed();

        yield return new WaitForSeconds(duration);

        _baseMoveSpeed /= multiplier;
        RecalculateMoveSpeed();
    }
}

public enum LoadCategory
{
    Light,      // ������ ��������
    Medium,     // ��������
    Heavy,      // ������� ��������
    Overloaded  // ��������
}