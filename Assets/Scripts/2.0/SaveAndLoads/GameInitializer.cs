using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    void Awake()
    {
        // Принудительно создаем инстансы при старте игры
        var loadingManager = UltraSimpleLoadingManager.Instance;
        var migrationManager = PlayerPrefsMigrationManager.Instance;

        Debug.Log($"LoadingSceneManager создан: {loadingManager != null}");
        Debug.Log($"PlayerPrefsMigrationManager создан: {migrationManager != null}");
    }
}