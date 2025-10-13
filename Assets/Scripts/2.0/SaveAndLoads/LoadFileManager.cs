using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadFileManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform contentPanel;
    [SerializeField] private GameObject saveFilePrefab;

    private string SaveFolderPath => Application.persistentDataPath;

    void Start()
    {
        RefreshSaveFilesList();
    }

    public void RefreshSaveFilesList()
    {
        // Очищаем панель
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // Создаем папку если не существует
        if (!Directory.Exists(SaveFolderPath))
        {
            Directory.CreateDirectory(SaveFolderPath);
            return;
        }

        // Находим все json файлы
        var jsonFiles = Directory.GetFiles(SaveFolderPath, "*.json")
                                .OrderByDescending(f => File.GetLastWriteTime(f))
                                .ToArray();

        // Создаем префабы для каждого файла
        foreach (var filePath in jsonFiles)
        {
            CreateLoadFileEntry(filePath);
        }
    }

    private void CreateLoadFileEntry(string filePath)
    {
        var saveFileEntry = Instantiate(saveFilePrefab, contentPanel);
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var lastModified = File.GetLastWriteTime(filePath);

        // Находим компоненты Text
        var fileNameText = saveFileEntry.transform.Find("FileName")?.GetComponent<TextMeshProUGUI>();
        var fileDateText = saveFileEntry.transform.Find("FileDateChange")?.GetComponent<TextMeshProUGUI>();

        if (fileNameText != null)
            fileNameText.text = fileName;

        if (fileDateText != null)
            fileDateText.text = lastModified.ToString("dd.MM.yyyy HH:mm");

        // Находим кнопки
        var loadButton = saveFileEntry.transform.Find("ButtonLoad")?.GetComponent<Button>();
        var deleteButton = saveFileEntry.transform.Find("ButtonDelete")?.GetComponent<Button>();

        // Вешаем обработчики
        if (loadButton != null)
            loadButton.onClick.AddListener(() => LoadGame(filePath));

        if (deleteButton != null)
            deleteButton.onClick.AddListener(() => DeleteSaveFile(filePath, saveFileEntry));
    }

    private void LoadGame(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"Файл не найден: {filePath}");
                RefreshSaveFilesList();
                return;
            }

            Debug.Log($"Загрузка игры из: {filePath}");

            // Пробуем обычный менеджер
            //if (LoadingSceneManager.Instance != null)
            //{
            //    LoadingSceneManager.Instance.LoadGameWithSaveFile(filePath, "MainGame");
            //}
            //else
            
            //Debug.LogError("LoadingSceneManager не найден! Пробуем ультра-простой...");

            // Пробуем ультра-простой менеджер
            if (UltraSimpleLoadingManager.Instance != null)
            {
                UltraSimpleLoadingManager.Instance.LoadGameWithSave(filePath);
            }
            else
            {
                // Последний вариант - прямая загрузка
                Debug.LogWarning("Прямая загрузка MainGame");
                SceneManager.LoadScene("MainGame");
            }
            
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка при загрузке файла {filePath}: {e.Message}");
        }
    }

    private void DeleteSaveFile(string filePath, GameObject fileEntry)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Destroy(fileEntry);
                Debug.Log($"Файл удален: {Path.GetFileName(filePath)}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка при удалении файла {filePath}: {e.Message}");
        }
    }
}