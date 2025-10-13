using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UltraSimpleLoadingManager : MonoBehaviour
{
    private static UltraSimpleLoadingManager _instance;
    private string _saveFilePath;

    public static UltraSimpleLoadingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("UltraSimpleLoadingManager");
                _instance = go.AddComponent<UltraSimpleLoadingManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public void LoadGameWithSave(string saveFilePath)
    {
        _saveFilePath = saveFilePath;
        Debug.Log($"UltraSimple: Загружаем с файла {saveFilePath}");

        // Сразу переходим на сцену загрузки
        SceneManager.LoadScene("LoadingScene");
    }

    // Этот метод должен вызываться из сцены LoadingScene после её загрузки
    public void OnLoadingSceneLoaded()
    {
        Debug.Log("UltraSimple: Сцена загрузки загружена, начинаем процесс");
        StartCoroutine(LoadingProcess());
    }

    private IEnumerator LoadingProcess()
    {
        Debug.Log("UltraSimple: Шаг 1 - Загрузка данных");

        // Загружаем данные
        if (PlayerPrefsMigrationManager.Instance != null)
        {
            PlayerPrefsMigrationManager.Instance._savePath = _saveFilePath;
            PlayerPrefsMigrationManager.Instance.ImportFromJson();
        }

        yield return new WaitForSeconds(1f); // Имитация загрузки

        Debug.Log("UltraSimple: Шаг 2 - Загрузка основной сцены");

        // Загружаем основную сцену
        SceneManager.LoadScene("MainGame");

        Debug.Log("UltraSimple: Процесс завершен");
    }
}