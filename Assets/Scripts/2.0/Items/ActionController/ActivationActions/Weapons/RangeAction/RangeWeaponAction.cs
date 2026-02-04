using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class RangeWeaponAction : WeaponActionController
{
    private RangeWeaponStats rangeWeaponStats;
    private List<ItemStar> itemStarPatrons = new List<ItemStar>();
    private ItemMove currentPatron;
    private TargetFinder targetFinder;
    private float attackTimer = 0f;
    private GameObject player;

    [SerializeField] private List<ItemType> AmmoTypes = new List<ItemType>();

    protected override void Awake()
    {
        base.Awake();

        // Ищем TargetFinder на игроке
        player = PlayerDataManager.Instance?.playerCharacter;
        if (player != null)
        {
            targetFinder = player.GetComponent<TargetFinder>();
            if (targetFinder == null)
            {
                targetFinder = player.AddComponent<TargetFinder>();
            }
        }

        // Ищем слоты для патронов
        rangeWeaponStats = GetComponent<RangeWeaponStats>();
        FindAmmoStars();
    }

    public override void Equip()
    {
        // Ищем TargetFinder на игроке
        player = PlayerDataManager.Instance?.playerCharacter;
        if (player != null)
        {
            targetFinder = player.GetComponent<TargetFinder>();
            if (targetFinder == null)
            {
                targetFinder = player.AddComponent<TargetFinder>();
            }
        }

        // Ищем слоты для патронов
        rangeWeaponStats = GetComponent<RangeWeaponStats>();
        FindAmmoStars();

        isEquipped = true;
        Debug.Log($"Auto weapon equipped: {gameObject.name}");
    }

    //private IEnumerator Initialize()
    //{
    //    yield return null;
    //    yield return null;
    //    yield return null;
    //    yield return null;
    //    //проставляем звёзды в предметах
    //    itemStarPatrons = GetComponentsInChildren<ItemStar>().ToList().Where(e => HasMatchingItemType(e.AllowedItemTypes)).ToList();
    //    rangeWeaponStats = GetComponent<RangeWeaponStats>();
    //    InitializeBase();
    //}
    private bool HasMatchingAmmoType(List<ItemType> starTypes)
    {
        foreach (var type in starTypes)
        {
            if (AmmoTypes.Contains(type))
                return true;
        }
        return false;
    }
    private void FindAmmoStars()
    {
        // Ищем ItemStar с подходящими типами патронов
        var allStars = GetComponentsInChildren<ItemStar>();
        foreach (var star in allStars)
        {
            if (HasMatchingAmmoType(star.AllowedItemTypes))
            {
                itemStarPatrons.Add(star);
            }
        }
    }
    private void InitializeBase()
    {

        cooldownTime = rangeWeaponStats.CoolDownRange;
        staminaCost = rangeWeaponStats.BaseStaminaRange;
        damageMin = rangeWeaponStats.MinDamageRange;
        damageMax = rangeWeaponStats.MaxDamageRange;
        critDamageMelee = rangeWeaponStats.CritDamageRange;
        baseCritChance = rangeWeaponStats.CritChanceRange;

        currentCooldown = cooldownTime;
        StartCooldown();
    }

    // Обновляем логику для боя
    public override void UpdateForBattle()
    {
        if (!isEquipped || !CheckPatron()) return;

        base.UpdateForBattle();
    }

    protected override void ExecuteAction()
    {
        if (targetFinder == null || !targetFinder.HasTarget) return;

        Transform target = targetFinder.CurrentTarget;
        if (target != null && IsTargetInRange(target))
        {
            Attack(target.position);
            StartCooldown();
            SpendPatron();
        }
    }

    private bool IsTargetInRange(Transform target)
    {
        if (rangeWeaponStats == null) return false;

        float distance = Vector2.Distance(transform.position, target.position);
        return distance <= rangeWeaponStats.DistanceRadius;
    }

    private void Attack(Vector3 targetPosition)
    {
        if (rangeWeaponStats == null || player.transform == null) return;

        if (rangeWeaponStats.ProjectilePrefab == null)
        {
            Debug.LogError($"No projectile prefab assigned to {gameObject.name}");
            return;
        }

        // Создаем снаряды
        for (int i = 0; i < rangeWeaponStats.ProjectileCount; i++)
        {
            CreateProjectile(targetPosition);
        }

        // Применяем износ прочности
        ApplyWeaponDurabilityDamage();
    }

    private void CreateProjectile(Vector3 targetPos)
    {
        // Рассчитываем направление
        Vector2 direction = (targetPos - player.transform.position).normalized;

        // todo выстрелить примерно в том направлении в зависимости от разброса

        // Создаем снаряд
        GameObject projectileObj = Instantiate(
            rangeWeaponStats.ProjectilePrefab,
            player.transform.position,
            Quaternion.identity
        );

        // Масштабируем
        projectileObj.transform.localScale = Vector3.one * rangeWeaponStats.ProjectileSize;

        // Инициализируем снаряд
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
        {
            float damage = CalculateDamage();
            projectile.Initialize(damage, rangeWeaponStats.ProjectileSpeed, transform);
        }

        // Задаем движение
        Rigidbody2D rb = projectileObj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * rangeWeaponStats.ProjectileSpeed;
        }

        // Поворачиваем в направлении движения
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectileObj.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private float CalculateDamage()
    {
        var weaponStats = GetComponent<RangeWeaponStats>();
        if (weaponStats == null) return 10f;

        float baseDamage = Random.Range(weaponStats.MinDamageRange, weaponStats.MaxDamageRange);

        // Критический урон
        if (Random.Range(0, 100) < weaponStats.CritChanceRange)
        {
            baseDamage *= (float)weaponStats.CritDamageRange / 100f;
        }

        return baseDamage;
    }

    private void ApplyWeaponDurabilityDamage()
    {
        var itemStats = GetComponent<ItemStats>();
        if (itemStats != null)
        {
            itemStats.ApplyDamageDurability(1f); // 1 единица износа за выстрел
        }
    }

    // Переопределяем проверку патронов для автоматического оружия
    protected bool CheckPatron()
    {
        foreach (var star in itemStarPatrons)
        {
            if (star.CurrentItem != null)
            {
                var itemMove = star.CurrentItem.GetComponent<ItemMove>();
                if (itemMove != null && itemMove.StackCount >= 1)
                {
                    currentPatron = itemMove;
                    return true;
                }
            }
        }
        currentPatron = null;
        return false;
    }

    // Переопределяем расход патрона
    private void SpendPatron()
    {
        if (currentPatron != null)
        {
            currentPatron.StackCount--;
            if (currentPatron.StackCount == 0)
            {
                Destroy(currentPatron.gameObject);
            }
        }
    }


}