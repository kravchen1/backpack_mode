using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveFileManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform contentPanel;
    [SerializeField] private GameObject saveFilePrefab;
    [SerializeField] private TMP_InputField inputField;

    private string SaveFolderPath => Application.persistentDataPath;

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
            CreateSaveFileEntry(filePath);
        }
    }

    private void CreateSaveFileEntry(string filePath)
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
        var loadButton = saveFileEntry.transform.Find("ButtonRewrite")?.GetComponent<Button>();
        var deleteButton = saveFileEntry.transform.Find("ButtonDelete")?.GetComponent<Button>();

        // Вешаем обработчики
        if (loadButton != null)
            loadButton.onClick.AddListener(() => RewriteGame(filePath));

        if (deleteButton != null)
            deleteButton.onClick.AddListener(() => DeleteSaveFile(filePath, saveFileEntry));
    }

    private void RewriteGame(string filePath)
    {
        PlayerPrefsMigrationManager.Instance._savePath = Path.Combine(Application.persistentDataPath, filePath);
        PlayerPrefsMigrationManager.Instance.ExportToJson();
        RefreshSaveFilesList();
    }

    public void SaveGame()
    {
        GameObject.FindGameObjectWithTag("Player")?.GetComponent<TopDownCharacterController>().ForceSavePosition();
        GridObjectManager.Instance.SaveWorldData();
        GameObject.Find("CanvasInventory")?.transform.GetChild(0).GetComponent<CellsData>().SaveData();

        PlayerPrefsMigrationManager.Instance._savePath = Path.Combine(Application.persistentDataPath, inputField.text + ".json");
        PlayerPrefsMigrationManager.Instance.ExportToJson();
        RefreshSaveFilesList();
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