// PlayerDataManager.cs
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static UnityEngine.Rendering.STP;

public class PlayerDataManager : MonoBehaviour
{
    public string PlayerName = "playerTest1";
    public static PlayerDataManager Instance; // Синглтон для простого доступа

    public PlayerAttributes Attributes { get; private set; }
    public PlayerStats Stats { get; private set; }

    private string _saveKey = "PlayerStatsAndAttributes"; // Ключ для PlayerPrefs

    public GameObject playerCharacter;


    public CellsData cellsInventory;
    public CellsData cellsFight;
    public bool IsAlive => Stats.CurrentHealth > 0;

    #region flashLight
    private float _flashLightRadius = 0f;
    private float _flashLightIntensity = 0f;

    public Light2D _flashLight; // Используем стандартный Light компонент
    private Coroutine _radiusChangeCoroutine;
    private Coroutine _intensityChangeCoroutine;

    public float flashLightRadius
    {
        get => _flashLightRadius;
        set
        {
            float oldValue = _flashLightRadius;
            _flashLightRadius = value;

            // Останавливаем предыдущую анимацию если она есть
            if (_radiusChangeCoroutine != null)
                StopCoroutine(_radiusChangeCoroutine);

            // Запускаем новую анимацию
            _radiusChangeCoroutine = StartCoroutine(SmoothRadiusChange(oldValue, value));
            SaveData();
        }
    }

    public float flashLightIntensity
    {
        get => _flashLightIntensity;
        set
        {
            float oldValue = _flashLightIntensity;
            _flashLightIntensity = value;

            // Останавливаем предыдущую анимацию если она есть
            if (_intensityChangeCoroutine != null)
                StopCoroutine(_intensityChangeCoroutine);

            // Запускаем новую анимацию
            _intensityChangeCoroutine = StartCoroutine(SmoothIntensityChange(oldValue, value));
            SaveData();
        }
    }

    private IEnumerator SmoothRadiusChange(float from, float to, float duration = 1f)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Плавное изменение радиуса прожектора
            _flashLight.pointLightOuterRadius = Mathf.Lerp(from, to, t);

            yield return null;
        }

        // Гарантируем, что достигли конечного значения
        _flashLight.pointLightOuterRadius = to;
        _radiusChangeCoroutine = null;
    }

    // Метод для вызова извне с определенной длительностью
    public void SetFlashlightRadiusSmooth(float newRadius, float changeDuration = 1f)
    {
        flashLightRadius = newRadius;
        if (_radiusChangeCoroutine != null)
            StopCoroutine(_radiusChangeCoroutine);
        _radiusChangeCoroutine = StartCoroutine(SmoothRadiusChange(_flashLight.pointLightOuterRadius, newRadius, changeDuration));
    }

    private IEnumerator SmoothIntensityChange(float from, float to, float duration = 1f)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Плавное изменение радиуса прожектора
            _flashLight.intensity = Mathf.Lerp(from, to, t);

            yield return null;
        }

        // Гарантируем, что достигли конечного значения
        _flashLight.intensity = to;
        _intensityChangeCoroutine = null;
    }

    // Метод для вызова извне с определенной длительностью
    public void SetFlashlightIntensitySmooth(float newIntensity, float changeDuration = 1f)
    {
        flashLightIntensity = newIntensity;
        if (_intensityChangeCoroutine != null)
            StopCoroutine(_intensityChangeCoroutine);
        _intensityChangeCoroutine = StartCoroutine(SmoothIntensityChange(_flashLight.intensity, newIntensity, changeDuration));
    }
    #endregion

    [Header("Death Settings")]
    [SerializeField] private Image deathImage;
    [SerializeField] private TextMeshProUGUI deathText;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float maxAlpha = 1f;

    #region Initialize
    private void Awake()
    {
        // Простая реализация синглтона
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeData();

            SetFlashlightRadiusSmooth(_flashLightRadius, 1f);
            SetFlashlightIntensitySmooth(_flashLightIntensity, 1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void InitializeData()
    {
        // Создаем экземпляры классов
        Attributes = new PlayerAttributes();
        Stats = GetComponent<PlayerStats>() ?? gameObject.AddComponent<PlayerStats>();

        // Инициализируем Stats, передавая ему Attributes
        Stats.Initialize(Attributes);
        // Подписываемся на событие смерти из Stats
        Stats.OnDeath += HandleDeath;
        // Загружаем данные
        LoadData();
    }
    #endregion

    #region SaveLoadMethods
    // Метод для быстрого сброса к дефолтным значениям (для тестирования)
    [ContextMenu("Reset Data")]
    public void ResetToDefault()
    {
        // Устанавливаем базовые значения
        Attributes.endurance.BaseValue = 1;
        Attributes.strength.BaseValue = 1;
        Attributes.agility.BaseValue = 1;
        Attributes.intellect.BaseValue = 1;
        Attributes.charisma.BaseValue = 1;
        Attributes.luck.BaseValue = 1;

        Stats.SetLevel(1);
        Stats.CurrentExp = 0;
        Stats.Money = 2000f;
        Stats.CurrentHealth = Stats.MaxHealth; // Полное здоровье
        Stats.CurrentStamina = Stats.MaxStamina;
        Stats.CurrentWeight = 0;
        _flashLightRadius = 0f;
        _flashLightIntensity = 0f;

        Debug.Log("Data reset to default.");
    }

    [ContextMenu("Save Data")]
    public void SaveData()
    {
        // Сериализуем все данные в один класс для сохранения
        PlayerSaveData saveData = new PlayerSaveData();
        saveData.attributes = Attributes;
        saveData.currentHealth = Stats.CurrentHealth;
        saveData.currentStamina = Stats.CurrentStamina;
        saveData.money = Stats.Money;
        saveData.currentWeight = Stats.CurrentWeight;
        saveData.level = Stats.Level;
        saveData.currentExp = Stats.CurrentExp;
        saveData.unspentSkillPoints = Stats.UnspentSkillPoints;
        saveData.flashLightRadius = _flashLightRadius;
        saveData.flashLightIntensity = _flashLightIntensity;

        string jsonData = JsonUtility.ToJson(saveData, true); // true для красивого форматирования в отладке
        if (PlayerPrefsMigrationManager.Instance != null)
        {
            PlayerPrefsMigrationManager.Instance.RegisterStringPref(_saveKey);
        }
        PlayerPrefs.SetString(_saveKey, jsonData);
        PlayerPrefs.Save(); // Важно вызывать Save()

        //Debug.Log("Game Saved: " + jsonData);
    }

    [ContextMenu("Load Data")]
    public void LoadData()
    {
        if (PlayerPrefs.HasKey(_saveKey))
        {
            string jsonData = PlayerPrefs.GetString(_saveKey);
            PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(jsonData);

            // Восстанавливаем атрибуты
            Attributes = saveData.attributes ?? new PlayerAttributes();

            // Восстанавливаем Stats
            Stats.Initialize(Attributes); // Сначала инициализируем, чтобы подписаться на события и пересчитать статы
            Stats.SetLevel(saveData.level);
            Stats.CurrentExp = saveData.currentExp;
            Stats.UnspentSkillPoints = saveData.unspentSkillPoints;
            Stats.CurrentHealth = saveData.currentHealth;
            Stats.CurrentStamina = saveData.currentStamina;
            Stats.Money = saveData.money;
            Stats.CurrentWeight = saveData.currentWeight;
            _flashLightRadius = saveData.flashLightRadius;
            _flashLightIntensity = saveData.flashLightIntensity;

            //Debug.Log("Game Loaded: " + jsonData);
        }
        else
        {
            Debug.Log("No save data found. Initializing with default values.");
            ResetToDefault();
        }
    }

    // Вызывайте этот метод при выходе из игры или в контрольных точках
    //private void OnApplicationQuit()
    //{
    //    SaveData();
    //}

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveData();
        }
    }
    #endregion

    #region Expirience
    // Методы для работы с опытом и уровнем
    public void AddExperience(int expAmount)
    {
        if (Instance == null) return;

        Stats.AddExp(expAmount);
        Debug.Log($"Added {expAmount} exp. Current: {Stats.CurrentExp}/{Stats.ExpToNextLevel}. Level: {Stats.Level}");

        SaveData();
    }

    public void LevelUp()
    {
        if (Instance == null) return;

        // Добавляем достаточно опыта для следующего уровня
        int neededExp = Stats.ExpToNextLevel - Stats.CurrentExp;
        Stats.AddExp(neededExp);

        SaveData();
    }
    #endregion

    #region Attributes
    // Метод для траты очков улучшений
    public bool SpendSkillPointOnAttribute(System.Func<bool> attributeUpgradeMethod)
    {
        if (Instance == null) return false;

        bool success = Stats.SpendSkillPointOnAttribute(attributeUpgradeMethod);
        if (success)
        {
            SaveData();
        }
        return success;
    }
    // Упрощенные методы для повышения конкретных атрибутов
    public bool UpgradeStrength()
    {
        return SpendSkillPointOnAttribute(() =>
        {
            Attributes.strength.BaseValue++;
            return true;
        });
    }
    public bool UpgradeEndurance()
    {
        return SpendSkillPointOnAttribute(() =>
        {
            Attributes.endurance.BaseValue++;
            return true;
        });
    }
    public bool UpgradeAgility()
    {
        return SpendSkillPointOnAttribute(() =>
        {
            Attributes.agility.BaseValue++;
            return true;
        });
    }
    public bool UpgradeIntellect()
    {
        return SpendSkillPointOnAttribute(() =>
        {
            Attributes.intellect.BaseValue++;
            return true;
        });
    }
    public bool UpgradeCharisma()
    {
        return SpendSkillPointOnAttribute(() =>
        {
            Attributes.charisma.BaseValue++;
            return true;
        });
    }
    public bool UpgradeLuck()
    {
        return SpendSkillPointOnAttribute(() =>
        {
            Attributes.luck.BaseValue++;
            return true;
        });
    }
    #endregion

    #region MoneyAPI
    // Методы для управления деньгами
    public void AddMoney(float amount)
    {
        if (Instance == null) return;

        Stats.Money += amount;
        Debug.Log($"Added {amount} money. Total: {Stats.Money}");

        SaveData();
    }

    public bool SpendMoney(float amount)
    {
        if (Instance == null || Stats.Money < amount) return false;

        Stats.Money -= amount;
        Debug.Log($"Spent {amount} money. Remaining: {Stats.Money}");

        SaveData();
        return true;
    }
    #endregion

    #region BattleAPI
    public void TakeDamage(int damage)
    {
        if (Instance == null) return;

        if(UnityEngine.Random.Range(0,100) < 10)//обходим броню
        {
            Stats.CurrentHealth -= damage;
            Debug.Log($"Нанесено урона: {damage}. Здоровье: {Stats.CurrentHealth}");
        }
        else//ищем броню, которая впитает урон
        {
            Stats.CurrentHealth -= DestroyRandomArmorInFight(damage);
        }

        SaveData();
    }

    private int DestroyRandomArmorInFight(int damage)
    {
        //ItemArmorController foundArmor = new ItemArmorController();
        int remaining = damage;
        if (BattleManager.Instance.isBattleActive)
        {
            var Armors = cellsFight.GetComponentsInChildren<ItemArmorController>().Where(e => e.gameObject.GetComponent<ItemStats>().durability > 0 && e.gameObject.GetComponent<ItemStats>().isUseFight).ToList();
            if (Armors.Count > 0)
            {
                int random = UnityEngine.Random.Range(0, Armors.Count);
                remaining = Armors[random].TakeDamage(damage);
            }
        }
        else if (cellsInventory.gameObject.activeSelf)
        {
            var Armors = cellsInventory.GetComponentsInChildren<ItemArmorController>().Where(e => e.gameObject.GetComponent<ItemStats>().durability > 0 && e.gameObject.GetComponent<ItemStats>().isUseFight).ToList();
            if (Armors.Count > 0)
            {
                int random = UnityEngine.Random.Range(0, Armors.Count);
                remaining = Armors[random].TakeDamage(damage);
            }
        }
        return remaining;
    }
    public void Heal(int countPoint)
    {
        if (Instance == null) return;

        Stats.CurrentHealth += countPoint;
        Debug.Log($"Вылечено: {countPoint}. Здоровье: {Stats.CurrentHealth}");

        SaveData();
    }
    #endregion

    #region Death Handling
    private void HandleDeath()
    {
        Time.timeScale = 0f;

        // Запускаем процесс смерти
        StartDeathSequence();
    }

    private void StartDeathSequence()
    {
        //Запускаем анимацию смерти
        PlayDeathAnimation();
    }

    private void PlayDeathAnimation()
    {
        var animationController = GetComponent<NPCAnimationController>();
        var playerController = GetComponent<TopDownCharacterController>();
        // Запускаем анимацию смерти
        animationController.animator.SetBool("IsDead", true);
        // Отключаем анимацию движения
        animationController.enabled = false;
        playerController.enabled = false;

        //корутина для появления UI элементов
        StartCoroutine(FadeInUICoroutine());
    }

    private System.Collections.IEnumerator FadeInUICoroutine()
    {
        // Проверяем наличие UI элементов
        if (deathImage == null && deathText == null)
        {
            yield break;
        }

        // Инициализируем прозрачность в 0
        if (deathImage != null)
        {
            Color imageColor = deathImage.color;
            deathImage.color = new Color(imageColor.r, imageColor.g, imageColor.b, 0f);
            deathImage.gameObject.SetActive(true);
        }

        if (deathText != null)
        {
            Color textColor = deathText.color;
            deathText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
            deathText.gameObject.SetActive(true);
        }

        // Плавное увеличение прозрачности
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(0f, maxAlpha, elapsedTime / fadeInDuration);

            // Применяем альфу к Image
            if (deathImage != null)
            {
                Color imageColor = deathImage.color;
                deathImage.color = new Color(imageColor.r, imageColor.g, imageColor.b, currentAlpha);
            }

            // Применяем альфу к TextMeshPro
            if (deathText != null)
            {
                Color textColor = deathText.color;
                deathText.color = new Color(textColor.r, textColor.g, textColor.b, currentAlpha);
            }

            yield return null;
        }

        // Убеждаемся, что в конце альфа равна максимальному значению
        if (deathImage != null)
        {
            Color imageColor = deathImage.color;
            deathImage.color = new Color(imageColor.r, imageColor.g, imageColor.b, maxAlpha);
        }

        if (deathText != null)
        {
            Color textColor = deathText.color;
            deathText.color = new Color(textColor.r, textColor.g, textColor.b, maxAlpha);
        }
    }

    #endregion
}




// Вспомогательный класс, который будет сериализоваться в JSON
[System.Serializable]
public class PlayerSaveData
{
    public PlayerAttributes attributes;
    public int currentHealth;
    public float currentStamina;
    public float money;
    public float currentWeight;
    public int level;
    public int currentExp;
    public int unspentSkillPoints;
    public float flashLightRadius;
    public float flashLightIntensity;
}