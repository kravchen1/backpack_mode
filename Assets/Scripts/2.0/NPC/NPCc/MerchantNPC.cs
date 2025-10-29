using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using static UnityEngine.Rendering.STP;

public class MerchantNPC : NPC
{
    [Header("Merchant Configuration")]
    [SerializeField] private int requiredReputation = 100;
    [SerializeField] private int initialReputation = 0;

    private int currentReputation;
    private bool isFriend = false;

    private Coroutine lookCoroutine;

    // Свойства для доступа к репутации
    public int CurrentReputation => currentReputation;
    public int RequiredReputation => requiredReputation;
    public bool IsFriend => isFriend;

    // Событие для уведомления об изменении репутации
    public System.Action<int, int> OnReputationChanged; // текущая, предыдущая

    #region Unity Lifecycle
    protected override void Start()
    {
        // Инициализируем репутацию перед вызовом базового Start
        currentReputation = initialReputation;

        // Вызываем базовый Start для стандартной инициализации NPC
        base.Start();

        // Устанавливаем начальное состояние на основе репутации
        UpdateStateBasedOnReputation();
    }
    #endregion

    #region Reputation Management
    public void AddReputation(int amount)
    {
        if (amount == 0 || isFriend) return;

        int previousReputation = currentReputation;
        currentReputation = Mathf.Max(0, currentReputation + amount);

        OnReputationChanged?.Invoke(currentReputation, previousReputation);

        // Проверяем, достигли ли требуемой репутации
        if (currentReputation >= requiredReputation && !isFriend)
        {
            BecomeFriend();
        }

        // Обновляем визуальные индикаторы (опционально)
        UpdateVisualFeedback();
    }

    public void SetReputation(int value)
    {
        if (value == currentReputation || isFriend) return;

        int previousReputation = currentReputation;
        currentReputation = Mathf.Max(0, value);

        OnReputationChanged?.Invoke(currentReputation, previousReputation);

        if (currentReputation >= requiredReputation && !isFriend)
        {
            BecomeFriend();
        }

        UpdateVisualFeedback();
    }

    public void ResetReputation()
    {
        if (isFriend) return;

        int previousReputation = currentReputation;
        currentReputation = initialReputation;

        OnReputationChanged?.Invoke(currentReputation, previousReputation);
        UpdateStateBasedOnReputation();
        UpdateVisualFeedback();
    }

    private void BecomeFriend()
    {
        isFriend = true;
        SetState(NPCStateType.Friendly);

        // Дополнительные действия при становлении другом
        OnBecameFriend();
    }

    protected virtual void OnBecameFriend()
    {
        // Можно переопределить в дочерних классах для дополнительной логики
        Debug.Log($"{Config.NPCName} стал вашим другом!");

        // Например, начать торговать или дать специальный предмет
    }

    private void UpdateStateBasedOnReputation()
    {
        if (isFriend)
        {
            SetState(NPCStateType.Friendly);
        }
        else
        {
            // Торговцы по умолчанию нейтральны, пока не станут друзьями
            SetState(NPCStateType.Neutral);
        }
    }

    private void UpdateVisualFeedback()
    {
        // Можно изменить цвет имени в зависимости от репутации
        if (nameTextMeshPro != null)
        {
            float reputationPercent = (float)currentReputation / requiredReputation;
            Color reputationColor = Color.Lerp(Config.neutralColor, Config.friendlyColor, reputationPercent);
            nameTextMeshPro.color = reputationColor;
        }
    }
    #endregion

    #region Overridden NPC Methods
    // Переопределяем метод обнаружения игрока - торговцы не реагируют на вход в триггер
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !other.isTrigger) return;

        // Торговцы не проверяют харизму при входе в триггер
        // Просто сохраняем ссылку на игрока для возможного взаимодействия
        var playerController = other.GetComponent<TopDownCharacterController>();
        if (playerController != null)
        {
            DetectedPlayer = playerController;

            // Можно добавить свою логику реакции на игрока
            OnPlayerApproached(playerController);
        }
    }

    // Переопределяем метод потери игрока
    protected override void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !other.isTrigger) return;

        var playerController = other.GetComponent<TopDownCharacterController>();
        if (playerController == DetectedPlayer)
        {
            // Торговцы не преследуют игрока, поэтому просто сбрасываем ссылку
            OnPlayerLeft(playerController);
            DetectedPlayer = null;
        }
    }

    // Переопределяем проверку харизмы - торговцы используют репутацию вместо харизмы
    public override void CheckCharisma()
    {
        // Торговцы игнорируют харизму игрока, используя систему репутации
        // Можно оставить пустым или добавить логику для особых случаев
    }

    // Переопределяем методы смены состояния, чтобы учесть систему репутации
    public override void MakeHostile()
    {
        // Торговцы не могут стать враждебными через этот метод
        // Их состояние определяется только репутацией
        Debug.LogWarning($"Торговец {Config.NPCName} не может стать враждебным. Используйте систему репутации.");
    }

    public override void MakeFriendly()
    {
        // Можно использовать для принудительного установления дружбы
        if (!isFriend)
        {
            BecomeFriend();
        }
    }

    public override void MakeNeutral()
    {
        // Торговцы всегда нейтральны, пока не станут друзьями
        if (!isFriend)
        {
            SetState(NPCStateType.Neutral);
        }
    }
    #endregion

    #region Merchant Specific Methods
    protected virtual void OnPlayerApproached(TopDownCharacterController player)
    {
        StopMovement();

        if (lookCoroutine != null)
        {
            StopCoroutine(lookCoroutine);
        }

        lookCoroutine = StartCoroutine(LookAtPlayerRoutine(this, player));
    }

    protected virtual void OnPlayerLeft(TopDownCharacterController player)
    {
        if (lookCoroutine != null)
        {
            StopCoroutine(lookCoroutine);
            lookCoroutine = null;
        }

        AnimationController?.CancelForcedLook();
        currentState.UpdateState(this);
    }

    // Метод для взаимодействия с торговцем (вызывается извне)
    public virtual void InteractWithMerchant()
    {
        if (isFriend)
        {
            StartTradeWithFriend();
        }
        else
        {
            StartTradeWithNeutral();
        }
    }

    protected virtual void StartTradeWithNeutral()
    {
        // Базовая торговля без бонусов
        Debug.Log($"{Config.NPCName}: Добро пожаловать! Мои цены справедливы.");
        // Здесь можно открыть UI торговли
    }

    protected virtual void StartTradeWithFriend()
    {
        // Торговля с бонусами для друзей
        Debug.Log($"{Config.NPCName}: Рад видеть друга! У меня для тебя особые условия.");
        // Здесь можно открыть UI торговли с лучшими ценами/ассортиментом
    }
    #endregion

    #region Coroutines
    private IEnumerator LookAtPlayerRoutine(NPC npc, TopDownCharacterController player)
    {
        while (player != null && npc.HasDetectedPlayer)
        {
            Vector2 direction = (player.transform.position - npc.transform.position).normalized;
            npc.ForceLookAt(direction, 0.5f);
            yield return new WaitForSeconds(0.1f);
        }

        npc.AnimationController?.CancelForcedLook();
    }
    #endregion

    #region Editor Methods
#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();

        // Валидация настроек торговца
        if (requiredReputation <= 0)
        {
            Debug.LogWarning($"{name}: requiredReputation должен быть положительным числом!", this);
            requiredReputation = 100;
        }

        if (initialReputation < 0)
        {
            initialReputation = 0;
        }
    }

    // Контекстные меню для тестирования в редакторе
    [ContextMenu("Add 10 Reputation")]
    private void TestAddReputation() => AddReputation(10);

    [ContextMenu("Add 100 Reputation")]
    private void TestAddMaxReputation() => AddReputation(100);

    [ContextMenu("Reset Reputation")]
    private void TestResetReputation() => ResetReputation();
#endif
    #endregion
}