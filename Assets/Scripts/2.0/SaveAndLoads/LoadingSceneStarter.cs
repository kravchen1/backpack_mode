using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneStarter : MonoBehaviour
{
    void Start()
    {
        Debug.Log("LoadingSceneStarter: Сцена загрузки запущена");

        // Уведомляем менеджер что сцена загрузки готова
        if (UltraSimpleLoadingManager.Instance != null)
        {
            UltraSimpleLoadingManager.Instance.OnLoadingSceneLoaded();
        }
        else
        {
            Debug.LogError("UltraSimpleLoadingManager не найден!");
            // Аварийная загрузка
            SceneManager.LoadScene("MainGame");
        }
    }
}