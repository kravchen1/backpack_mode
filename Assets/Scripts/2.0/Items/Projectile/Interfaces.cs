// Интерфейс для объектов, которые могут получать урон
public interface IDamageable
{
    void TakeDamage(float amount);
}

// Интерфейс для объектов, которые можно замедлить
public interface ISlowable
{
    void ApplySlow(float duration, float multiplier);
    void RemoveSlow();
}

// Интерфейс для объектов, которые могут гореть
public interface IBurnable
{
    void ApplyBurn(float duration, float damagePerSecond);
    void RemoveBurn();
}