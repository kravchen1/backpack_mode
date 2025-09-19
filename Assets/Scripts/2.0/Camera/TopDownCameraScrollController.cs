using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Контроллер камеры для 2D игры с видом сверху
/// Обеспечивает плавное масштабирование колесиком мыши с сохранением настроек
/// </summary>
[RequireComponent(typeof(Camera))]
public class TopDownCameraScrollController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField, Range(1f, 50f)]
    public float minOrthographicSize = 8.4f;

    [SerializeField, Range(1f, 50f)]
    public float maxOrthographicSize = 25f;

    [SerializeField, Range(0.1f, 5f)]
    private float scrollSensitivity = 1f;

    [SerializeField, Range(0.1f, 10f)]
    private float smoothTime = 0.3f;

    [Header("Persistence")]
    [SerializeField]
    private bool saveSettingsBetweenSessions = true;

    private Camera controlledCamera;
    private float targetOrthographicSize;
    private float currentVelocity;
    private string settingsKey;

    private const string ORTHO_SIZE_KEY = "CameraOrthographicSize_";

    private void Awake()
    {
        InitializeComponents();
        InitializeSettings();
    }

    private void InitializeComponents()
    {
        controlledCamera = GetComponent<Camera>();

        if (controlledCamera == null)
        {
            Debug.LogError("TopDownCameraController requires Camera component!");
            enabled = false;
            return;
        }
    }

    private void InitializeSettings()
    {
        settingsKey = ORTHO_SIZE_KEY + SceneManager.GetActiveScene().name;

        // Устанавливаем начальный размер камеры
        if (saveSettingsBetweenSessions && PlayerPrefs.HasKey(settingsKey))
        {
            targetOrthographicSize = PlayerPrefs.GetFloat(settingsKey, minOrthographicSize);
            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, minOrthographicSize, maxOrthographicSize);
        }
        else
        {
            targetOrthographicSize = controlledCamera.orthographicSize;
        }

        controlledCamera.orthographicSize = targetOrthographicSize;
    }

    private void Update()
    {
        HandleCameraScroll();
        SmoothCameraSize();
    }

    private void HandleCameraScroll()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scrollInput) > Mathf.Epsilon)
        {
            AdjustCameraSize(scrollInput);
        }
    }

    private void AdjustCameraSize(float scrollInput)
    {
        float scrollDirection = Mathf.Sign(scrollInput);
        float scrollAmount = scrollSensitivity * scrollDirection;

        targetOrthographicSize -= scrollAmount;
        targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, minOrthographicSize, maxOrthographicSize);

        SaveCameraSettings();
    }

    private void SmoothCameraSize()
    {
        if (Mathf.Abs(controlledCamera.orthographicSize - targetOrthographicSize) > 0.01f)
        {
            controlledCamera.orthographicSize = Mathf.SmoothDamp(
                controlledCamera.orthographicSize,
                targetOrthographicSize,
                ref currentVelocity,
                smoothTime
            );
        }
        else
        {
            controlledCamera.orthographicSize = targetOrthographicSize;
        }
    }

    private void SaveCameraSettings()
    {
        if (saveSettingsBetweenSessions)
        {
            PlayerPrefs.SetFloat(settingsKey, targetOrthographicSize);
        }
    }

    /// <summary>
    /// Устанавливает целевой размер ортографической камеры
    /// </summary>
    public void SetTargetSize(float newSize, bool immediate = false)
    {
        targetOrthographicSize = Mathf.Clamp(newSize, minOrthographicSize, maxOrthographicSize);

        if (immediate)
        {
            controlledCamera.orthographicSize = targetOrthographicSize;
            currentVelocity = 0f;
        }

        SaveCameraSettings();
    }

    /// <summary>
    /// Сбрасывает настройки камеры для текущей сцены
    /// </summary>
    public void ResetCameraSettings()
    {
        targetOrthographicSize = minOrthographicSize;
        controlledCamera.orthographicSize = targetOrthographicSize;
        currentVelocity = 0f;

        if (saveSettingsBetweenSessions && PlayerPrefs.HasKey(settingsKey))
        {
            PlayerPrefs.DeleteKey(settingsKey);
        }
    }

    /// <summary>
    /// Возвращает текущий целевой размер камеры
    /// </summary>
    public float GetTargetSize() => targetOrthographicSize;

    /// <summary>
    /// Возвращает нормализованное значение размера камеры (0-1)
    /// </summary>
    public float GetNormalizedSize() =>
        (targetOrthographicSize - minOrthographicSize) / (maxOrthographicSize - minOrthographicSize);

    // Для отладки в редакторе
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (minOrthographicSize > maxOrthographicSize)
        {
            minOrthographicSize = maxOrthographicSize - 1f;
        }

        if (scrollSensitivity < 0.1f)
        {
            scrollSensitivity = 0.1f;
        }
    }
#endif
}