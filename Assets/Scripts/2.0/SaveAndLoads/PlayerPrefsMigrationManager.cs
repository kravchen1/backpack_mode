using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerPrefsMigrationManager : MonoBehaviour
{
    [Header("PlayerPrefs Keys to Manage")]
    [SerializeField] private List<string> intKeys = new List<string>();
    [SerializeField] private List<string> floatKeys = new List<string>();
    [SerializeField] private List<string> stringKeys = new List<string>();

    public string _savePath;

    public static PlayerPrefsMigrationManager Instance;
    private PlayerPrefsData _prefsData;

    //public static PlayerPrefsMigrationManager Instance
    //{
    //    get
    //    {
    //        if (_instance == null)
    //        {
    //            _instance = FindFirstObjectByType<PlayerPrefsMigrationManager>();
    //            if (_instance == null)
    //            {
    //                GameObject go = new GameObject("PlayerPrefsMigrationManager");
    //                _instance = go.AddComponent<PlayerPrefsMigrationManager>();
    //                DontDestroyOnLoad(go);
    //            }
    //        }
    //        return _instance;
    //    }
    //}

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        if (!string.IsNullOrEmpty(_savePath))
        {
            _savePath = Path.Combine(Application.persistentDataPath, _savePath + ".json");
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _prefsData = new PlayerPrefsData();
        //Debug.Log("PlayerPrefsMigrationManager инициализирован");
    }

    // Новые методы для автоматической регистрации PlayerPrefs
    public void RegisterIntPref(string key, int defaultValue = 0)
    {
        if (!intKeys.Contains(key))
        {
            intKeys.Add(key);
            //Debug.Log($"Зарегистрирован Int PlayerPref: {key}");
        }

        // Автоматически устанавливаем значение по умолчанию, если ключ не существует
        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetInt(key, defaultValue);
        }
    }

    public void RegisterFloatPref(string key, float defaultValue = 0f)
    {
        if (!floatKeys.Contains(key))
        {
            floatKeys.Add(key);
            //Debug.Log($"Зарегистрирован Float PlayerPref: {key}");
        }

        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetFloat(key, defaultValue);
        }
    }

    public void RegisterStringPref(string key, string defaultValue = "")
    {
        if (!stringKeys.Contains(key))
        {
            stringKeys.Add(key);
            Debug.Log($"Зарегистрирован String PlayerPref: {key}");
        }

        if (!PlayerPrefs.HasKey(key))
        {
            PlayerPrefs.SetString(key, defaultValue);
        }
    }

    // Удобный метод для массовой регистрации
    public void RegisterMultiplePrefs(Dictionary<string, int> intPrefs = null,
                                     Dictionary<string, float> floatPrefs = null,
                                     Dictionary<string, string> stringPrefs = null)
    {
        if (intPrefs != null)
        {
            foreach (var pref in intPrefs)
            {
                RegisterIntPref(pref.Key, pref.Value);
            }
        }

        if (floatPrefs != null)
        {
            foreach (var pref in floatPrefs)
            {
                RegisterFloatPref(pref.Key, pref.Value);
            }
        }

        if (stringPrefs != null)
        {
            foreach (var pref in stringPrefs)
            {
                RegisterStringPref(pref.Key, pref.Value);
            }
        }
    }

    [ContextMenu("Import From JSON")]
    [ContextMenu("Import From JSON")]
    public void ImportFromJson()
    {
        if (string.IsNullOrEmpty(_savePath))
        {
            _savePath = Path.Combine(Application.persistentDataPath, "playerprefs_backup.json");
        }

        try
        {
            if (File.Exists(_savePath))
            {
                string json = File.ReadAllText(_savePath);
                _prefsData = JsonUtility.FromJson<PlayerPrefsData>(json);

                // Автоматически регистрируем ключи из импортированных данных
                RegisterImportedKeys();

                ApplyToPlayerPrefs();
               // Debug.Log($"Данные импортированы из: {_savePath}");
            }
            else
            {
               // Debug.LogWarning($"Файл не найден: {_savePath}");
            }
        }
        catch (Exception e)
        {
            //Debug.LogError($"Ошибка импорта: {e.Message}");
        }
    }

    // Новый метод для регистрации ключей из импортированных данных
    private void RegisterImportedKeys()
    {
        // Регистрируем int ключи
        foreach (var pref in _prefsData.intPrefs)
        {
            if (!intKeys.Contains(pref.key))
            {
                intKeys.Add(pref.key);
                //Debug.Log($"Автоматически зарегистрирован Int PlayerPref из JSON: {pref.key}");
            }
        }

        // Регистрируем float ключи
        foreach (var pref in _prefsData.floatPrefs)
        {
            if (!floatKeys.Contains(pref.key))
            {
                floatKeys.Add(pref.key);
                //Debug.Log($"Автоматически зарегистрирован Float PlayerPref из JSON: {pref.key}");
            }
        }

        // Регистрируем string ключи
        foreach (var pref in _prefsData.stringPrefs)
        {
            if (!stringKeys.Contains(pref.key))
            {
                stringKeys.Add(pref.key);
                //Debug.Log($"Автоматически зарегистрирован String PlayerPref из JSON: {pref.key}");
            }
        }
    }

    [ContextMenu("Export To JSON")]
    public void ExportToJson()
    {
        if (string.IsNullOrEmpty(_savePath))
        {
            _savePath = Path.Combine(Application.persistentDataPath, "playerprefs_backup.json");
        }

        CollectCurrentPlayerPrefs();
        SaveToJsonFile();
        //Debug.Log($"Данные экспортированы в: {_savePath}");
    }

    private void CollectCurrentPlayerPrefs()
    {
        _prefsData.intPrefs.Clear();
        _prefsData.floatPrefs.Clear();
        _prefsData.stringPrefs.Clear();

        foreach (string key in intKeys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                _prefsData.intPrefs.Add(new IntPref(key, PlayerPrefs.GetInt(key)));
            }
        }

        foreach (string key in floatKeys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                _prefsData.floatPrefs.Add(new FloatPref(key, PlayerPrefs.GetFloat(key)));
            }
        }

        foreach (string key in stringKeys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                _prefsData.stringPrefs.Add(new StringPref(key, PlayerPrefs.GetString(key)));
            }
        }

        _prefsData.saveTime = DateTime.Now;
    }

    private void ApplyToPlayerPrefs()
    {
        foreach (var pref in _prefsData.intPrefs)
        {
            PlayerPrefs.SetInt(pref.key, pref.value);
        }

        foreach (var pref in _prefsData.floatPrefs)
        {
            PlayerPrefs.SetFloat(pref.key, pref.value);
        }

        foreach (var pref in _prefsData.stringPrefs)
        {
            PlayerPrefs.SetString(pref.key, pref.value);
        }

        PlayerPrefs.Save();
    }

    private void SaveToJsonFile()
    {
        try
        {
            string json = JsonUtility.ToJson(_prefsData, true);
            File.WriteAllText(_savePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка экспорта: {e.Message}");
        }
    }


    private void OnApplicationQuit()
    {
        //var dayManager = FindFirstObjectByType<DayManager>();
        //dayManager.SaveGameData();
        ExportToJson();
    }
}