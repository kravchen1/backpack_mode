using UnityEngine;

public class DynamicCameraSize : MonoBehaviour
{
    [Header("Base Resolution Settings")]
    public int baseWidth = 960;
    public int baseHeight = 540;
    public int pixelsPerUnit = 32;

    [Header("World Boundaries (in World Units)")]
    public Vector2 baseMinBoundary = new Vector2(155f, 58f);
    public Vector2 baseMaxBoundary = new Vector2(1735f, 980f);

    [Header("Camera Settings")]
    public float maxSizeMultiplier = 2.5f;

    [Header("Boundary Scaling Mode")]
    public bool scaleBoundariesWithResolution = true;
    public float boundaryScaleFactor = 1.0f;

    [Header("Debug")]
    public bool logChanges = true;

    private Camera _camera;
    private int _lastScreenWidth;
    private int _lastScreenHeight;
    private TopDownCameraScrollController _cameraScrollController;
    private TopDownCameraFollowController _cameraFollowController;
    private TopDownCharacterController _playerController;

    void Start()
    {
        _camera = GetComponent<Camera>();
        _lastScreenWidth = Screen.width;
        _lastScreenHeight = Screen.height;

        // Получаем компоненты
        _cameraScrollController = GetComponent<TopDownCameraScrollController>();
        _cameraFollowController = GetComponent<TopDownCameraFollowController>();
        _playerController = GameObject.FindGameObjectWithTag("Player")?.GetComponent<TopDownCharacterController>();

        if (_cameraScrollController == null)
        {
            Debug.LogWarning("TopDownCameraScrollController not found on camera!");
        }

        if (_cameraFollowController == null)
        {
            Debug.LogError("TopDownCameraFollowController not found on camera!");
            return;
        }

        if (_playerController == null)
        {
            Debug.LogError("Player with TopDownCharacterController not found!");
            return;
        }

        UpdateCameraSize(true);
    }

    void Update()
    {
        if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
        {
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
            UpdateCameraSize();
        }
    }

    void UpdateCameraSize(bool forceUpdate = false)
    {
        if (_cameraFollowController == null || _playerController == null) return;

        // 1. РАССЧИТЫВАЕМ БАЗОВЫЙ РАЗМЕР КАМЕРЫ
        float baseWorldHeight = (float)baseHeight / pixelsPerUnit;
        float baseOrthoSize = baseWorldHeight / 4f;

        // 2. ОПРЕДЕЛЯЕМ МАСШТАБ РАЗРЕШЕНИЯ
        float resolutionScale = (float)Screen.height / baseHeight;

        // 3. КОРРЕКТИРУЕМ РАЗМЕР КАМЕРЫ
        float newOrthoSize = baseOrthoSize * resolutionScale;

        // 4. ОБНОВЛЯЕМ НАСТРОЙКИ КАМЕРЫ
        _camera.orthographicSize = newOrthoSize;

        if (_cameraScrollController != null)
        {
            _cameraScrollController.minOrthographicSize = newOrthoSize;
            _cameraScrollController.maxOrthographicSize = newOrthoSize * maxSizeMultiplier;
        }

        // 5. ОБНОВЛЯЕМ СКОРОСТЬ ПЕРСОНАЖА
        PlayerDataManager.Instance.Stats.BaseMoveSpeed *= newOrthoSize;

        // 6. ОБНОВЛЯЕМ ГРАНИЦЫ КАМЕРЫ
        UpdateCameraBoundaries(resolutionScale);

        // 7. Логирование
        if (logChanges || forceUpdate)
        {
            Debug.Log($"New Resolution: {Screen.width}x{Screen.height} " +
                      $"(Scale: x{resolutionScale:F2}) | " +
                      $"Camera Size: {newOrthoSize:F2} | ");
        }
    }

    void UpdateCameraBoundaries(float resolutionScale)
    {
        if (_cameraFollowController == null) return;

        Vector2 newMinBoundary = baseMinBoundary;
        Vector2 newMaxBoundary = baseMaxBoundary;

        if (scaleBoundariesWithResolution)
        {
            // Масштабируем границы пропорционально изменению разрешения
            float scale = resolutionScale * boundaryScaleFactor;

            newMinBoundary = baseMinBoundary * scale;
            newMaxBoundary = baseMaxBoundary * scale;

            // Альтернативный подход: центрируем границы и масштабируем от центра
            // Vector2 center = (baseMinBoundary + baseMaxBoundary) / 2f;
            // Vector2 halfSize = (baseMaxBoundary - baseMinBoundary) / 2f * scale;
            // newMinBoundary = center - halfSize;
            // newMaxBoundary = center + halfSize;
        }

        // Устанавливаем новые границы
        _cameraFollowController.SetBoundaries(newMinBoundary, newMaxBoundary);

        if (logChanges)
        {
            Debug.Log($"Camera Boundaries: Min({newMinBoundary.x:F1}, {newMinBoundary.y:F1}), " +
                      $"Max({newMaxBoundary.x:F1}, {newMaxBoundary.y:F1})");
        }
    }

    /// <summary>
    /// Устанавливает базовые границы (для разрешения 1920x1080)
    /// </summary>
    public void SetBaseBoundaries(Vector2 minBoundary, Vector2 maxBoundary)
    {
        baseMinBoundary = minBoundary;
        baseMaxBoundary = maxBoundary;
        UpdateCameraBoundaries((float)Screen.height / baseHeight);
    }

    /// <summary>
    /// Включает/выключает масштабирование границ
    /// </summary>
    public void SetBoundaryScaling(bool enabled, float scaleFactor = 1.0f)
    {
        scaleBoundariesWithResolution = enabled;
        boundaryScaleFactor = scaleFactor;
        UpdateCameraBoundaries((float)Screen.height / baseHeight);
    }

    /// <summary>
    /// Для отладки: сброс к базовым границам 1920x1080
    /// </summary>
    public void ResetToBaseBoundaries()
    {
        UpdateCameraBoundaries(1.0f); // Масштаб 1.0 = 1920x1080
    }

    /// <summary>
    /// Метод для принудительного обновления всех параметров
    /// </summary>
    public void ForceUpdate()
    {
        _lastScreenWidth = Screen.width;
        _lastScreenHeight = Screen.height;
        UpdateCameraSize(true);
    }

    /// <summary>
    /// Возвращает текущий масштаб относительно базового разрешения
    /// </summary>
    public float GetCurrentScale()
    {
        return (float)Screen.height / baseHeight;
    }
}