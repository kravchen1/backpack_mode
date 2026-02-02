using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using static UnityEngine.Rendering.STP;

public class MerchantNPC : NPC
{
    // Событие для уведомления об изменении репутации
    public System.Action<int, int> OnReputationChanged; // текущая, предыдущая

    #region Unity Lifecycle
    protected override void Start()
    {
        // Вызываем базовый Start для стандартной инициализации NPC
        base.Start();
    }
    #endregion


    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !other.isTrigger) return;

        var playerController = other.GetComponent<TopDownCharacterController>();
    }

    // Переопределяем метод потери игрока
    protected override void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !other.isTrigger) return;

        var playerController = other.GetComponent<TopDownCharacterController>();
    }


}