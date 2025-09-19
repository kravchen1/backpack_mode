using UnityEngine;

public class DynamicCameraSize : MonoBehaviour
{
    [Header("Base Resolution Settings")]
    public int baseWidth = 960;
    public int baseHeight = 540;
    public int pixelsPerUnit = 32;

    [Header("Debug")]
    public bool logChanges = true;

    private Camera _camera;
    private int _lastScreenWidth;
    private int _lastScreenHeight;
    private TopDownCameraScrollController topDownCameraScrollController;

    void Start()
    {
        _lastScreenWidth = Screen.width;
        _lastScreenHeight = Screen.height;

        topDownCameraScrollController = GetComponent<TopDownCameraScrollController>();

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
        if (topDownCameraScrollController == null) return;

        // 1. РАССЧИТЫВАЕМ БАЗОВЫЙ РАЗМЕР КАМЕРЫ (для baseHeight)
        // Это наша константа, которая не зависит от текущего разрешения.
        // Это тот размер, при котором игра выглядит "хорошо" на базовом разрешении.
        float baseWorldHeight = (float)baseHeight / pixelsPerUnit;
        float baseOrthoSize = baseWorldHeight / 4f; // ~8.4375 для 540/32

        // 2. ОПРЕДЕЛЯЕМ, НАСКОЛЬКО ТЕКУЩЕЕ РАЗРЕШЕНИЕ БОЛЬШЕ БАЗОВОГО
        // Сравниваем по ВЫСОТЕ, так как orthographicSize управляет именно высотой.
        // ЕСЛИ РАЗРЕШЕНИЕ СТАЛО БОЛЬШЕ -> scaleFactor > 1 -> камеру нужно ОТДАЛИТЬ.
        float resolutionScale = (float)Screen.height / baseHeight;

        // 3. КОРРЕКТИРУЕМ БАЗОВЫЙ РАЗМЕР КАМЕРЫ НА ЭТОТ КОЭФФИЦИЕНТ
        // УМНОЖАЕМ базовый размер на коэффициент, чтобы ОТОДВИНУТЬ камеру
        // при увеличении разрешения.
        float newOrthoSize = baseOrthoSize * resolutionScale;

        // 4. ПРИМЕНЯЕМ НОВЫЙ РАЗМЕР
        //_camera.orthographicSize = newOrthoSize;
        topDownCameraScrollController.minOrthographicSize = newOrthoSize;
        topDownCameraScrollController.maxOrthographicSize = newOrthoSize * 2.5f;
        GameObject.FindGameObjectWithTag("Player").GetComponent<TopDownCharacterController>().moveSpeed = newOrthoSize * 2;

        // 5. Логирование для наглядности
        if (logChanges || forceUpdate)
        {
            Debug.Log($"New Resolution: {Screen.width}x{Screen.height} " +
                      $"(Scale: x{resolutionScale:F2}) | " +
                      $"Camera Size: {newOrthoSize:F4}");
        }
    }
}