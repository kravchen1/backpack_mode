using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneHistory
{
    public static string lastSceneName = string.Empty;
}

public class PersistentDropZone : MonoBehaviour
{
    private static PersistentDropZone instance;
    private static string[] persistentScenes = { "Cave", "GenerateMap", "BackPack" };

    private void Awake()
    {
        // Регистрируем обработчик загрузки сцен
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Если это дубликат на сцене, где объект должен сохраняться — уничтожаем
        if (instance != null && IsPersistentScene(SceneManager.GetActiveScene().name))
        {
            Destroy(gameObject);
            return;
        }

        // Переносим в DontDestroyOnLoad только для GenerateMap,Cave и BackPack
        if (IsPersistentScene(SceneManager.GetActiveScene().name))
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneHistory.lastSceneName == "Cave" && scene.name == "Cave" && instance == this)
        {
            ClearAllChildren();
            //Destroy(gameObject); // Удаляем из DontDestroyOnLoad
            //instance = null;
            return;
        }
        // Если это НЕ сцена, где объект должен сохраняться — удаляем
        if (!IsPersistentScene(scene.name) && instance == this)
        {
            SceneHistory.lastSceneName = scene.name;
            Destroy(gameObject);
            return;
        }

        // Отключаем детей, если это BackPack
        if (scene.name == "BackPack")
        {
            SetChildrenActive(false);
        }
        else
        {
            SetChildrenActive(true);
        }
        SceneHistory.lastSceneName = scene.name;
    }

    private bool IsPersistentScene(string sceneName)
    {
        return System.Array.Exists(persistentScenes, scene => scene == sceneName);
    }

    private void SetChildrenActive(bool isActive)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }

    private void ClearAllChildren()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от события, чтобы не было утечек памяти
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}