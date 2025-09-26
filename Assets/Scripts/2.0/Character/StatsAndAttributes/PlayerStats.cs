// PlayerStats.cs
using UnityEngine;

[System.Serializable]
public class PlayerStats : MonoBehaviour
{
    // Ссылка на атрибуты
    public PlayerAttributes attributes;

    // Здоровье
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;

    // Выносливость
    [SerializeField] private float _maxStamina;
    [SerializeField] private float _currentStamina;

    // Деньги
    [SerializeField] private float _money;

    // Вес
    [SerializeField] private float _currentWeight;
    [SerializeField] private float _maxWeight;

    // Скорость движения
    [SerializeField] private float _baseMoveSpeed = 2f;
    [SerializeField] private float _currentMoveSpeed;

    // Пороги веса (настраиваются в инспекторе)
    public float lightLoadThreshold = 30f;
    public float mediumLoadThreshold = 60f;
    public float heavyLoadThreshold = 90f;

    // События для UI (например, полосок здоровья)
    public event System.Action<int, int> OnHealthChanged; // (current, max)
    public event System.Action<float, float> OnStaminaChanged;
    public event System.Action<float, float> OnWeightChanged; // (current, max)
    public event System.Action<float> OnMoneyChanged;

    // Новое событие специально для урона
    public event System.Action<int, int, int> OnDamageTaken; // damage, currentHealth, maxHealth
    public event System.Action OnDeath;

    // Событие для изменения скорости
    public event System.Action<float> OnMoveSpeedChanged;

    // Настройки восстановления стамины
    [SerializeField] private float _staminaRegenDelay = 1f; // Задержка перед началом восстановления
    [SerializeField] private float _staminaRegenRate = 1f; // Скорость восстановления в секунду
    [SerializeField] private float _staminaRegenRateInCombat = 1f; // Медленное восстановление в бою

    // Таймеры для восстановления
    private float _staminaRegenTimer;
    private bool _isInCombat = false;

    public void SetCombatState(bool inCombat)
    {
        _isInCombat = inCombat;
    }

    public void StartStaminaRegen()
    {
        // Запускаем таймер восстановления
        _staminaRegenTimer = _staminaRegenDelay;
    }

    public void StopStaminaRegen()
    {
        _staminaRegenTimer = 0f;
    }

    // Метод для обновления восстановления (будем вызывать извне)
    public void UpdateStaminaRegen(float deltaTime)
    {
        if (_currentStamina >= MaxStamina) return;

        if (_staminaRegenTimer > 0)
        {
            _staminaRegenTimer -= deltaTime;
            return;
        }

        // Восстанавливаем стамину
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

            // Всегда вызываем изменение здоровья
            OnHealthChanged?.Invoke(_currentHealth, MaxHealth);

            // Если здоровье уменьшилось - вызываем событие урона
            if (_currentHealth < previousHealth)
            {
                int damage = previousHealth - _currentHealth;
                OnDamageTaken?.Invoke(damage, _currentHealth, MaxHealth);

                Debug.Log($"Damage taken: {damage}. Health: {_currentHealth}/{MaxHealth}");

                // Проверяем смерть
                if (_currentHealth <= 0)
                {
                    OnDeath?.Invoke();
                }
            }
        }
    }

    public int MaxHealth
    {
        get => _maxHealth; // Можно добавить модификатор от силы: _maxHealth + attributes.Strength * 10;
        set
        {
            _maxHealth = value + attributes.Endurance * 10;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0, _maxHealth); // Корректируем текущее здоровье
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }
    }

    // Аналогично для Stamina...
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
        get => _maxStamina; // Можно добавить модификатор от Выносливости: _maxHealth + attributes.Strength * 10;
        set
        {
            _maxStamina = value + attributes.Endurance;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, _maxStamina); // Корректируем текущее здоровье
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
        get => _maxWeight; // Можно сделать зависимым от силы
        set
        {
            _maxWeight = value + attributes.Strength * 2;
            OnWeightChanged?.Invoke(_currentWeight, _maxWeight);
        }
    }

    // Метод для получения текущей категории нагрузки
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

        // Применяем модификаторы от веса
        newSpeed = ApplyWeightModifiers(newSpeed);

        CurrentMoveSpeed = newSpeed;
    }

    private float ApplyWeightModifiers(float speed)
    {
        var loadCategory = GetCurrentLoadCategory();

        switch (loadCategory)
        {
            //case LoadCategory.Light:
            //    return speed * 1.1f; // +10% скорости при легкой нагрузке
            //case LoadCategory.Medium:
            //    return speed; // Базовая скорость
            //case LoadCategory.Heavy:
            //    return speed * 0.7f; // -30% скорости при тяжелой нагрузке
            case LoadCategory.Overloaded:
                return speed * 0.4f; // -60% скорости при перегрузке
            default:
                return speed;
        }
    }

    // Инициализация (вызывается после загрузки атрибутов)
    public void Initialize(PlayerAttributes attrs)
    {
        attributes = attrs;
        // Пересчитываем MaxHealth, MaxStamina, MaxWeight в зависимости от атрибутов
        RecalculateDerivedStats();
        // Подписываемся на изменения атрибутов, чтобы пересчитывать производные статы
        attributes.strength.OnValueChanged += (value) => RecalculateDerivedStats();
        attributes.endurance.OnValueChanged += (value) => RecalculateDerivedStats();
        // ... подписаться на другие атрибуты при необходимости
    }

    private void RecalculateDerivedStats()
    {
        MaxHealth = 100 + (attributes.Endurance * 10); // Пример формулы
        MaxStamina = 10 + (attributes.Endurance);
        MaxWeight = 50 + (attributes.Strength * 2);
    }

    // Методы для временных модификаторов скорости (баффы/дебаффы)
    public void AddSpeedModifier(float multiplier, float duration)
    {
        // Реализация временных модификаторов
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
    Light,      // Слабая нагрузка
    Medium,     // Нагрузка
    Heavy,      // Высокая нагрузка
    Overloaded  // Перегруз
}