using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public enum TimeFormat
{
    Format24H,
    Format12H
}

public class DayManager : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private TimeFormat timeFormat = TimeFormat.Format24H;
    [SerializeField] private float realSecondsPerGameDay = 300f; // 5 минут реального времени = 1 игровой день
    [SerializeField] private float startHour = 6f; // 06:30
    [SerializeField] private float startMinute = 30f;

    [Header("Date Settings")]
    [SerializeField] private int startYear = 3113;
    [SerializeField] private int startMonth = 8;
    [SerializeField] private int startDay = 26;

    [Header("Light Settings")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private float minLightIntensity = 0.01f;
    [SerializeField] private float maxLightIntensity = 0.25f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dateText;

    [Header("Save Settings")]
    [SerializeField] private bool enableAutoSave = true;

    // Текущее игровое время (0-1, где 0 = начало дня, 1 = конец дня)
    private float currentTimeOfDay = 0f;
    private System.DateTime currentDate;

    // Ключи для сохранения
    private const string SAVE_TIME_KEY = "GameTimeOfDay";
    private const string SAVE_YEAR_KEY = "GameYear";
    private const string SAVE_MONTH_KEY = "GameMonth";
    private const string SAVE_DAY_KEY = "GameDay";
    private const string SAVE_TIMEFORMAT_KEY = "TimeFormat";

    // Событие для уведомления других систем о смене дня
    public System.Action OnDayChanged;

    private void Start()
    {
        // Попытка загрузки сохраненных данных
        if (!LoadGameData())
        {
            // Если сохранений нет, инициализация начальными значениями
            InitializeNewGame();
        }

        UpdateUI();
        UpdateLighting();
    }

    private void Update()
    {
        // Обновление времени
        float timeDelta = Time.deltaTime / realSecondsPerGameDay;
        currentTimeOfDay += timeDelta;

        // Проверка смены дня
        if (currentTimeOfDay >= 1f)
        {
            currentTimeOfDay -= 1f;
            AdvanceDay();
        }

        UpdateUI();
        UpdateLighting();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && enableAutoSave)
        {
            SaveGameData();
        }
    }

    private void OnApplicationQuit()
    {
        if (enableAutoSave)
        {
            SaveGameData();
        }
    }

    private void InitializeNewGame()
    {
        currentDate = new System.DateTime(startYear, startMonth, startDay);
        currentTimeOfDay = (startHour * 3600f + startMinute * 60f) / 86400f;

        // Сохраняем начальные настройки
        SaveGameData();
    }

    private void AdvanceDay()
    {
        currentDate = currentDate.AddDays(1);
        OnDayChanged?.Invoke();

        // Автосохранение при смене дня
        if (enableAutoSave)
        {
            SaveGameData();
        }
    }

    private void UpdateUI()
    {
        // Обновление времени
        if (timeText != null)
        {
            timeText.text = GetFormattedTime();
        }

        // Обновление даты
        if (dateText != null)
        {
            dateText.text = GetFormattedDate();
        }
    }

    private string GetFormattedTime()
    {
        float totalSeconds = currentTimeOfDay * 86400f;
        int hours = Mathf.FloorToInt(totalSeconds / 3600f);
        int minutes = Mathf.FloorToInt((totalSeconds % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);

        if (timeFormat == TimeFormat.Format12H)
        {
            string period = hours >= 12 ? "PM" : "AM";
            int twelveHour = hours % 12;
            if (twelveHour == 0) twelveHour = 12;

            return $"{twelveHour:D2}:{minutes:D2}:{seconds:D2} {period}";
        }
        else
        {
            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }
    }

    private string GetFormattedDate()
    {
        string dayOfWeek = currentDate.ToString("dddd");
        return $"{dayOfWeek}, {currentDate:yyyy}";
    }

    private void UpdateLighting()
    {
        if (globalLight != null)
        {
            float lightIntensity = CalculateLightIntensity(currentTimeOfDay);
            globalLight.intensity = Mathf.Lerp(minLightIntensity, maxLightIntensity, lightIntensity);
        }
    }

    private float CalculateLightIntensity(float timeOfDay)
    {
        float dawnStart = 0.208f; // 05:00
        float dawnEnd = 0.48f;   // 12:00
        float duskStart = 0.75f;  // 18:00
        float duskEnd = 0.88f;   // 23:00

        if (timeOfDay >= dawnStart && timeOfDay <= dawnEnd)
        {
            return Mathf.InverseLerp(dawnStart, dawnEnd, timeOfDay);
        }
        else if (timeOfDay >= duskStart && timeOfDay <= duskEnd)
        {
            return 1f - Mathf.InverseLerp(duskStart, duskEnd, timeOfDay);
        }
        else if (timeOfDay > dawnEnd && timeOfDay < duskStart)
        {
            return 1f;
        }
        else
        {
            return 0f;
        }
    }

    #region Save/Load Methods

    /// <summary>
    /// Сохраняет текущее состояние игры
    /// </summary>
    public void SaveGameData()
    {
        try
        {
            PlayerPrefs.SetFloat(SAVE_TIME_KEY, currentTimeOfDay);
            PlayerPrefs.SetInt(SAVE_YEAR_KEY, currentDate.Year);
            PlayerPrefs.SetInt(SAVE_MONTH_KEY, currentDate.Month);
            PlayerPrefs.SetInt(SAVE_DAY_KEY, currentDate.Day);
            PlayerPrefs.SetInt(SAVE_TIMEFORMAT_KEY, (int)timeFormat);

            PlayerPrefs.Save();
            Debug.Log("Game data saved successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game data: {e.Message}");
        }
    }

    /// <summary>
    /// Загружает сохраненное состояние игры
    /// </summary>
    /// <returns>True если загрузка успешна, false если нет сохранений</returns>
    public bool LoadGameData()
    {
        try
        {
            // Проверяем есть ли сохраненные данные
            if (!PlayerPrefs.HasKey(SAVE_TIME_KEY))
            {
                Debug.Log("No saved game data found");
                return false;
            }

            // Загружаем время
            currentTimeOfDay = PlayerPrefs.GetFloat(SAVE_TIME_KEY, 0f);

            // Загружаем дату
            int year = PlayerPrefs.GetInt(SAVE_YEAR_KEY, startYear);
            int month = PlayerPrefs.GetInt(SAVE_MONTH_KEY, startMonth);
            int day = PlayerPrefs.GetInt(SAVE_DAY_KEY, startDay);

            // Проверяем валидность даты
            if (IsValidDate(year, month, day))
            {
                currentDate = new System.DateTime(year, month, day);
            }
            else
            {
                Debug.LogWarning("Invalid date in save data, using default date");
                currentDate = new System.DateTime(startYear, startMonth, startDay);
            }

            // Загружаем формат времени
            int savedTimeFormat = PlayerPrefs.GetInt(SAVE_TIMEFORMAT_KEY, (int)timeFormat);
            timeFormat = (TimeFormat)savedTimeFormat;

            Debug.Log("Game data loaded successfully");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game data: {e.Message}");
            InitializeNewGame();
            return false;
        }
    }

    /// <summary>
    /// Проверяет валидность даты
    /// </summary>
    private bool IsValidDate(int year, int month, int day)
    {
        if (year < 1 || month < 1 || month > 12 || day < 1)
            return false;

        return day <= System.DateTime.DaysInMonth(year, month);
    }

    /// <summary>
    /// Удаляет все сохраненные данные
    /// </summary>
    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteKey(SAVE_TIME_KEY);
        PlayerPrefs.DeleteKey(SAVE_YEAR_KEY);
        PlayerPrefs.DeleteKey(SAVE_MONTH_KEY);
        PlayerPrefs.DeleteKey(SAVE_DAY_KEY);
        PlayerPrefs.DeleteKey(SAVE_TIMEFORMAT_KEY);
        PlayerPrefs.Save();

        Debug.Log("Save data deleted");
    }

    /// <summary>
    /// Возвращает информацию о последнем сохранении
    /// </summary>
    public string GetSaveInfo()
    {
        if (PlayerPrefs.HasKey(SAVE_TIME_KEY))
        {
            float savedTime = PlayerPrefs.GetFloat(SAVE_TIME_KEY);
            int hours = Mathf.FloorToInt(savedTime * 24f);
            int minutes = Mathf.FloorToInt((savedTime * 24f - hours) * 60f);

            return $"Last save: {hours:D2}:{minutes:D2}, {PlayerPrefs.GetInt(SAVE_DAY_KEY)}.{PlayerPrefs.GetInt(SAVE_MONTH_KEY)}.{PlayerPrefs.GetInt(SAVE_YEAR_KEY)}";
        }
        return "No save data";
    }

    #endregion

    #region Public Methods

    public System.DateTime GetCurrentDate()
    {
        return currentDate;
    }

    public float GetCurrentTimeOfDay()
    {
        return currentTimeOfDay;
    }

    public string GetCurrentTimeString()
    {
        return GetFormattedTime();
    }

    public string GetCurrentDateString()
    {
        return GetFormattedDate();
    }

    public void SetTimeSpeed(float secondsPerDay)
    {
        realSecondsPerGameDay = Mathf.Max(1f, secondsPerDay);
    }

    public void SwitchTimeFormat()
    {
        timeFormat = timeFormat == TimeFormat.Format24H ? TimeFormat.Format12H : TimeFormat.Format24H;
        UpdateUI();

        // Сохраняем настройки формата времени
        if (enableAutoSave)
        {
            PlayerPrefs.SetInt(SAVE_TIMEFORMAT_KEY, (int)timeFormat);
            PlayerPrefs.Save();
        }
    }

    public void SkipToNextDay()
    {
        AdvanceDay();
        currentTimeOfDay = 0f;
        UpdateUI();
        UpdateLighting();
    }

    #endregion
}