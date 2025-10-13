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

    private static PlayerPrefsMigrationManager _instance;
    private PlayerPrefsData _prefsData;

    public static PlayerPrefsMigrationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerPrefsMigrationManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("PlayerPrefsMigrationManager");
                    _instance = go.AddComponent<PlayerPrefsMigrationManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        _prefsData = new PlayerPrefsData();
        Debug.Log("PlayerPrefsMigrationManager инициализирован");
    }

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
                ApplyToPlayerPrefs();
                Debug.Log($"Данные импортированы из: {_savePath}");
            }
            else
            {
                Debug.LogWarning($"Файл не найден: {_savePath}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка импорта: {e.Message}");
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
        Debug.Log($"Данные экспортированы в: {_savePath}");
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
}